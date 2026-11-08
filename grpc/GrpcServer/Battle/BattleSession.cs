using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Server.Hubs;
using MessagePack;
using Byac.ServerShared.Enums;
using Byac.ServerShared.Hub;
using Byac.ServerShared.MessagePackObjects;

namespace Byac.Services.Battle
{
    public class BattleSession
    {
        public Ulid BattleId { get; }
        public string RouteToken { get; }
        public BattleRule Rule { get; }
        public int[] PlayerViewerIds { get; }
        public List<BattlePlayerDataMpo> Players { get; } = new List<BattlePlayerDataMpo>();
        public List<BattleTeamDataMpo> Teams { get; } = new List<BattleTeamDataMpo>();
        public int Round { get; set; } = 1;
        public BattlePhase Phase { get; set; } = BattlePhase.Initialized;

        public sealed class BossContext
        {
            public int DeckId;
            public int PoolHp;
            public int MaxHp;
            public bool Recorded;
        }

        public BossContext Boss { get; private set; }

        internal void SetBossContext(BossContext boss) => Boss = boss;

        internal int TakeNextEquipmentKey() => _nextEquipmentKey++;
        internal IReadOnlyDictionary<int, BattleGuildBuffEffectInfo> GuildBuffEffectsByUnitId => _guildBuffEffectsByUnitId;
        internal void SeedBasePlayerStatus(BattleUnitCalculatedStatusSetMpo status) => _basePlayerStatus = new[] { status };

        private int _bossLeftLife;

        private readonly HashSet<int> _humanSeats = new HashSet<int>();
        private readonly HashSet<int> _attachedHumans = new HashSet<int>();
        private readonly HashSet<int> _everAttachedHumans = new HashSet<int>();
        private readonly Dictionary<int, DateTime> _humanDisconnectedAtUtc = new Dictionary<int, DateTime>();
        private readonly Dictionary<int, Guid> _humanConnectionId = new Dictionary<int, Guid>();

        private readonly Dictionary<int, IGroup> _ownChannels = new Dictionary<int, IGroup>();

        private readonly HashSet<int> _unicastSkipLogged = new HashSet<int>();
        private readonly object _identityLock = new object();
        public readonly DateTime CreatedAtUtc = DateTime.UtcNow;

        public static readonly int ReconnectGraceSeconds =
            int.TryParse(Environment.GetEnvironmentVariable("BYAC_GRACE_SEC"), out var g) && g > 0 ? g : 180;

        public static readonly int DrainGraceSeconds =
            int.TryParse(Environment.GetEnvironmentVariable("BYAC_DRAIN_GRACE_SEC"), out var dg) && dg > 0 ? dg : 0;

        public IGroup Group { get; set; }
        public object SignalLock { get; } = new object();
        public int[] ExpectedSignal { get; set; } = new[] { 0, 0 };
        public bool SequenceStarted { get; set; }
        public HashSet<int> ReadyPlayers { get; } = new HashSet<int>();
        public bool PreparationOpen { get; set; }
        public DateTime? PreparationDeadlineUtc { get; set; }
        public int PreparationEpoch { get; set; }
        public int RoundBeginSentEpoch { get; set; } = -1;
        public bool InitialDistributionDone { get; set; }
        public bool BattleRunning { get; set; }
        public bool Finished { get; set; }

        public DateTime? FinishedAtUtc { get; private set; }
        public bool BattleCompleteEmitted { get; set; }

        private readonly HashSet<int> _personalResultsSent = new HashSet<int>();

        private readonly ConcurrentDictionary<int, bool> _bpGranted = new ConcurrentDictionary<int, bool>();

        private readonly HashSet<int> _abandonedViewers = new HashSet<int>();
        private int _roundResultAckEpoch;

        private readonly Dictionary<int, (int BaseGold, int DistributeGold, int BonusGold, int ResultRewardGold, int InterestGold)> _lastRoundSettle =
            new Dictionary<int, (int, int, int, int, int)>();

        private int _roundDropsEnqueued;

        private readonly Dictionary<int, int> _winStreaks = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _loseStreaks = new Dictionary<int, int>();
        public const int RoundResultAckTimeoutSeconds = 12;

        public const int EliminatedAckTimeoutSeconds = 3;

        public const int BattleSeconds = 60;
        public const float BattleCompleteAtSeconds = 12.0f;
        public const float ResultAnimationGraceSeconds = 4.5f;

        public const float DropItemsLeadSeconds = 0.8f;

        private readonly object _phaseLock = new object();
        private readonly object _readyLock = new object();

        private readonly object _shopPulseLock = new object();
        private readonly HashSet<(int Round, int Viewer)> _shopPulseSent = new();
        private readonly Random _rng = new Random();
        private BattleUnitCalculatedStatusSetMpo[] _basePlayerStatus = Array.Empty<BattleUnitCalculatedStatusSetMpo>();

        private record PvpPair(int MatchId, int PrimaryViewerId, int SecondaryViewerId, bool Corpse = false);
        private record Corpse(int ViewerId, BattleUnitDataMpo[] MirroredUnits,
                              BattleUnitCalculatedStatusSetMpo Status);

        private record Matching(List<(int A, int B)> Pairs, int Repeats, int Within3);
        private PvpPair[] _pvpPairs = Array.Empty<PvpPair>();

        private int _pvpIndex;

        private int _pairingRound = -1;
        private readonly Dictionary<int, HashSet<int>> _met = new();
        private readonly Dictionary<int, Dictionary<int, int>> _lastMetPvpIndex = new();
        private readonly Dictionary<int, int> _byeCounts = new();

        private readonly List<Corpse> _corpses = new();

        private int _battleSalt;

        private int SimRngSalt(int round) => unchecked(_battleSalt * 7919 + round);

        public BattleGuildBuffDataMpo GuildBuffData { get; private set; }

        private Dictionary<int, BattleGuildBuffEffectInfo> _guildBuffEffectsByUnitId =
            new Dictionary<int, BattleGuildBuffEffectInfo>();

        public BattleSession(BattleRule rule, int[] playerViewerIds)
        {
            BattleId = Ulid.NewUlid();
            RouteToken = Guid.NewGuid().ToString("N");
            foreach (var c in BattleId.ToString()) _battleSalt = unchecked(_battleSalt * 31 + c);
            _battleSalt &= 0x7FFFFFFF;
            Rule = rule;
            PlayerViewerIds = playerViewerIds;

            GuildBuffData = DrawGuildBuffData();
        }

        private BattleGuildBuffDataMpo DrawGuildBuffData()
        {
            try
            {
                var rows = BattleMaster.GuildBuffRows;
                if (rows.Count == 0)
                {
                    Console.WriteLine("[BATTLE] guild_buff_data empty -> fallback GuildId=1");
                    return MakeFallbackGuildBuffData();
                }

                var total = rows.Sum(r => r.LotteryRate);
                if (total <= 0) total = rows.Count;
                var roll = _rng.Next(total);
                var selected = rows[rows.Count - 1];
                var boundary = 0;
                foreach (var row in rows)
                {
                    boundary += row.LotteryRate;
                    if (roll < boundary) { selected = row; break; }
                }

                var others = rows.Where(r => r.GuildId != selected.GuildId)
                    .Select(r => r.GuildId).ToList();
                for (int i = others.Count - 1; i > 0; i--)
                {
                    int j = _rng.Next(i + 1);
                    (others[i], others[j]) = (others[j], others[i]);
                }
                others.Add(selected.GuildId);

                var targets = BattleMaster.GuildNonTokenUnitIds(selected.GuildId);
                var buffIds = new int[targets.Length];
                for (int i = 0; i < targets.Length; i++)
                {
                    buffIds[i] = selected.GuildBuffIds.Length > 0
                        ? selected.GuildBuffIds[_rng.Next(selected.GuildBuffIds.Length)]
                        : 0;
                }

                _guildBuffEffectsByUnitId.Clear();
                int appliedCount = 0, skippedRuntime = 0, skippedMissing = 0;
                for (int i = 0; i < targets.Length; i++)
                {
                    var effect = BattleMaster.GuildBuffEffect(buffIds[i]);
                    if (effect == null) { skippedMissing++; continue; }
                    if (effect.NeedsRuntimeHandler)
                    {
                        skippedRuntime++;
                        continue;
                    }
                    _guildBuffEffectsByUnitId[targets[i]] = effect;
                    appliedCount++;
                }

                Console.WriteLine($"[BATTLE] guild buff effects: applied={appliedCount} skippedRuntime={skippedRuntime} skippedMissing={skippedMissing} buffIds=[{string.Join(",", buffIds)}]");

                var data = new BattleGuildBuffDataMpo
                {
                    GuildId = (GuildId)selected.GuildId,
                    GuildBuffIds = buffIds,
                    DrawnGuildIds = others.ToArray(),
                    TargetUnitIds = targets,
                };
                Console.WriteLine($"[BATTLE] guild roulette draw guild={selected.GuildId} drawnCount={others.Count} targets=[{string.Join(",", targets)}] buffs=[{string.Join(",", buffIds)}]");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BATTLE] guild roulette draw EX -> fallback GuildId=1: {ex}");
                return MakeFallbackGuildBuffData();
            }
        }

        private static BattleGuildBuffDataMpo MakeFallbackGuildBuffData()
            => new BattleGuildBuffDataMpo
            {
                GuildId = (GuildId)1,
                GuildBuffIds = new[] { 1, 2, 3, 4, 5, 7 },
                DrawnGuildIds = new[] { 1 },
                TargetUnitIds = Array.Empty<int>(),
            };

        public int HumanViewerId => PlayerViewerIds[0];

        public void DeclareHumanSeats(IEnumerable<int> viewerIds)
        {
            lock (_identityLock)
            {
                foreach (var v in viewerIds) _humanSeats.Add(v);
            }
        }

        public IReadOnlyCollection<int> HumanViewerIds
        {
            get { lock (_identityLock) return _humanSeats.ToArray(); }
        }

        public bool IsHuman(int viewerId)
        {
            lock (_identityLock) return _humanSeats.Contains(viewerId);
        }

        public bool IsAttached(int viewerId)
        {
            lock (_identityLock) return _attachedHumans.Contains(viewerId);
        }

        public bool IsEverAttached(int viewerId)
        {
            lock (_identityLock) return _everAttachedHumans.Contains(viewerId);
        }

        public bool HasHumanAttached
        {
            get { lock (_identityLock) return _attachedHumans.Count > 0; }
        }

        public bool EverHadHumanAttached
        {
            get { lock (_identityLock) return _everAttachedHumans.Count > 0; }
        }

        public DateTime? DisconnectAt(int viewerId)
        {
            lock (_identityLock)
            {
                if (_attachedHumans.Contains(viewerId)) return null;
                if (_humanDisconnectedAtUtc.TryGetValue(viewerId, out var t)) return t;
                return _humanSeats.Contains(viewerId) ? CreatedAtUtc : (DateTime?)null;
            }
        }

        public bool GraceExpired(int viewerId)
        {
            var at = DisconnectAt(viewerId);
            return at.HasValue && (DateTime.UtcNow - at.Value).TotalSeconds > ReconnectGraceSeconds;
        }

        public bool IsDrivenByServer(int viewerId)
        {
            if (!IsHuman(viewerId)) return true;
            if (IsAttached(viewerId)) return false;
            return GraceExpired(viewerId);
        }

        public void MarkHumanAttached(int viewerId, Guid connectionId)
        {
            lock (_identityLock)
            {
                _humanSeats.Add(viewerId);
                _attachedHumans.Add(viewerId);
                _everAttachedHumans.Add(viewerId);
                _humanConnectionId[viewerId] = connectionId;
                _humanDisconnectedAtUtc.Remove(viewerId);
            }
        }

        public void MarkHumanDisconnected(int viewerId, Guid connectionId)
        {
            lock (_identityLock)
            {
                if (!_attachedHumans.Contains(viewerId)) return;
                if (_humanConnectionId.TryGetValue(viewerId, out var owner) && owner != connectionId) return;
                _attachedHumans.Remove(viewerId);
                _humanConnectionId.Remove(viewerId);
                _humanDisconnectedAtUtc[viewerId] = DateTime.UtcNow;
            }

            Console.WriteLine($"[BATTLE] human detached viewer={viewerId} grace={ReconnectGraceSeconds}s battle={BattleId}");
        }

        public bool HasOtherLiveHumanAttached(int viewerId)
        {
            int[] candidates;
            lock (_identityLock) candidates = _attachedHumans.ToArray();
            foreach (var vid in candidates)
            {
                if (vid == viewerId) continue;
                var p = FindPlayer(vid);
                if (p.ViewerId != 0 && p.Life > 0 && p.Alive) return true;
            }
            return false;
        }

        public bool HasOtherLiveHumanSeat(int viewerId)
        {
            foreach (var vid in HumanViewerIds)
            {
                if (vid == viewerId) continue;
                var p = FindPlayer(vid);
                if (p.ViewerId != 0 && p.Life > 0 && p.Alive) return true;
            }
            return false;
        }

        public int[] LiveAttachedHumans()
        {
            int[] candidates;
            lock (_identityLock) candidates = _attachedHumans.ToArray();
            return candidates.Where(vid =>
            {
                var p = FindPlayer(vid);
                return p.ViewerId != 0 && p.Life > 0 && p.Alive;
            }).ToArray();
        }

        public void RegisterChannel(int viewerId, IGroup own)
        {
            lock (_identityLock)
            {
                _ownChannels[viewerId] = own;
                _unicastSkipLogged.Remove(viewerId);
            }
        }

        public void UnregisterChannel(int viewerId, IGroup own)
        {
            lock (_identityLock)
            {
                if (_ownChannels.TryGetValue(viewerId, out var g) && ReferenceEquals(g, own))
                {
                    _ownChannels.Remove(viewerId);
                }
            }
        }

        public BattlePlayerDataMpo FindPlayer(int viewerId)
        {
            foreach (var p in Players)
            {
                if (p.ViewerId == viewerId) return p;
            }
            return default;
        }

        public void InitializeInitialDistribution()
        {
            if (InitialDistributionDone) return;
            InitialDistributionDone = true;

            if (Rule == BattleRule.Boss) return;
            var pool = BattleMaster.InitialPool;
            if (pool == null || pool.Length == 0)
            {
                Console.WriteLine("[BATTLE] !! initial pool empty, skipping distribution");
                return;
            }

            var testRankup = Environment.GetEnvironmentVariable("BYAC_TEST_RANKUP") == "1";
            var testEquip = Environment.GetEnvironmentVariable("BYAC_TEST_EQUIP") == "1";
            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i];
                var list = new List<BattleUnitDataMpo>();
                var forcedId = 0;
                for (int j = 0; j < 5; j++)
                {
                    int unitId;
                    if (testRankup && j < 3)
                    {
                        if (forcedId == 0) forcedId = pool[_rng.Next(pool.Length)];
                        unitId = forcedId;
                    }
                    else
                    {
                        unitId = pool[_rng.Next(pool.Length)];
                    }
                    list.Add(BattleMpoFactory.MakeUnit(200000 + i * 100 + j, unitId, 0, 0, UnitState.Shop, j));
                }
                p.InitialDistributeUnitList = list;
                if (testEquip)
                {
                    var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
                    inv.Add(BattleMpoFactory.MakeEquipment(_nextEquipmentKey++, 300110));
                    inv.Add(BattleMpoFactory.MakeEquipment(_nextEquipmentKey++, 300510));
                    p.EquipmentList = inv;
                    Console.WriteLine($"[BATTLE] test-equip injected 2 items viewer={p.ViewerId}");
                }
                p.ShopUnitList = RollShopUnits(p.ViewerId, Math.Max(1, p.Level));
                p.ShopUnitLotteryRates = ProjectLotteryRates(CurrentLotteryPattern(), Math.Max(1, p.Level));
                Players[i] = p;
            }
            Console.WriteLine($"[BATTLE] initial distribution ready pool={pool.Length} testRankup={testRankup} testEquip={testEquip}");
        }

        private void FinalizeInitialDistribution()
        {

            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i];
                var candidates = p.InitialDistributeUnitList ?? new List<BattleUnitDataMpo>();
                var field = new List<BattleUnitDataMpo>();
                var deck = new List<BattleUnitDataMpo>();
                for (int j = 0; j < candidates.Count; j++)
                {
                    var u = candidates[j];
                    u.State = UnitState.Deck;
                    u.Index = deck.Count;
                    deck.Add(u);
                }
                p.FieldUnitList = field;
                p.DeckUnitList = deck;
                p.InitialDistributeUnitList = new List<BattleUnitDataMpo>();

                p.CalculatedStatus = BattleMpoFactory.MakeStatusSetForUnits(
                    field, BattleMaster.EvaluateFieldSynergies(field), field, _guildBuffEffectsByUnitId);
                _basePlayerStatus = _basePlayerStatus.Length == Players.Count
                    ? _basePlayerStatus
                    : new BattleUnitCalculatedStatusSetMpo[Players.Count];
                _basePlayerStatus[i] = p.CalculatedStatus;
                Players[i] = p;

                var merges = 0;
                while (merges < 16 && TryDeferredMergeOnce(ref p, "initial"))
                {
                    merges++;
                    Players[i] = p;
                    RefreshSynergies(i);
                }
                if (merges > 0)
                    Console.WriteLine($"[BATTLE] initial auto-merge viewer={p.ViewerId} merges={merges} deck={(p.DeckUnitList?.Count ?? 0)}");
            }
            Console.WriteLine("[BATTLE] initial distribution finalized (0 field, all deck, official 0/1 open)");
        }

        private readonly ConcurrentQueue<DateTime> _broadcastTicks = new();

        private void NoteBroadcast()
        {
            _broadcastTicks.Enqueue(DateTime.UtcNow);
            while (_broadcastTicks.Count > 2048) _broadcastTicks.TryDequeue(out _);
        }

        internal int BroadcastsInLastSeconds(int seconds)
        {
            var cutoff = DateTime.UtcNow - TimeSpan.FromSeconds(seconds);
            var n = 0;
            foreach (var t in _broadcastTicks)
            {
                if (t >= cutoff) n++;
            }
            return n;
        }

        private readonly object _diagLock = new();
        private readonly List<(int viewer, int bc15s, DateTime at)> _dropDiag = new();
        private readonly List<(int viewer, float rate, DateTime at)> _rateDiag = new();

        public void RecordViewerDrop(int viewerId, int bc15s)
        {
            lock (_diagLock)
            {
                if (_dropDiag.Count < 64) _dropDiag.Add((viewerId, bc15s, DateTime.UtcNow));
            }
        }

        internal void RecordIntervalRate(int viewerId, float rate)
        {
            lock (_diagLock)
            {
                if (_rateDiag.Count < 64) _rateDiag.Add((viewerId, rate, DateTime.UtcNow));
            }
        }

        private void DumpConnectionDiagnostics()
        {
            lock (_diagLock)
            {
                if (_dropDiag.Count == 0 && _rateDiag.Count == 0) return;
                var drops = string.Join(",", _dropDiag.Select(d => $"v{d.viewer}@{d.at:HH:mm:ss}:bc{d.bc15s}"));
                var rates = _rateDiag.Count == 0
                    ? "-"
                    : string.Join(",", _rateDiag.Select(r => $"v{r.viewer}@{r.at:HH:mm:ss}:r{r.rate:F2}"));
                Console.WriteLine($"[BATTLE] conn-diag battle={BattleId} round={Round} drops[{_dropDiag.Count}]={drops} rates[{_rateDiag.Count}]={rates}");
            }
        }

        public NetTrace Net
        {
            get
            {
                if (_net != null) return _net;
                lock (_netLock) _net = _net ?? new NetTrace(BattleId.ToString());
                return _net;
            }
        }
        private NetTrace _net;
        private readonly object _netLock = new object();

        private void Broadcast(Action<IBattleHubReceiver> send)
        {
            NoteBroadcast();
            SendTo(Group, send, "room");
        }

        public async Task SendRemainingAxisToViewer(int viewerId)
        {
            List<(float at, BattleBoardEventResponse ev)>? toSend = null;
            lock (_liveAxisLock)
            {
                if (_liveAxis == null) return;
                int sent = Math.Min(_liveAxisSent, _liveAxis.Count);
                if (sent <= 0) return;
                toSend = _liveAxis.GetRange(0, sent);
            }
            Console.WriteLine($"[BATTLE] axis re-send viewer={viewerId} events={toSend.Count} (paced replay)");
            ServerMetrics.ReconnectReplay(toSend.Count);
            long pacedBytes = 0;
            foreach (var group in toSend.GroupBy(e => (int)MathF.Round(e.at / SimpleTickSimulator.MinFrameGapSeconds)).OrderBy(g => g.Key))
            {
                foreach (var (subFrame, subBytes) in SplitFrameByBytes(group.Select(e => e.ev).ToArray()))
                {

                    if (pacedBytes > 192 * 1024)
                    {
                        int delayMs = Math.Max(25, (int)(subBytes * 1000.0 / (512 * 1024)));
                        await Delay(delayMs);
                    }
                    BroadcastToViewer(viewerId, bc => bc.OnBoardEventReceived(subFrame, 0.0f, OperateType.NORMAL));
                    pacedBytes += subBytes;
                }
            }
        }

        private void BroadcastToViewer(int viewerId, Action<IBattleHubReceiver> send)
        {
            IGroup own;
            lock (_identityLock) _ownChannels.TryGetValue(viewerId, out own);
            SendTo(own ?? Group, send, $"viewer={viewerId}{(own == null ? " (room fallback)" : "")}");
        }

        private void BroadcastBoundary(string frame, string info, Action<IBattleHubReceiver> send)
        {
            NoteBroadcast();
            int[] viewers;
            lock (_identityLock) viewers = _ownChannels.Keys.ToArray();
            if (viewers.Length == 0)
            {
                SendTo(Group, send, $"room boundary={frame}");
                return;
            }
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var ok = 0;
            var fails = new List<int>();
            foreach (var v in viewers)
            {
                IGroup own;
                lock (_identityLock) _ownChannels.TryGetValue(v, out own);
                if (own == null) continue;
                if (SendTo(own, send, $"viewer={v} boundary={frame}")) ok++;
                else fails.Add(v);
            }
            sw.Stop();
            Console.WriteLine($"[BATTLE] boundary round={Round} frame={frame} viewers={viewers.Length} ok={ok} fails=[{string.Join(",", fails)}] sendMs={sw.ElapsedMilliseconds} {info}");
        }

        private const int FrameBytesCap = 64 * 1024;

        private static List<(BattleBoardEventResponse[] Frame, int Bytes)> SplitFrameByBytes(BattleBoardEventResponse[] frame)
        {
            var result = new List<(BattleBoardEventResponse[], int)>(frame.Length / 16 + 1);
            if (frame.Length == 0) return result;
            var cur = new List<BattleBoardEventResponse>(frame.Length);
            int curBytes = 0;
            foreach (var ev in frame)
            {
                int sz;
                try { sz = MessagePackSerializer.Serialize(ev.GetType(), ev).Length; }
                catch { sz = 256; }
                if (cur.Count > 0 && curBytes + sz > FrameBytesCap)
                {
                    result.Add((cur.ToArray(), curBytes));
                    cur = new List<BattleBoardEventResponse>(frame.Length);
                    curBytes = 0;
                }
                cur.Add(ev);
                curBytes += sz;
            }
            if (cur.Count > 0) result.Add((cur.ToArray(), curBytes));
            return result;
        }

        private enum UniScopeMode { Off, Thin, Own }

        private static readonly UniScopeMode UniScope =
            Enum.TryParse<UniScopeMode>(
                Environment.GetEnvironmentVariable("BYAC_UNI_SCOPE")?.Trim() ?? "off", true, out var uniScopeParsed)
                ? uniScopeParsed : UniScopeMode.Off;

        private static bool KeepForSpectate(BattleBoardEventResponse e) =>
            e is not (BattleBoardStatusEffectAddedEventResponse
                or BattleBoardStatusEffectRemovedEventResponse
                or BattleBoardUnitStatusSyncEventResponse
                or BattleBoardStunEventResponse
                or BattleBoardChangeTechnicalPointEventResponse
                or BattleBoardBarrierEventResponse
                or BattleBoardAttachBarrierEventResponse
                or BattleBoardBarrierDamageEventResponse);

        private BattleBoardEventResponse[] FilterFrameForViewer(BattleBoardEventResponse[] frame, int ownMatch)
        {
            if (UniScope == UniScopeMode.Own)
                return frame.Where(e => e.MatchId == ownMatch || e.MatchId == 0).ToArray();
            return frame.Where(e => e.MatchId == ownMatch || e.MatchId == 0 || KeepForSpectate(e)).ToArray();
        }

        private bool BroadcastBoardFrames(BattleBoardEventResponse[] frame,
            Dictionary<int, int> sent, Dictionary<int, int> failed)
        {
            int[] viewers;
            lock (_identityLock) viewers = _ownChannels.Keys.ToArray();
            if (viewers.Length == 0)
            {
                Broadcast(bc => bc.OnBoardEventReceived(frame, 0.0f, OperateType.NORMAL));
                return true;
            }
            foreach (var v in viewers)
            {
                IGroup own;
                lock (_identityLock) _ownChannels.TryGetValue(v, out own);

                var frameForV = frame;
                if (UniScope != UniScopeMode.Off
                    && _ownMatchByViewerRound == Round
                    && _ownMatchByViewer.TryGetValue(v, out var ownMatch))
                    frameForV = FilterFrameForViewer(frame, ownMatch);
                if (frameForV.Length == 0) continue;
                if (SendTo(own, bc => bc.OnBoardEventReceived(frameForV, 0.0f, OperateType.NORMAL), $"viewer={v} board"))
                    sent[v] = sent.TryGetValue(v, out int c) ? c + 1 : 1;
                else
                    failed[v] = failed.TryGetValue(v, out int f) ? f + 1 : 1;
            }
            return false;
        }

        private bool TryUnicastToViewer(int viewerId, Action<IBattleHubReceiver> send)
        {
            IGroup own;
            bool announce = false;
            lock (_identityLock)
            {
                _ownChannels.TryGetValue(viewerId, out own);

                if (own == null && _everAttachedHumans.Contains(viewerId)
                    && _unicastSkipLogged.Add(viewerId)) announce = true;
            }
            if (announce)
                Console.WriteLine($"[BATTLE] !! unicast skipped (detached viewer) viewer={viewerId}");
            if (own == null) return false;
            return SendTo(own, send, $"viewer={viewerId}");
        }

        private bool SendTo(IGroup g, Action<IBattleHubReceiver> send, string target)
        {
            try
            {
                if (g == null)
                {
                    Console.WriteLine($"[BATTLE] !! broadcast skipped (no group) target={target}");
                    return false;
                }
                IBattleHubReceiver caster = g.CreateBroadcaster<IBattleHubReceiver>();
                if (FrameDump.Enabled)
                {
                    caster = FrameDump.Wrap(caster);
                }
                if (NetTrace.Enabled)
                {
                    caster = Net.Wrap(caster, target);
                }
                send(caster);
                return true;
            }
            catch (Exception ex)
            {
                ServerMetrics.BroadcastError();

                string inner = ex.InnerException != null ? $" <- {ex.InnerException.GetType().Name}: {ex.InnerException.Message}" : "";
                Console.WriteLine($"[BATTLE] broadcast EX target={target} {ex.GetType().Name}: {ex.Message}{inner}");
                return false;
            }
        }

        private BattlePlayerDataMpo[] SnapshotPlayers()
        {
            return Players.ToArray();
        }

        public void BroadcastUpdatePlayers() => Broadcast(bc => bc.OnUpdatePlayers(Players.ToArray()));

        private static Task Delay(int ms) => Task.Delay(ms);

        public void RegisterReady(int viewerId)
        {
            int epoch;
            lock (_readyLock)
            {
                ReadyPlayers.Add(viewerId);
                epoch = PreparationEpoch;
            }
            SendOwnShopPulse(viewerId);
            BroadcastReadyList();
            CheckAllReady(epoch);
        }

        public void RemoveReady(int viewerId)
        {
            lock (_readyLock)
            {
                ReadyPlayers.Remove(viewerId);
            }
            SendOwnShopPulse(viewerId);
        }

        private void SendOwnShopPulse(int viewerId)
        {
            if (!IsHuman(viewerId)) return;
            if (!CanTrade(viewerId)) return;
            var idx = PlayerIndex(viewerId);
            if (idx < 0) return;
            lock (_shopPulseLock)
            {
                if (!_shopPulseSent.Add((Round, viewerId))) return;
            }
            var snapshot = Players[idx];
            if (TryUnicastToViewer(viewerId, bc => bc.OnRefreshShopUnit(snapshot, null)))
                Console.WriteLine($"[BATTLE] shop pulse viewer={viewerId} round={Round} phase={Phase}");
        }

        private void BroadcastReadyList()
        {
            int[] ready;
            lock (_readyLock)
            {
                var list = new List<int>(ReadyPlayers);
                list.Sort();
                ready = list.ToArray();
            }
            Broadcast(bc => bc.OnReady(ready));
        }

        private void CheckAllReady(int epoch)
        {
            bool all;
            lock (_readyLock)
            {

                int need = 0;
                foreach (var p in Players)
                {
                    if (p.ViewerId == 0) continue;
                    bool eliminated = p.Life <= 0 || !p.Alive;
                    if (eliminated && IsHuman(p.ViewerId) && !IsDrivenByServer(p.ViewerId)) continue;
                    need++;
                }
                all = Players.Count > 0 && ReadyPlayers.Count >= Math.Max(1, need);
            }
            if (!all) return;
            _ = Task.Run(async () =>
            {
                await Delay(100);
                if (TryMarkPrepared())
                {
                    Console.WriteLine($"[BATTLE] all-ready boundary epoch={epoch}");
                    Broadcast(bc => bc.OnPhaseChanged(BattlePhase.Prepared, BattlePhase.Preparing));
                    await Delay(200);

                    if (Rule != BattleRule.Boss) CheckFieldUnitCount();
                    SendRoundBegin(epoch);
                }
            });
        }

        private bool TryMarkPrepared()
        {
            lock (_phaseLock)
            {
                if (!PreparationOpen || Phase != BattlePhase.Preparing) return false;
                PreparationOpen = false;
                PreparationDeadlineUtc = null;
                Phase = BattlePhase.Prepared;
                return true;
            }
        }

        private void SendRoundBegin(int epoch)
        {
            lock (_phaseLock)
            {
                if (RoundBeginSentEpoch == epoch) return;
                if (Phase != BattlePhase.Prepared) return;
                RoundBeginSentEpoch = epoch;
            }
            var data = BuildRoundBeginData();

            BroadcastBoundary("OnRoundBegin", $"matches={data.Matches.Length} players={data.Players.Length}", bc => bc.OnRoundBegin(data));

            RoundBeginSentAtUtc = DateTime.UtcNow;

            if (AiMirrorOn && Rule == BattleRule.Normal && !VerifyMode.IsActive)
            {
                AiMirrorStore.RecordRound(BattleId.ToString(), Round,
                    Players.Where(p => p.ViewerId != 0 && IsHuman(p.ViewerId) && p.Alive && p.Life > 0).ToArray());
            }
            ArmBeginRoundFallback();
        }

        public DateTime RoundBeginSentAtUtc { get; private set; }

        public void SendRoundBeginToViewer(int viewerId)
        {
            if (Phase != BattlePhase.Prepared) return;
            var data = BuildRoundBeginData();
            Console.WriteLine($"[BATTLE] OnRoundBegin re-sent viewer={viewerId} round={Round} matches={data.Matches.Length}");
            BroadcastToViewer(viewerId, bc => bc.OnRoundBegin(data));
        }

        private void ArmBeginRoundFallback()
        {
            var round = Round;
            var timeoutSec = 15 + DrainGraceSeconds;
            _ = Task.Run(async () =>
            {
                await Delay(timeoutSec * 1000);
                if (Finished || BattleRunning || Phase != BattlePhase.Prepared) return;
                Console.WriteLine($"[BATTLE] BeginRound fallback fires round={round} after {timeoutSec}s (client signal missed) -> force start");
                lock (SignalLock) ExpectedSignal = new[] { round, 2 };
                BattleRunning = true;
                await RunSimpleBattleAsync();
            });
        }

        private SetupRoundResponse BuildSetupRoundResponse()
        {
            return new SetupRoundResponse
            {
                Round = Round,
                Players = SnapshotPlayers(),
                FreeShopRefreshInfo = new FreeShopRefreshResponse[0],

                BattleOption = BattleOption.DropBoost,
                FieldCellDatas = new FieldCellDataMpo[0],
                RewardPlayerIds = new int[0],
            };
        }

        private (BattleUnitDataMpo[] units, BattleUnitCalculatedStatusSetMpo status) BuildNpcUnits(int npcId, int playerIndex, bool weakenNpc)
        {
            var units = new List<BattleUnitDataMpo>();

            var layout = BattleMaster.NpcLayout(npcId);
            for (int i = 0; i < layout.Length; i++)
            {
                var (unitId, x, y) = layout[i];
                var key = 1_000_000 + playerIndex * 100 + i;
                units.Add(BattleMpoFactory.MakeUnit(key, unitId, x, y, UnitState.Field, i, isNpc: true));
            }

            var status = BattleMpoFactory.MakeStatusSetForUnits(units,
                null, null, _guildBuffEffectsByUnitId);
            if (weakenNpc) status = WeakenLife(status);
            return (units.ToArray(), status);
        }

        private static BattleUnitDataMpo[] MirrorHalf(IEnumerable<BattleUnitDataMpo> units)
        {
            if (units == null) return new BattleUnitDataMpo[0];
            var list = new List<BattleUnitDataMpo>();
            foreach (var u in units)
            {
                if (u == null) continue;
                list.Add(new BattleUnitDataMpo
                {
                    UniqueKey = u.UniqueKey,
                    UnitId = u.UnitId,
                    Rank = u.Rank,
                    EquipmentList = u.EquipmentList,
                    State = u.State,
                    PositionX = 5 - u.PositionX,
                    PositionY = 7 - u.PositionY,
                    Index = u.Index,
                    IsPurchased = u.IsPurchased,
                    FieldDeployAt = u.FieldDeployAt,
                    IsToken = u.IsToken,
                    IsNpc = u.IsNpc,
                    IsHelp = u.IsHelp,
                    OriginPlayerId = u.OriginPlayerId,
                    MirroringRoleType = u.MirroringRoleType,
                    MirroringTeamType = u.MirroringTeamType,
                    TotalSkillCount = u.TotalSkillCount,
                });
            }
            return list.ToArray();
        }

        private static void PersistChargeSeals(List<BattleUnitDataMpo> fieldUnits, SimpleTickSimulator.SimResult sim)
        {
            if (fieldUnits == null || sim == null || sim.ChargeDeltas.Count == 0) return;
            foreach (var u in fieldUnits)
            {
                if (u == null) continue;
                if (sim.ChargeDeltas.TryGetValue(u.UniqueKey, out var delta) && delta > 0)
                    u.TotalSkillCount = Math.Min(9, u.TotalSkillCount + delta);
            }
        }

        private sealed class UnitStat
        {
            public int UnitId;
            public int Atk, Hit, Miss, AtkDmg, Ub, SkDmg, Taken, Heal;
            public float FirstAtk = -1f, LastAct = -1f, DeadAt = -1f;
            public char DeadSrc;
        }

        private static void LogUnitStats(int round, int matchId, string side,
            IEnumerable<BattleUnitDataMpo> units, List<(float at, BattleBoardEventResponse ev)> events)
        {
            var stats = new Dictionary<int, UnitStat>();
            if (units != null)
            {
                foreach (var u in units)
                {
                    if (u == null) continue;
                    stats[u.UniqueKey] = new UnitStat { UnitId = u.UnitId };
                }
            }
            void Dead(UnitStat s, float at, bool isDead, char src)
            {
                if (!isDead || s.DeadAt >= 0f) return;
                s.DeadAt = at;
                s.DeadSrc = src;
            }
            foreach (var (at, ev) in events)
            {
                switch (ev)
                {
                    case BattleBoardSkillSummonUnitEventResponse su:
                        foreach (var u in su.AddUnits ?? new BattleUnitDataMpo[0])
                        {
                            if (u == null || stats.ContainsKey(u.UniqueKey)) continue;
                            stats[u.UniqueKey] = new UnitStat { UnitId = u.UnitId };
                        }
                        break;
                    case BattleBoardAttackOperateEventResponse a:
                        if (stats.TryGetValue(a.UniqueKey, out var sa))
                        {
                            sa.Atk++;
                            if (a.Damage > 0) sa.Hit++;
                            if ((a.DamageType & DamageType.Avoid) != 0) sa.Miss++;
                            sa.AtkDmg += a.Damage;
                            if (sa.FirstAtk < 0f) sa.FirstAtk = at;
                            if (at > sa.LastAct) sa.LastAct = at;
                        }
                        if (stats.TryGetValue(a.TargetUniqueKey, out var ta))
                        {
                            ta.Taken += a.Damage;
                            Dead(ta, at, a.IsDead, 'a');
                        }
                        break;
                    case BattleBoardSkillDamageEventResponse sd:
                        if (stats.TryGetValue(sd.OperatorUniqueKey, out var so)) so.SkDmg += sd.Damage;
                        if (stats.TryGetValue(sd.TargetUniqueKey, out var st))
                        {
                            st.Taken += sd.Damage;

                            Dead(st, at, sd.IsDead, sd.Damage == 0 && sd.SkillId == 0 ? '0' : 's');
                        }
                        break;
                    case BattleBoardCounterSkillDamageEventResponse cd:
                        if (stats.TryGetValue(cd.OperatorUniqueKey, out var co)) co.SkDmg += cd.Damage;
                        if (stats.TryGetValue(cd.TargetUniqueKey, out var ct))
                        {
                            ct.Taken += cd.Damage;
                            Dead(ct, at, cd.IsDead, 'c');
                        }
                        break;
                    case BattleBoardEquipmentSkillDamageEventResponse ed:
                        if (stats.TryGetValue(ed.OperatorUnitUniqueKey, out var eo)) eo.SkDmg += ed.Damage;
                        if (stats.TryGetValue(ed.TargetUniqueKey, out var et))
                        {
                            et.Taken += ed.Damage;
                            Dead(et, at, ed.IsDead, 'e');
                        }
                        break;
                    case BattleBoardHpDamageEventResponse hd:
                        if (stats.TryGetValue(hd.TargetUniqueKey, out var ht))
                        {
                            ht.Taken += hd.Damage;
                            Dead(ht, at, hd.IsDead, 'h');
                        }
                        break;
                    case BattleBoardSkillOperateEventResponse spo:
                        if (stats.TryGetValue(spo.UniqueKey, out var sp))
                        {
                            sp.Ub++;
                            if (at > sp.LastAct) sp.LastAct = at;
                        }
                        break;
                    case BattleBoardHpHealEventResponse hh:
                        if (stats.TryGetValue(hh.TargetUniqueKey, out var hhT)) hhT.Heal += hh.Heal;
                        break;
                    case BattleBoardSkillHpHealEventResponse sh:
                        if (stats.TryGetValue(sh.TargetUniqueKey, out var shT)) shT.Heal += sh.Heal;
                        break;
                }
            }
            foreach (var pair in stats)
            {
                var s = pair.Value;
                Console.WriteLine($"[BATTLE] unitstat round={round} m={matchId} side={side} key={pair.Key} u={s.UnitId} atk={s.Atk} hit={s.Hit} miss={s.Miss} atkDmg={s.AtkDmg} ub={s.Ub} skDmg={s.SkDmg} taken={s.Taken} heal={s.Heal} dead={(s.DeadAt < 0f ? "-" : s.DeadAt.ToString("0.00"))}{(s.DeadAt < 0f ? "" : s.DeadSrc.ToString())} lastAct={(s.LastAct < 0f ? "-" : s.LastAct.ToString("0.00"))} firstAtk={(s.FirstAtk < 0f ? "-" : s.FirstAtk.ToString("0.00"))}");
            }
        }

        internal static readonly bool BoardSnapshotOn =
            Environment.GetEnvironmentVariable("BYAC_BOARD_SNAPSHOT") != "0";

        internal static readonly bool FrameLogOn =
            Environment.GetEnvironmentVariable("BYAC_FRAME_LOG") == "1";

        private static void LogBoardSnapshot(int round, int matchId, string side, int viewer, int salt,
            IReadOnlyList<BattleUnitDataMpo> units, BattleUnitCalculatedStatusSetMpo status,
            IReadOnlyList<ActiveSynergyEffect> synergies, IReadOnlyDictionary<int, BattleGuildBuffEffectInfo> guildBuffs)
        {
            if (!BoardSnapshotOn) return;
            var syn = synergies == null || synergies.Count == 0 ? "-" : string.Join(",",
                synergies.Select(s => $"{(s.SynergyType == 0 ? "T" : "R")}{s.Id}@{s.Level}"));

            var gb = guildBuffs == null || guildBuffs.Count == 0 ? "-" : string.Join(",",
                guildBuffs.Select(kv => $"{kv.Key}:b{kv.Value.GuildBuffId}tp{(int)kv.Value.TpRatePercent}"));
            Console.WriteLine($"[BATTLE] board round={round} m={matchId} side={side} viewer={viewer} salt={salt} n={units?.Count ?? 0} syn={syn} gb={gb}");
            if (units == null) return;
            var byKey = new Dictionary<int, BattleUnitCalculatedStatusMpo>();
            foreach (var s in status?.UnitStatuses ?? Array.Empty<BattleUnitCalculatedStatusMpo>())
                if (s != null) byKey[s.UniqueKey] = s;
            foreach (var u in units)
            {
                if (u == null) continue;
                var st = byKey.TryGetValue(u.UniqueKey, out var c) ? (c.Adjusted ?? c.Current ?? c.Base) : null;
                var gear = u.EquipmentList == null || u.EquipmentList.Count == 0 ? "-"
                    : string.Join(",", u.EquipmentList.Where(e => e != null).Select(e => e.Id));

                var mir = $"R{(u.MirroringRoleType == null || u.MirroringRoleType.Count == 0 ? "-" : string.Join("/", u.MirroringRoleType.Select(x => (int)x)))}"
                    + $"T{(u.MirroringTeamType == null || u.MirroringTeamType.Count == 0 ? "-" : string.Join("/", u.MirroringTeamType.Select(x => (int)x)))}";

                Console.WriteLine($"[BATTLE] board-unit round={round} m={matchId} side={side} key={u.UniqueKey} u={u.UnitId}"
                    + $" r={u.Rank} pos=({u.PositionX},{u.PositionY}) idx={u.Index} gear={gear} seals={u.TotalSkillCount} mir={mir}"
                    + $" flag={(u.IsToken ? "t" : "")}{(u.IsNpc ? "n" : "")}{(u.IsHelp ? "h" : "")}"
                    + $" hp={Or(st?.Life)} atk={Or(st?.Attack)} matk={Or(st?.MagicAttack)} def={Or(st?.Defense)} mdef={Or(st?.MagicDefense)}"
                    + $" recast={OrF(st?.AttackRecastTime)} range={OrF(st?.AttackRange)} tp={Or(st?.TechnicalPoint)}/{Or(st?.MaxTechnicalPoint)}");
            }
        }

        private static int Or(int? v) => v ?? -1;
        private static float OrF(float? v) => v ?? -1f;

        private static BattleUnitCalculatedStatusSetMpo CloneStatusSet(BattleUnitCalculatedStatusSetMpo set)
        {
            if (set == null) return null;
            var list = new List<BattleUnitCalculatedStatusMpo>();
            foreach (var st in set.UnitStatuses ?? new BattleUnitCalculatedStatusMpo[0])
            {
                list.Add(new BattleUnitCalculatedStatusMpo
                {
                    UniqueKey = st.UniqueKey,
                    Current = BattleMpoFactory.CloneStatus(st.Current),
                    Adjusted = BattleMpoFactory.CloneStatus(st.Adjusted),
                    Base = BattleMpoFactory.CloneStatus(st.Base),
                });
            }
            return BattleMpoFactory.MakeStatusSet(list.ToArray());
        }

        private static BattleUnitCalculatedStatusSetMpo WeakenLife(BattleUnitCalculatedStatusSetMpo set)
        {
            if (set?.UnitStatuses == null) return set;
            foreach (var st in set.UnitStatuses)
            {
                if (st.Current != null) st.Current.Life = 1;
                if (st.Adjusted != null) st.Adjusted.Life = 1;
                if (st.Base != null) st.Base.Life = 1;
            }
            return set;
        }

        private static BoardUnitStatusMpo FindUnitStatus(BattleUnitCalculatedStatusSetMpo set, int uniqueKey)
        {
            if (set?.UnitStatuses == null) return null;
            foreach (var st in set.UnitStatuses)
            {
                if (st.UniqueKey == uniqueKey) return st.Current;
            }
            return null;
        }

        private BattleUnitCalculatedStatusSetMpo CurrentPlayerStatusSet(int playerIndex)
        {

            return _basePlayerStatus.Length > playerIndex ? _basePlayerStatus[playerIndex] : null;
        }

        private void RefreshSynergies(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= Players.Count) return;
            var p = Players[playerIndex];

            p.UnitMirroringPairs = BattleMaster.ApplyMirroring(p.FieldUnitList);
            var synergies = BattleMaster.EvaluateFieldSynergies(p.FieldUnitList);

            var set = BattleMpoFactory.MakeStatusSetForUnits(
                (p.FieldUnitList ?? new List<BattleUnitDataMpo>())
                    .Concat(p.DeckUnitList ?? new List<BattleUnitDataMpo>()), synergies, p.FieldUnitList,
                _guildBuffEffectsByUnitId);
            p.CalculatedStatus = set;
            if (playerIndex < _basePlayerStatus.Length) _basePlayerStatus[playerIndex] = set;
            Players[playerIndex] = p;
        }

        internal NpcRoundStartResponse[] BuildNpcRoundStartResponses(int npcId)
        {

            var weakenNpc = false;
            var list = new List<NpcRoundStartResponse>();
            for (int i = 0; i < Players.Count; i++)
            {
                var (units, status) = BuildNpcUnits(npcId, i, weakenNpc);

                if (Rule == BattleRule.Boss && Boss != null) ApplyBossPoolHp(units, status);
                var player = Players[i];
                var playerStatus = CurrentPlayerStatusSet(i) ?? player.CalculatedStatus;
                player.CalculatedStatus = playerStatus;
                list.Add(new NpcRoundStartResponse
                {
                    Player = player,
                    NpcUnits = units,
                    CalculatedStatus = status,
                });
            }
            return list.ToArray();
        }

        private PvpPair[] BuildPvpPairs(int round)
        {
            if (round == _pairingRound) return _pvpPairs;

            _pvpIndex++;

            var seats = Players.Where(p => p.Life > 0 && p.Alive)
                               .OrderBy(p => p.Index)
                               .Select(p => p.ViewerId).ToList();
            List<(int A, int B)> matched;
            int bye = 0;
            Corpse? corpse = null;

            if (seats.Count % 2 == 1)
            {

                var minBye = int.MaxValue;
                foreach (var v in seats) minBye = Math.Min(minBye, _byeCounts.GetValueOrDefault(v));
                var best = default(Matching);
                foreach (var v in seats)
                {
                    if (_byeCounts.GetValueOrDefault(v) != minBye) continue;
                    var rest = new List<int>(seats);
                    rest.Remove(v);
                    var m = BestMatching(Rotate(rest));
                    if (best == null || m.Repeats < best.Repeats
                        || (m.Repeats == best.Repeats && m.Within3 <= best.Within3)) { best = m; bye = v; }
                }
                _byeCounts[bye] = _byeCounts.GetValueOrDefault(bye) + 1;
                matched = best.Pairs;
                if (_corpses.Count > 0) corpse = _corpses[^1];

            }
            else matched = BestMatching(Rotate(seats)).Pairs;

            var pairs = new List<PvpPair>();
            foreach (var (a, b) in matched)
            {
                MarkMet(a, b);
                pairs.Add(MakePair(pairs.Count + 1, a, b));
            }
            if (corpse != null)
            {

                pairs.Add(new PvpPair(pairs.Count + 1, bye, corpse.ViewerId, Corpse: true));
            }

            _pairingRound = round;
            _pvpPairs = pairs.ToArray();
            Console.WriteLine($"[BATTLE] PAIR round={round} pvpIndex={_pvpIndex} alive={seats.Count} " +
                $"pairs=[{string.Join(" ", _pvpPairs.Select(p => $"{p.PrimaryViewerId}v{p.SecondaryViewerId}{(p.Corpse ? "(dead)" : "")}"))}] " +
                $"bye={bye} corpses={_corpses.Count}");
            return _pvpPairs;
        }

        private Matching BestMatching(List<int> pool)
        {
            var cur = new List<(int A, int B)>();
            var bestPairs = new List<(int A, int B)>();
            var bestRepeats = int.MaxValue;
            var bestWithin3 = int.MaxValue;

            void Search(List<int> open)
            {
                if (open.Count == 0)
                {
                    var rep = 0; var w3 = 0;
                    foreach (var (a, b) in cur)
                    {
                        var tier = OpponentTier(a, b);
                        if (tier != 0) rep++;
                        if (tier == 2) w3++;
                    }
                    if (rep > bestRepeats || (rep == bestRepeats && w3 > bestWithin3)) return;
                    bestRepeats = rep; bestWithin3 = w3;
                    bestPairs.Clear();
                    bestPairs.AddRange(cur);
                    return;
                }
                var a0 = open[0];
                for (int i = 1; i < open.Count; i++)
                {

                    var next = new List<int>(open);
                    var b = next[i];
                    next.RemoveAt(i);
                    next.RemoveAt(0);
                    cur.Add((a0, b));
                    Search(next);
                    cur.RemoveAt(cur.Count - 1);
                }
            }

            if (pool.Count >= 2) Search(pool);
            return new Matching(bestPairs,
                bestRepeats == int.MaxValue ? 0 : bestRepeats,
                bestWithin3 == int.MaxValue ? 0 : bestWithin3);
        }

        private List<int> Rotate(List<int> seats)
        {
            var list = new List<int>(seats.Count);
            if (seats.Count == 0) return list;
            var start = (_battleSalt + _pvpIndex) % seats.Count;
            for (int i = 0; i < seats.Count; i++) list.Add(seats[(start + i) % seats.Count]);
            return list;
        }

        private int OpponentTier(int a, int b)
        {
            if (!_met.TryGetValue(a, out var met) || !met.Contains(b)) return 0;
            var gap = int.MaxValue;
            if (_lastMetPvpIndex.TryGetValue(a, out var byOpp) && byOpp.TryGetValue(b, out var at))
                gap = _pvpIndex - at;
            return gap > 3 ? 1 : 2;
        }

        private void MarkMet(int a, int b)
        {
            if (!_met.TryGetValue(a, out var ma)) _met[a] = ma = new HashSet<int>();
            if (!_met.TryGetValue(b, out var mb)) _met[b] = mb = new HashSet<int>();
            ma.Add(b);
            mb.Add(a);
            if (!_lastMetPvpIndex.TryGetValue(a, out var la)) _lastMetPvpIndex[a] = la = new Dictionary<int, int>();
            if (!_lastMetPvpIndex.TryGetValue(b, out var lb)) _lastMetPvpIndex[b] = lb = new Dictionary<int, int>();
            la[b] = _pvpIndex;
            lb[a] = _pvpIndex;
        }

        private PvpPair MakePair(int matchId, int a, int b)
        {
            bool aHuman = IsHuman(a), bHuman = IsHuman(b);
            if ((!aHuman && bHuman) || (aHuman == bHuman && a > b))
                (a, b) = (b, a);
            return new PvpPair(matchId, a, b);
        }

        private void RecordCorpse(in BattlePlayerDataMpo p)
        {
            var viewerId = p.ViewerId;
            if (_corpses.Exists(c => c.ViewerId == viewerId)) return;
            var units = MirrorHalf(p.FieldUnitList);
            _corpses.Add(new Corpse(viewerId, units, p.CalculatedStatus));
            Console.WriteLine($"[BATTLE] corpse frozen viewer={viewerId} units={units.Length} round={Round}");
        }

        private Corpse? CorpseOf(int viewerId)
        {
            foreach (var c in _corpses)
                if (c.ViewerId == viewerId) return c;
            return null;
        }

        private BattleRoundBeginDataMpo BuildRoundBeginData()
        {
            var npcId = BattleMaster.NpcIdForRound(Round);

            if (Rule == BattleRule.Boss && Boss != null) npcId = BattleMaster.BossRow.NpcId;
            var weakenNpc = false;
            var players = SnapshotPlayers();
            for (int i = 0; i < players.Length; i++)
            {
                var st = CurrentPlayerStatusSet(i);
                if (st != null) players[i].CalculatedStatus = st;
            }
            var matches = new List<BattleRoundMatchMpo>();
            if (npcId != 0)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    var p = players[i];
                    var (npcUnits, npcStatus) = BuildNpcUnits(npcId, i, weakenNpc);
                    if (Rule == BattleRule.Boss && Boss != null) ApplyBossPoolHp(npcUnits, npcStatus);
                    matches.Add(new BattleRoundMatchMpo
                    {
                        Id = i + 1,
                        PrimaryViewerId = p.ViewerId,
                        PrimaryUnits = p.FieldUnitList?.ToArray() ?? new BattleUnitDataMpo[0],
                        SecondaryViewerId = null,
                        SecondaryUnits = npcUnits,
                        ShadowViewerId = null,
                        PrimaryFieldUnitCalculatedStatus = p.CalculatedStatus ?? BattleMpoFactory.MakeStatusSet(new BattleUnitCalculatedStatusMpo[0]),
                        SecondaryFieldUnitCalculatedStatus = npcStatus,
                    });
                }
            }
            else
            {

                var pairs = BuildPvpPairs(Round);
                _pvpPairs = pairs;
                foreach (var pair in pairs)
                {
                    var primary = FindPlayer(pair.PrimaryViewerId);
                    var secondary = FindPlayer(pair.SecondaryViewerId);
                    if (primary.ViewerId == 0 || secondary.ViewerId == 0) continue;
                    var corpse = pair.Corpse ? CorpseOf(pair.SecondaryViewerId) : null;
                    matches.Add(new BattleRoundMatchMpo
                    {
                        Id = pair.MatchId,
                        PrimaryViewerId = primary.ViewerId,
                        PrimaryUnits = primary.FieldUnitList?.ToArray() ?? new BattleUnitDataMpo[0],
                        SecondaryViewerId = secondary.ViewerId,

                        SecondaryUnits = corpse?.MirroredUnits ?? MirrorHalf(secondary.FieldUnitList),
                        ShadowViewerId = null,
                        PrimaryFieldUnitCalculatedStatus = primary.CalculatedStatus
                            ?? BattleMpoFactory.MakeStatusSet(new BattleUnitCalculatedStatusMpo[0]),
                        SecondaryFieldUnitCalculatedStatus = (corpse?.Status ?? secondary.CalculatedStatus)
                            ?? BattleMpoFactory.MakeStatusSet(new BattleUnitCalculatedStatusMpo[0]),
                    });
                }
            }
            return new BattleRoundBeginDataMpo
            {
                Players = players,
                Matches = matches.ToArray(),
                Teams = Teams.ToArray(),
            };
        }

        public async Task RunRoundSetupSequenceAsync()
        {
            if (Rule == BattleRule.Boss)
            {
                await RunBossRoundSetupSequenceAsync();
                return;
            }
            try
            {
                Console.WriteLine($"[BATTLE] seq start battle={BattleId} round={Round}");
                var npcId = BattleMaster.NpcIdForRound(Round);
                Console.WriteLine($"[BATTLE] seq npcId={npcId}");

                await Delay(100);
                Phase = BattlePhase.GuildBuffRoulette;
                Broadcast(bc => bc.OnPhaseChanged(BattlePhase.GuildBuffRoulette, BattlePhase.Initialized));
                await Delay(200);

                Broadcast(bc => bc.OnStartGuildBuffRoulette(GuildBuffData.GuildId));
                await Delay(7000);
                Phase = BattlePhase.Transitioning;
                Broadcast(bc => bc.OnPhaseChanged(BattlePhase.Transitioning, BattlePhase.GuildBuffRoulette));
                await Delay(200);
                Broadcast(bc => bc.OnEndGuildBuffRoulette());

                await Delay(1000);
                Broadcast(bc => bc.OnNotifyInitialDistributeUnits(SnapshotPlayers(), BattleOption.DropBoost, new FieldCellDataMpo[0]));
                await Delay(3000);
                FinalizeInitialDistribution();
                Broadcast(bc => bc.OnInitialUnitsDistributeEnd(SnapshotPlayers()));

                await Delay(500);
                Phase = BattlePhase.SetupPlayerInfo;
                Broadcast(bc => bc.OnPhaseChanged(BattlePhase.SetupPlayerInfo, BattlePhase.Transitioning));
                await Delay(200);
                Broadcast(bc => bc.OnInformNextRound(Round, npcId));
                await Delay(200);
                var setupData1 = BuildSetupRoundResponse();
                BroadcastBoundary("OnSetupRound", $"round={Round}", bc => bc.OnSetupRound(setupData1));
                if (npcId != 0)
                {
                    await Delay(200);
                    var npcData1 = BuildNpcRoundStartResponses(npcId);
                    BroadcastBoundary("OnNpcRoundStart", $"npc={npcId}", bc => bc.OnNpcRoundStart(npcData1));
                }

                await RunPreparationPhaseAsync(npcId);
                Console.WriteLine($"[BATTLE] seq done phase={Phase} round={Round}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BATTLE] seq EX {ex}");
            }
        }

        private async Task RunBossRoundSetupSequenceAsync()
        {
            try
            {
                Console.WriteLine($"[BATTLE] boss-seq start battle={BattleId} round={Round}");
                var npcId = BattleMaster.BossRow.NpcId;
                await Delay(300);
                Phase = BattlePhase.SetupPlayerInfo;
                Broadcast(bc => bc.OnPhaseChanged(BattlePhase.SetupPlayerInfo, BattlePhase.Initialized));
                await Delay(200);
                Broadcast(bc => bc.OnInformNextRound(Round, npcId));
                await Delay(200);
                var setupData = BuildSetupRoundResponse();
                BroadcastBoundary("OnSetupRound", $"round={Round}", bc => bc.OnSetupRound(setupData));
                await Delay(200);
                BuildAndBroadcastBossRoundStart(npcId);
                await RunPreparationPhaseAsync(npcId);
                Console.WriteLine($"[BATTLE] boss-seq done phase={Phase} round={Round}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BATTLE] boss-seq EX {ex}");
            }
        }

        private void BuildAndBroadcastBossRoundStart(int npcId)
        {
            var (npcUnits, npcStatus) = BuildNpcUnits(npcId, 0, weakenNpc: false);
            if (Boss != null) ApplyBossPoolHp(npcUnits, npcStatus);
            var bossKey = npcUnits.Where(u => u.UnitId == BossBattleStore.BossUnitId)
                .Select(u => u.UniqueKey).FirstOrDefault();
            var zakoKeys = npcUnits.Where(u => u.UnitId != BossBattleStore.BossUnitId)
                .Select(u => u.UniqueKey).ToArray();
            var players = SnapshotPlayers();
            if (players.Length == 0) return;
            var resp = new BossRoundStartResponse
            {
                Player = players[0],
                NpcUnits = npcUnits,
                CalculatedStatus = npcStatus,
                BossUnitUniqueKey = bossKey,
                ZakoUnitUniqueKeys = zakoKeys,
            };
            BroadcastBoundary("OnBossRoundStart", $"npc={npcId} bossKey={bossKey} zako={zakoKeys.Length}", bc => bc.OnBossRoundStart(resp));
            Console.WriteLine($"[BATTLE] boss round start round={Round} npc={npcId} pool={Boss?.PoolHp}/{Boss?.MaxHp} bossKey={bossKey}");
        }

        private void ApplyBossPoolHp(BattleUnitDataMpo[] npcUnits, BattleUnitCalculatedStatusSetMpo status)
        {
            if (npcUnits == null || status?.UnitStatuses == null) return;
            var pool = Math.Max(1, Boss?.PoolHp ?? 0);
            var bossKeys = npcUnits.Where(u => u.UnitId == BossBattleStore.BossUnitId)
                .Select(u => u.UniqueKey).ToHashSet();
            if (bossKeys.Count == 0) return;
            foreach (var s in status.UnitStatuses)
            {
                if (s == null || !bossKeys.Contains(s.UniqueKey)) continue;
                if (s.Current != null) s.Current.Life = pool;
                if (s.Adjusted != null) s.Adjusted.Life = pool;
                if (s.Base != null) s.Base.Life = pool;
            }
            Console.WriteLine($"[BATTLE] boss pool hp override -> {pool}");
        }

        private void RecordBossOutcomeIfNeeded(int bossLeftLife)
        {
            if (Boss == null || Boss.Recorded || Players.Count == 0) return;
            Boss.Recorded = true;
            var viewerId = HumanViewerIds.FirstOrDefault();
            if (viewerId == 0) return;
            var damage = Math.Max(0, Boss.PoolHp - Math.Max(0, bossLeftLife));
            var rec = BossBattleStore.RecordResult(viewerId, BattleId.ToString(), Boss.DeckId, damage);
            Console.WriteLine($"[BATTLE] boss outcome viewer={viewerId} pool={Boss.PoolHp} left={bossLeftLife} dmg={damage} remain={rec.Remain} defeated={rec.Defeated}");
            ArchiveBossDeckLineup(viewerId);
        }

        private void ArchiveBossDeckLineup(int viewerId)
        {
            try
            {
                var p = FindPlayer(viewerId);
                var board = (p.FieldUnitList ?? new List<BattleUnitDataMpo>()).Where(u => u != null).ToList();
                if (board.Count == 0) return;
                var syn = BattleMaster.EvaluateFieldSynergies(board);
                BossBattleStore.UpdateDeckLineup(viewerId, Boss.DeckId,
                    board.Select(u => new BossBattleStore.DeckUnitRec
                    {
                        Key = u.UniqueKey,
                        UnitId = u.UnitId,
                        Rank = u.Rank,
                        X = u.PositionX,
                        Y = u.PositionY,
                        Equips = (u.EquipmentList ?? new List<BattleEquipmentDataMpo>())
                            .Where(e => e != null).Select(e => e.Id).ToArray(),
                    }).ToList(),
                    syn.Where(s => s != null && s.IsConfirmed && s.SynergyType == SynergyType.Role)
                        .Select(s => new BossBattleStore.SynRec { Id = s.Id, Count = s.CurrentFieldCount }).ToList(),
                    syn.Where(s => s != null && s.IsConfirmed && s.SynergyType == SynergyType.Team)
                        .Select(s => new BossBattleStore.SynRec { Id = s.Id, Count = s.CurrentFieldCount }).ToList());
            }
            catch (Exception ex)
            {

                Console.WriteLine($"[BATTLE] boss archiveDeck FAIL viewer={viewerId}: {ex.Message}");
            }
        }

        public async Task RunSubsequentRoundSetupSequenceAsync()
        {
            try
            {
                Console.WriteLine($"[BATTLE] next-seq start round={Round}");
                DeliverFriendRewards();

                ProcessDeferredMerges("setup");
                var npcId = BattleMaster.NpcIdForRound(Round);
                lock (_readyLock) ReadyPlayers.Clear();
                lock (_phaseLock)
                {
                    PreparationOpen = false;
                    RoundBeginSentEpoch = -1;
                }

                await Delay(200);
                Phase = BattlePhase.Transitioning;
                Broadcast(bc => bc.OnPhaseChanged(BattlePhase.Transitioning, BattlePhase.Result));
                await Delay(1000);
                Phase = BattlePhase.SetupPlayerInfo;
                Broadcast(bc => bc.OnPhaseChanged(BattlePhase.SetupPlayerInfo, BattlePhase.Transitioning));
                await Delay(200);
                Broadcast(bc => bc.OnInformNextRound(Round, npcId));
                await Delay(200);
                var setupData = BuildSetupRoundResponse();
                BroadcastBoundary("OnSetupRound", $"round={Round}", bc => bc.OnSetupRound(setupData));
                if (npcId != 0)
                {
                    await Delay(200);
                    var npcData = BuildNpcRoundStartResponses(npcId);
                    BroadcastBoundary("OnNpcRoundStart", $"npc={npcId}", bc => bc.OnNpcRoundStart(npcData));
                }

                await RunPreparationPhaseAsync(npcId);
                Console.WriteLine($"[BATTLE] next-seq done phase={Phase} round={Round}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BATTLE] next-seq EX {ex}");
            }
        }

        private async Task RunPreparationPhaseAsync(int npcId)
        {
            _ = npcId;
            var rule = BattleMaster.GetRule((int)Rule) ?? BattleMaster.GetRule(0);

            var prepareSeconds = BattleMaster.GetRound(Round)?.PrepareTime ?? 0;
            if (prepareSeconds <= 0) prepareSeconds = rule?.PrepareTime ?? 25;

            prepareSeconds = VerifyMode.ApplyPrepareSeconds(prepareSeconds);
            Console.WriteLine($"[BATTLE] prep-start round={Round} prepareSeconds={prepareSeconds} npcId={npcId} verifyMode={VerifyMode.IsActive}");

            await Delay(200);
            Phase = BattlePhase.Preparing;

            RefreshShopsForNewRound();

            AssignAiKinds(Round);

            Broadcast(bc => bc.OnPhaseChanged(BattlePhase.Preparing, BattlePhase.SetupPlayerInfo));

            await Delay(200);
            int epoch;
            lock (_phaseLock)
            {
                PreparationOpen = true;

                PreparationDeadlineUtc = DateTime.UtcNow.AddSeconds(Math.Max(10, prepareSeconds - 5));
                PreparationEpoch++;
                epoch = PreparationEpoch;
            }
            Broadcast(bc => bc.OnPreparetionCountStart(new RemainTimeResponse { RemainTime = prepareSeconds }));

            _ = RunAiPreparationActionsAsync();

            var drivenSeats = PlayerViewerIds.Where(vid => vid != 0 && IsDrivenByServer(vid)).ToArray();
            for (int count = 1; count <= 7; count++)
            {
                await Delay(count == 1 ? 1000 : 500);
                if (Phase != BattlePhase.Preparing)
                {
                    Console.WriteLine($"[BATTLE] prep ready-ladder aborted at count={count} phase={Phase}");
                    break;
                }
                lock (_readyLock)
                {
                    for (int i = 0; i < count && i < drivenSeats.Length; i++)
                    {
                        ReadyPlayers.Add(drivenSeats[i]);
                    }
                }
                BroadcastReadyList();
                CheckAllReady(epoch);
            }

            int readyElapsedMs = 1000 + 500 * 6;
            int remaining = Math.Max(0, prepareSeconds * 1000 - readyElapsedMs);

            var prepWaitDeadline = DateTime.UtcNow.AddMilliseconds(remaining);
            while (true)
            {
                var leftMs = (int)Math.Max(0, (prepWaitDeadline - DateTime.UtcNow).TotalMilliseconds);
                if (leftMs <= 0) break;
                if (Phase != BattlePhase.Preparing || !PreparationOpen) break;
                await Delay(Math.Min(leftMs, 10000));
                if (Phase != BattlePhase.Preparing || !PreparationOpen) break;
                var remainSec = Math.Max(1, (int)Math.Ceiling((prepWaitDeadline - DateTime.UtcNow).TotalSeconds));
                Broadcast(bc => bc.OnPreparetionCountStart(new RemainTimeResponse { RemainTime = remainSec }));
            }
            if (TryMarkPrepared())
            {
                Console.WriteLine($"[BATTLE] prep timeout boundary epoch={epoch}");
                Broadcast(bc => bc.OnPhaseChanged(BattlePhase.Prepared, BattlePhase.Preparing));
                await Delay(200);

                if (Rule != BattleRule.Boss) CheckFieldUnitCount();
                SendRoundBegin(epoch);
            }
        }

        private int _dummyKindId;
        internal void SetDummyKindId(int dummyKindId) => _dummyKindId = dummyKindId;

        private readonly Dictionary<int, BattleAiKindInfo> _aiKindByViewer = new Dictionary<int, BattleAiKindInfo>();

        private readonly ConcurrentDictionary<int, List<AiMirrorRound>> _aiMirrorTimeline = new();

        internal static readonly bool AiMirrorOn =
            Environment.GetEnvironmentVariable("BYAC_AI_MIRROR") != "0";

        internal void AssignAiKinds(int round)
        {
            var kinds = BattleMaster.AiKinds;
            if (kinds == null || kinds.Count == 0) return;
            int tier = round <= 3 ? 100 : round <= 6 ? 300 : 500;
            var rng = new Random(unchecked(HumanViewerId * 7919 + round * 0x6720 + _battleSalt * 7919));
            var pool = new List<BattleAiKindInfo>();
            foreach (var k in kinds) if (k.TargetPartyPower == tier) pool.Add(k);
            if (pool.Count == 0) { foreach (var k in kinds) pool.Add(k); }
            foreach (var vid in PlayerViewerIds)
            {

                if (!IsDrivenByServer(vid)) continue;
                var chosen = pool[rng.Next(pool.Count)];
                _aiKindByViewer[vid] = chosen;
                Console.WriteLine($"[BATTLE] AI kind assigned viewer={vid} kindId={chosen.Id} tier={chosen.TargetPartyPower} strat={chosen.StrategySetId} round={round}");
            }
        }

        private Task RunAiPreparationActionsAsync()
        {
            Console.WriteLine($"[BATTLE] AI prep start dummyKind={_dummyKindId} drivenCount={PlayerViewerIds.Count(IsDrivenByServer)}");
            var tasks = new List<Task>();
            foreach (var vid in PlayerViewerIds)
            {
                if (!IsDrivenByServer(vid)) continue;
                var idx = PlayerIndex(vid);
                if (idx < 0) continue;
                var p = Players[idx];
                if (!p.Alive || p.Life <= 0) continue;
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await RunAiPreparationForPlayerAsync(vid);
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine($"[BATTLE] AI prep EX viewer={vid}: {ex.GetType().Name}: {ex.Message}");
                    }
                }));
            }
            return Task.WhenAll(tasks);
        }

        internal void AssignAiMirrors()
        {
            if (!AiMirrorOn || VerifyMode.IsActive) return;
            var aiSeats = PlayerViewerIds.Where(vid => vid != 0 && !IsHuman(vid)).ToList();
            if (aiSeats.Count == 0) return;
            var donors = AiMirrorStore.PickDonors(aiSeats.Count);
            if (donors.Count == 0)
            {
                Console.WriteLine($"[BATTLE] ai-mirror assign battle={BattleId} donors=0 (pool empty) seats={aiSeats.Count} -> all pure ai");
                return;
            }
            var desc = new List<string>();
            for (int i = 0; i < aiSeats.Count && i < donors.Count; i++)
            {
                var (donorBattle, donorViewer) = donors[i];
                var timeline = AiMirrorStore.LoadTimeline(donorBattle, donorViewer);
                if (timeline.Count == 0) continue;
                _aiMirrorTimeline[aiSeats[i]] = timeline;
                desc.Add($"ai={aiSeats[i]}←{donorBattle[..Math.Min(8, donorBattle.Length)]}/v{donorViewer}({timeline.Count}r)");
            }
            if (desc.Count > 0)
                Console.WriteLine($"[BATTLE] ai-mirror assign battle={BattleId} mirrors={desc.Count}/{aiSeats.Count} [{string.Join(" ", desc)}]");
        }

        private bool TryApplyAiMirror(int viewerId)
        {
            if (!AiMirrorOn || IsHuman(viewerId)) return false;
            if (!_aiMirrorTimeline.TryGetValue(viewerId, out var timeline)) return false;
            var snap = timeline.FirstOrDefault(t => t.Round == Round);
            if (snap == null)
            {
                _aiMirrorTimeline.TryRemove(viewerId, out _);
                Console.WriteLine($"[BATTLE] ai-mirror takeover viewer={viewerId} round={Round} (donor timeline exhausted) -> pure ai from here");
                return false;
            }
            var idx = PlayerIndex(viewerId);
            if (idx < 0) return false;
            var p = Players[idx];
            if (!p.Alive || p.Life <= 0) return false;
            var units = new List<BattleUnitDataMpo>();
            for (int i = 0; i < snap.Units.Count; i++)
            {
                var s = snap.Units[i];
                var u = BattleMpoFactory.MakeUnit(_nextShopUnitKey++, s.UnitId, s.X, s.Y, UnitState.Field, i);
                u.Rank = Math.Max(1, s.Rank);
                if (s.Equipment is { Count: > 0 })
                {
                    foreach (var e in s.Equipment) e.UniqueKey = _nextEquipmentKey++;
                    u.EquipmentList = s.Equipment;
                }
                units.Add(u);
            }
            p.FieldUnitList = units;
            p.Level = Math.Max(1, snap.Level);
            p.Gold = Math.Max(0, snap.Gold);
            Players[idx] = p;

            Console.WriteLine($"[BATTLE] ai-mirror viewer={viewerId} round={Round} units={units.Count} donorLv={snap.Level} donorGold={snap.Gold}");
            return true;
        }

        private static BattleAiKindInfo SelectAiKind(int partyPower)
        {
            var kinds = BattleMaster.AiKinds;
            BattleAiKindInfo best = null;
            foreach (var k in kinds)
            {
                if (k.StrategySetId != 1) continue;
                if (k.TargetPartyPower < partyPower) continue;
                if (best == null || k.TargetPartyPower < best.TargetPartyPower) best = k;
            }
            if (best != null) return best;
            foreach (var k in kinds)
            {
                if (k.StrategySetId != 1) continue;
                if (best == null || k.TargetPartyPower > best.TargetPartyPower) best = k;
            }
            return best;
        }

        private static Task CommandPace(BattleAiKindInfo kind, Random rng)
        {
            if (kind == null || kind.MaxCommandDelay <= 0) return Task.CompletedTask;
            int min = Math.Max(0, kind.MinCommandDelay);
            int max = Math.Max(min, kind.MaxCommandDelay);
            return Task.Delay(rng.Next(min, max + 1));
        }

        private async Task RunAiPreparationForPlayerAsync(int viewerId)
        {

            var mirrored = TryApplyAiMirror(viewerId);
            var rng = GetShopRng(viewerId);
            var kindRng = new Random(unchecked(viewerId * 31 + Round * 7 + 0xA1 + _battleSalt * 7919));
            var p = FindPlayer(viewerId);

            p.FieldUnitList = p.FieldUnitList?.ToList();
            p.ShopUnitList = p.ShopUnitList?.ToList();
            p.DeckUnitList = p.DeckUnitList?.ToList();

            var kind = _aiKindByViewer.TryGetValue(viewerId, out var assigned) && assigned != null
                ? assigned
                : SelectAiKind(BattleMaster.AiPartyPower(p.FieldUnitList));
            if (mirrored)
            {
                await CommandPace(kind, kindRng);
                await RunAiEmoteBurstAsync(viewerId, kind, kindRng);
                return;
            }
            var fieldLimit = Math.Max(1, p.Level);
            var position = FindAiDeployCell(p);
            var bought = false;

            var missBuy = kind != null && kindRng.NextDouble() < kind.MissProbBuyUnit;
            var missDeploy = kind != null && kindRng.NextDouble() < kind.MissProbDeployUnit;
            if (!missBuy && !missDeploy && (p.FieldUnitList?.Count ?? 0) < fieldLimit && position.HasValue)
            {
                var shop = p.ShopUnitList ?? new List<BattleUnitDataMpo>();
                BattleUnitDataMpo chosen = default;
                var bestPrice = int.MaxValue;
                foreach (var u in shop)
                {
                    var price = UnitPrice(u);
                    if (price > p.Gold || price >= bestPrice) continue;
                    bestPrice = price;
                    chosen = u;
                }
                if (bestPrice < int.MaxValue)
                {
                    var res = BuyShopUnit(viewerId, chosen.UniqueKey, null,
                        position.Value.x, position.Value.y, p.FieldUnitList?.Count ?? 0);
                    if (res.Ok)
                    {
                        bought = true;
                        var snap = FindPlayer(viewerId);
                        if (res.DidRankUp)
                        {
                            Broadcast(bc => bc.OnBuyShopUnitToFieldAndRankUp(snap, res.RankUppedUnit));
                        }
                        else
                        {
                            Broadcast(bc => bc.OnBuyShopUnitToField(snap));
                        }
                        Console.WriteLine($"[BATTLE] AI buy viewer={viewerId} key={chosen.UniqueKey} unit={chosen.UnitId} pos=({position.Value.x},{position.Value.y}) gold={snap.Gold} rankUp={res.DidRankUp} kind={(kind?.Id ?? 0)}");
                    }
                    else
                    {
                        Console.WriteLine($"[BATTLE] AI buy reject viewer={viewerId} key={chosen.UniqueKey}: {res.Reason}");
                    }
                }
            }
            await CommandPace(kind, kindRng);

            p = FindPlayer(viewerId);
            var need = BattleMaster.ExpToNext(p.Level);
            var missLevel = kind != null && kindRng.NextDouble() < kind.MissProbLevelup;
            if (!missLevel && need >= 0 && p.Gold >= need)
            {
                var res = BuyExpOp(viewerId);
                if (res.Ok)
                {
                    var snap = FindPlayer(viewerId);
                    Broadcast(bc => bc.OnBuyExp(snap));
                    Console.WriteLine($"[BATTLE] AI buyexp viewer={viewerId} level={snap.Level} gold={snap.Gold}");
                }
            }

            else if (!bought)
            {
                var refreshPrice = (BattleMaster.GetRule((int)Rule) ?? BattleMaster.GetRule(0))?.RefreshShopPrice ?? 2;
                p = FindPlayer(viewerId);
                var missRefresh = kind != null && kindRng.NextDouble() < kind.MissProbRefreshShop;
                if (!missRefresh && p.Gold >= refreshPrice && rng.NextDouble() < 0.25)
                {
                    var res = RefreshShop(viewerId);
                    if (res.Ok)
                    {
                        var snap = FindPlayer(viewerId);

                        TryUnicastToViewer(viewerId, bc => bc.OnRefreshShopUnit(snap, null));
                        Console.WriteLine($"[BATTLE] AI refresh viewer={viewerId} gold={snap.Gold} slots={snap.ShopUnitList?.Count ?? 0}");
                    }
                }
            }
            await CommandPace(kind, kindRng);

            await RunAiEmoteBurstAsync(viewerId, kind, kindRng);
        }

        private async Task RunAiEmoteBurstAsync(int viewerId, BattleAiKindInfo? kind, Random kindRng)
        {
            if (kind != null && kind.EmoteTimesMin > 0 && kindRng.NextDouble() < kind.EmoteProb)
            {
                int timesMin = Math.Max(1, kind.EmoteTimesMin);
                int timesMax = Math.Max(timesMin, kind.EmoteTimesMax);
                int times = kindRng.Next(timesMin, timesMax + 1);
                int delayMin = Math.Max(0, kind.EmoteDelaySecMin);
                int delayMax = Math.Max(delayMin, kind.EmoteDelaySecMax);
                for (int t = 0; t < times; t++)
                {
                    var waitMs = kindRng.Next(delayMin, delayMax + 1) * 1000;
                    if (waitMs > 0) await Task.Delay(waitMs);
                    if (Phase != BattlePhase.Preparing) break;
                    var emoteId = kindRng.Next(1, 7);
                    SendEmoteOp(viewerId, emoteId);
                }
                Console.WriteLine($"[BATTLE] AI emote viewer={viewerId} kind={kind.Id} times={times} prob={kind.EmoteProb}");
            }
        }

        private static (int x, int y)? FindAiDeployCell(in BattlePlayerDataMpo p)
        {
            for (int y = 4; y <= 7; y++)
            {
                for (int x = 0; x <= 5; x++)
                {
                    if (ValidateFieldDestination(p, x, y) == null) return (x, y);
                }
            }
            return null;
        }

        private int FieldPower(in BattlePlayerDataMpo p)
        {
            int total = 0;
            foreach (var u in p.FieldUnitList ?? new List<BattleUnitDataMpo>())
            {
                var info = BattleMaster.GetUnit(u.UnitId);
                if (info == null) continue;
                total += info.Life + info.Atk + info.MagicAtk + info.Defence + info.MagicDefence;
            }
            return total;
        }

        private int NpcPower(int npcId)
        {
            int total = 0;
            foreach (var (unitId, _, _) in BattleMaster.NpcLayout(npcId))
            {
                var info = BattleMaster.GetUnit(unitId);
                if (info == null) continue;
                total += info.Life + info.Atk + info.MagicAtk + info.Defence + info.MagicDefence;
            }
            return total;
        }

        private static int ComputeLoseDamage(int round, int aliveEnemyUnits)
        {
            var rd = BattleMaster.GetRound(round);
            int baseDmg = rd?.BaseDamage ?? 0;
            int unitDmg = rd?.UnitDamage ?? 0;
            int raw = Math.Max(0, baseDmg + unitDmg * Math.Max(0, aliveEnemyUnits));
            double mult = TestDamageMultiplier;
            return Math.Max(0, (int)Math.Round(raw * mult));
        }

        private static double TestDamageMultiplier
        {
            get
            {
                var raw = Environment.GetEnvironmentVariable("BYAC_TEST_DAMAGE_MULTIPLIER");
                return double.TryParse(raw, out var v) && v > 0 ? v : 1.0;
            }
        }

        private void CheckFieldUnitCount()
        {
            var changed = new List<BattlePlayerDataMpo>();
            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i];
                if (!p.Alive || p.Life <= 0) continue;
                var cap = Math.Min(9, Math.Max(1, p.Level));

                var field = (p.FieldUnitList ?? new List<BattleUnitDataMpo>()).ToList();
                var deck = (p.DeckUnitList ?? new List<BattleUnitDataMpo>()).ToList();
                var mutated = false;

                {
                    var seen = new HashSet<(int, int)>();
                    var relocated = new List<(BattleUnitDataMpo u, int ox, int oy)>();
                    foreach (var u in field)
                    {
                        var cell = (u.PositionX, u.PositionY);
                        if (seen.Add(cell)) continue;
                        relocated.Add((u, u.PositionX, u.PositionY));
                    }
                    foreach (var (u, ox, oy) in relocated)
                    {
                        if (deck.Count >= DeckCapacity) continue;
                        field.Remove(u);
                        u.State = UnitState.Deck;
                        u.PositionX = 0;
                        u.PositionY = 0;
                        u.Index = deck.Count;
                        deck.Add(u);
                        _deploySeqs.Remove(u.UniqueKey);
                        Console.WriteLine($"[BATTLE] overlap-fix viewer={p.ViewerId} unit={u.UniqueKey} cell=({ox},{oy})->deck");
                    }
                    if (relocated.Count > 0)
                    {
                        ReindexField(ref p);
                        ReindexDeck(ref p);
                        mutated = true;
                    }
                }

                if (field.Count > cap)
                {
                    var extra = field
                        .OrderByDescending(u => _deploySeqs.TryGetValue(u.UniqueKey, out var q) ? q : 0)
                        .Take(field.Count - cap)
                        .ToList();
                    foreach (var u in extra)
                    {
                        field.Remove(u);
                        if (deck.Count < DeckCapacity)
                        {
                            u.State = UnitState.Deck;
                            u.PositionX = 0;
                            u.PositionY = 0;
                            u.Index = deck.Count;
                            deck.Add(u);

                            Console.WriteLine($"[BATTLE] over-cap bounce viewer={p.ViewerId} round={Round} unit={u.UniqueKey} ->deck");
                        }
                        else
                        {
                            var price = BattleMaster.SellUnitPrice(u.Rank, UnitTier(u));
                            p.Gold += price;
                            if (u.EquipmentList != null && u.EquipmentList.Count > 0)
                                p.EquipmentList = (p.EquipmentList ?? new List<BattleEquipmentDataMpo>())
                                    .Concat(u.EquipmentList).ToList();
                            _deploySeqs.Remove(u.UniqueKey);
                            Console.WriteLine("[BATTLE] over-cap sell viewer={0} unit={1} gold+={2}", p.ViewerId, u.UnitId, price);
                        }
                    }
                    ReindexDeck(ref p);
                    ReindexField(ref p);
                    mutated = true;
                }

                while (field.Count < cap && deck.Count > 0 && FindFreeOwnTile(field, out var x, out var y))
                {
                    var di = Random.Shared.Next(deck.Count);
                    var u = deck[di];
                    deck.RemoveAt(di);
                    u.State = UnitState.Field;
                    u.PositionX = x;
                    u.PositionY = y;
                    u.Index = field.Count;
                    u.IsPurchased = true;
                    field.Add(u);
                    mutated = true;
                }
                if (mutated)
                {
                    ReindexField(ref p);
                    p.FieldUnitList = field;
                    p.DeckUnitList = deck;
                    Players[i] = p;

                    RefreshSynergies(i);
                    changed.Add(Players[i]);
                }
            }
            if (changed.Count > 0)
            {
                Broadcast(bc => bc.OnCheckFieldUnitCount(changed.ToArray()));
                Console.WriteLine("[BATTLE] field auto-fill players={0}", changed.Count);
            }
        }

        private static bool FindFreeOwnTile(List<BattleUnitDataMpo> field, out int x, out int y)
        {
            int[] cols = { 2, 3, 1, 4, 0, 5 };
            for (var row = 4; row <= 7; row++)
            {
                foreach (var cx in cols)
                {
                    var occupied = false;
                    foreach (var u in field)
                    {
                        if (u.PositionX == cx && u.PositionY == row) { occupied = true; break; }
                    }
                    if (!occupied) { x = cx; y = row; return true; }
                }
            }
            x = 0; y = 0;
            return false;
        }

        public async Task RunSimpleBattleAsync()
        {
            try
            {
                var round = Round;

                BattleRunning = true;

                var npcId = BattleMaster.NpcIdForRound(round);

                if (Rule == BattleRule.Boss && Boss != null) npcId = BattleMaster.BossRow.NpcId;
                if (Rule != BattleRule.Boss) CheckFieldUnitCount();

                for (int pi = 0; pi < Players.Count; pi++)
                {
                    var pm = Players[pi];
                    pm.UnitMirroringPairs = BattleMaster.ApplyMirroring(pm.FieldUnitList);
                    Players[pi] = pm;
                }
                var axis = new List<(float at, BattleBoardEventResponse ev)>();
                var matchOutcomes = new Dictionary<int, (bool Win, int Damage, int Kills)>();
                var matchInfo = new Dictionary<int, (int SecondaryLife, bool TimedOut)>();

                _roundDropPlans.Clear();
                _roundDropsEnqueued = 0;

                _ownMatchByViewer = new Dictionary<int, int>();
                _ownMatchByViewerRound = Round;
                Console.WriteLine($"[BATTLE] simple-battle npcId={npcId} players={Players.Count}");
                if (npcId != 0 && Players.Count > 0)
                {

                    var npcPow = NpcPower(npcId);
                    Console.WriteLine($"[BATTLE] PVE round={round} npcId={npcId} npcPower={npcPow} players={Players.Count}");
                    for (int mi = 0; mi < Players.Count; mi++)
                    {
                        var p = Players[mi];

                        if (p.Life <= 0 || !p.Alive)
                        {
                            Console.WriteLine($"[BATTLE] PVE skip round={round} viewer={p.ViewerId} (eliminated, no match)");
                            continue;
                        }
                        var units = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
                        var (npcUnits, npcStatus) = BuildNpcUnits(npcId, mi, weakenNpc: false);
                        if (Rule == BattleRule.Boss && Boss != null) ApplyBossPoolHp(npcUnits, npcStatus);
                        var playerStatus = CurrentPlayerStatusSet(mi) ?? p.CalculatedStatus;
                        var playerSyn = BattleMaster.EvaluateActiveSynergyEffects(units);
                        var sim = SimpleTickSimulator.Simulate(mi + 1, units, playerStatus, npcUnits, npcStatus, _guildBuffEffectsByUnitId,
                            primarySynergies: playerSyn,
                            rngSalt: SimRngSalt(round));
                        axis.AddRange(sim.Events);
                        LogBoardSnapshot(round, mi + 1, "p", p.ViewerId, SimRngSalt(round), units, playerStatus, playerSyn, _guildBuffEffectsByUnitId);
                        LogBoardSnapshot(round, mi + 1, "n", npcId, SimRngSalt(round), npcUnits, npcStatus, null, null);
                        LogUnitStats(round, mi + 1, "p", units, sim.Events);
                        LogUnitStats(round, mi + 1, "n", npcUnits, sim.Events);
                        PersistChargeSeals(units, sim);

                        bool playerWins = sim.IsDraw ? FieldPower(p) >= npcPow : sim.PrimaryWin;

                        int damage = Rule == BattleRule.Boss
                            ? 0
                            : playerWins ? 0 : ComputeLoseDamage(round, sim.SecondaryAlive);

                        int kills = Math.Max(0, npcUnits.Length - sim.SecondaryAlive);
                        matchOutcomes[p.ViewerId] = (playerWins, damage, kills);
                        matchInfo[p.ViewerId] = (sim.SecondaryRemainingLife, sim.TimedOut);
                        _ownMatchByViewer[p.ViewerId] = mi + 1;

                        if (playerWins && p.Life > 0 && Rule != BattleRule.Boss)
                        {
                            PlanRoundDrops(p, sim.Events, npcUnits, mi + 1, round, axis);
                        }

                        if (Rule == BattleRule.Boss && Boss != null && !Boss.Recorded)
                        {
                            var bossKey = npcUnits.Where(u => u.UnitId == BossBattleStore.BossUnitId)
                                .Select(u => u.UniqueKey).FirstOrDefault();
                            _bossLeftLife = bossKey != 0 && sim.FinalLives.TryGetValue(bossKey, out var bl)
                                ? bl
                                : sim.SecondaryRemainingLife;

                            if (bossKey == 0)
                                Console.WriteLine($"!! [BATTLE] boss unit {BossBattleStore.BossUnitId} missing from npc board" +
                                    $" round={round} viewer={p.ViewerId} battle={BattleId} fallbackLeft={_bossLeftLife}" +
                                    $" npcUnits={npcUnits.Length} -- full HP pool counted; check master boss_battle_data/unit_data");
                        }
                        Console.WriteLine($"[BATTLE] PVE match round={round} viewer={p.ViewerId} power={FieldPower(p)} vs npc={npcPow} win={playerWins} draw={sim.IsDraw} timedOut={sim.TimedOut} damage={damage} simEvents={sim.Events.Count} aliveP={sim.PrimaryAlive} aliveS={sim.SecondaryAlive}{(Rule == BattleRule.Boss ? $" bossLeft={_bossLeftLife}" : "")}");
                    }
                }
                else if (Players.Count > 0)
                {

                    var pairs = _pvpPairs.Length > 0 ? _pvpPairs : BuildPvpPairs(round);
                    _pvpPairs = pairs;
                    foreach (var pair in pairs)
                    {
                        var primary = FindPlayer(pair.PrimaryViewerId);
                        var secondary = FindPlayer(pair.SecondaryViewerId);
                        if (primary.ViewerId == 0 || secondary.ViewerId == 0) continue;
                        _ownMatchByViewer[primary.ViewerId] = pair.MatchId;
                        _ownMatchByViewer[secondary.ViewerId] = pair.MatchId;
                        var corpse = pair.Corpse ? CorpseOf(pair.SecondaryViewerId) : null;
                        var pUnits = primary.FieldUnitList ?? new List<BattleUnitDataMpo>();

                        var sUnits = corpse?.MirroredUnits ?? MirrorHalf(secondary.FieldUnitList);
                        var pSyn = BattleMaster.EvaluateActiveSynergyEffects(pUnits);
                        var sSyn = BattleMaster.EvaluateActiveSynergyEffects(sUnits);
                        var sStatus = corpse?.Status ?? secondary.CalculatedStatus;
                        var sim = SimpleTickSimulator.Simulate(pair.MatchId, pUnits, primary.CalculatedStatus, sUnits,
                            sStatus, _guildBuffEffectsByUnitId,
                            primarySynergies: pSyn,
                            secondarySynergies: sSyn,
                            rngSalt: SimRngSalt(round));
                        axis.AddRange(sim.Events);
                        LogBoardSnapshot(round, pair.MatchId, "p", primary.ViewerId, SimRngSalt(round), pUnits, primary.CalculatedStatus, pSyn, _guildBuffEffectsByUnitId);
                        LogBoardSnapshot(round, pair.MatchId, "s", pair.SecondaryViewerId, SimRngSalt(round), sUnits, sStatus, sSyn, _guildBuffEffectsByUnitId);
                        LogUnitStats(round, pair.MatchId, "p", pUnits, sim.Events);
                        LogUnitStats(round, pair.MatchId, "s", sUnits, sim.Events);

                        PersistChargeSeals(pUnits, sim);
                        if (corpse == null) PersistChargeSeals(secondary.FieldUnitList, sim);
                        bool primaryWins = sim.IsDraw
                            ? FieldPower(primary) >= FieldPower(secondary)
                            : sim.PrimaryWin;
                        int winnerAlive = primaryWins ? sim.PrimaryAlive : sim.SecondaryAlive;
                        int loserDamage = ComputeLoseDamage(round, winnerAlive);

                        int primaryKills = Math.Max(0, sUnits.Length - sim.SecondaryAlive);
                        int secondaryKills = Math.Max(0, pUnits.Count - sim.PrimaryAlive);

                        matchOutcomes[primary.ViewerId] = (primaryWins, primaryWins ? 0 : loserDamage, primaryKills);
                        if (corpse == null)
                            matchOutcomes[secondary.ViewerId] = (!primaryWins, primaryWins ? loserDamage : 0, secondaryKills);

                        int primarySecondaryLife = Math.Max(0, secondary.Life - (primaryWins ? loserDamage : 0));
                        int secondarySecondaryLife = Math.Max(0, primary.Life - (primaryWins ? 0 : loserDamage));
                        matchInfo[primary.ViewerId] = (primarySecondaryLife, sim.TimedOut);
                        if (corpse == null) matchInfo[secondary.ViewerId] = (secondarySecondaryLife, sim.TimedOut);
                        Console.WriteLine($"[BATTLE] PVP match round={round} m={pair.MatchId} p={primary.ViewerId}({FieldPower(primary)}) s={secondary.ViewerId}({FieldPower(secondary)}) corpse={corpse != null} win={(primaryWins ? primary.ViewerId : secondary.ViewerId)} timedOut={sim.TimedOut} loserDamage={loserDamage} simEvents={sim.Events.Count}");
                    }
                }

                for (int ri = 0; ri < Players.Count; ri++) RefreshSynergies(ri);

                SimpleTickSimulator.DiscretizeAxis(axis);
                SimpleTickSimulator.EnforceFrameRevisions(axis);
                float lastCombatAt = axis.Count > 0 ? axis.Max(e => e.at) : 0f;
                float completeBase = Math.Max(9.5f, lastCombatAt + 0.1f);
                if (npcId == 0 && _pvpPairs.Length > 0)
                {

                    var pairs = _pvpPairs;
                    for (int mi = 0; mi < pairs.Length; mi++)
                    {
                        var pair = pairs[mi];
                        var primary = FindPlayer(pair.PrimaryViewerId);
                        var secondary = FindPlayer(pair.SecondaryViewerId);
                        if (primary.ViewerId == 0 || secondary.ViewerId == 0) continue;
                        var pOut = matchOutcomes.TryGetValue(primary.ViewerId, out var po)
                            ? po : (Win: true, Damage: 0, Kills: 0);
                        var sOut = matchOutcomes.TryGetValue(secondary.ViewerId, out var so)
                            ? so : (Win: false, Damage: 0, Kills: 0);
                        var timedOut = matchInfo.TryGetValue(primary.ViewerId, out var mInfo) && mInfo.TimedOut;
                        float at = completeBase + mi * 0.1f;
                        axis.Add((at, new BattleBoardCompleteEventResponse
                        {
                            MatchId = pair.MatchId,
                            RaisedAt = at,
                            LastMatchInRound = mi == pairs.Length - 1,
                            PrimaryResult = pOut.Win ? BattleResultType.Win : BattleResultType.Lose,
                            PrimaryDamage = pOut.Damage,
                            PrimaryLife = Math.Max(0, primary.Life - pOut.Damage),
                            SecondaryResult = sOut.Win ? BattleResultType.Win : BattleResultType.Lose,
                            SecondaryDamage = sOut.Damage,
                            SecondaryLife = Math.Max(0, secondary.Life - sOut.Damage),
                            TimedOut = timedOut,
                        }));
                        Console.WriteLine($"[BATTLE] Complete(PVP) round={round} match={pair.MatchId} p={primary.ViewerId} pWin={pOut.Win} pDmg={pOut.Damage} pLife={Math.Max(0, primary.Life - pOut.Damage)} s={secondary.ViewerId} sWin={sOut.Win} sDmg={sOut.Damage} sLife={Math.Max(0, secondary.Life - sOut.Damage)} timedOut={timedOut}");
                    }
                }
                else
                {

                    for (int i = 0; i < Players.Count; i++)
                    {
                        var p = Players[i];

                        var outcome = matchOutcomes.TryGetValue(p.ViewerId, out var o)
                            ? o
                            : (p.Life > 0 && p.Alive
                                ? (Win: true, Damage: 0, Kills: 0)
                                : (Win: false, Damage: 0, Kills: 0));
                        var info = matchInfo.TryGetValue(p.ViewerId, out var m)
                            ? m : (SecondaryLife: 0, TimedOut: false);
                        var primaryWin = outcome.Win;
                        var damage = outcome.Damage;
                        var life = Math.Max(0, p.Life - damage);
                        if (damage > 0)
                        {
                            Console.WriteLine($"[BATTLE] Complete round={round} viewer={p.ViewerId} win={primaryWin} damage={damage} life={life} secondaryLife={info.SecondaryLife} timedOut={info.TimedOut}");
                        }
                        axis.Add((completeBase + i * 0.1f, new BattleBoardCompleteEventResponse
                        {
                            MatchId = i + 1,
                            RaisedAt = completeBase + i * 0.1f,
                            LastMatchInRound = i == Players.Count - 1,
                            PrimaryResult = primaryWin ? BattleResultType.Win : BattleResultType.Lose,
                            PrimaryDamage = damage,
                            PrimaryLife = life,
                            SecondaryResult = primaryWin ? BattleResultType.Lose : BattleResultType.Win,
                            SecondaryDamage = 0,
                            SecondaryLife = info.SecondaryLife,
                            TimedOut = info.TimedOut,
                        }));
                    }
                }

                Phase = BattlePhase.Running;
                await Delay(100);
                Broadcast(bc => bc.OnPhaseChanged(BattlePhase.Running, BattlePhase.Prepared));
                await Delay(100);
                Broadcast(bc => bc.OnPreparetionCountStart(new RemainTimeResponse { RemainTime = BattleSeconds }));

                const float lead = 0.5f;
                long sendStart = System.Diagnostics.Stopwatch.GetTimestamp();
                float RealElapsed() => (float)(System.Diagnostics.Stopwatch.GetTimestamp() - sendStart) / System.Diagnostics.Stopwatch.Frequency;
                float prevSendAt = -1f;
                bool wasLagging = false;

                int frameCount = 0;
                long frameEvents = 0;
                float lastFrameAt = -1f;

                var sentByViewer = new Dictionary<int, int>();
                var failedByViewer = new Dictionary<int, int>();
                int groupBatches = 0;
                int[] viewersAtStart;
                lock (_identityLock) viewersAtStart = _ownChannels.Keys.ToArray();
                lock (_liveAxisLock) { _liveAxis = axis; _liveAxisSent = 0; _liveAxisRound = Round; }
                foreach (var group in axis.GroupBy(e => (int)MathF.Round(e.at / SimpleTickSimulator.MinFrameGapSeconds)).OrderBy(g => g.Key))
                {
                    float bucketAt = group.Key * SimpleTickSimulator.MinFrameGapSeconds;
                    float target = Math.Max(0f, bucketAt - lead);
                    float elapsed = RealElapsed();
                    if (target > elapsed && prevSendAt >= 0f && target < prevSendAt + SimpleTickSimulator.MinFrameGapSeconds)
                    {
                        target = prevSendAt + SimpleTickSimulator.MinFrameGapSeconds;
                    }
                    int waitMs = (int)((target - elapsed) * 1000);
                    if (waitMs > 0)
                    {
                        await Delay(waitMs);
                    }
                    prevSendAt = RealElapsed();
                    var frame = group.Select(e => e.ev).ToArray();

                    if (FrameLogOn)
                    {
                        var ubDmgs = frame.OfType<BattleBoardSkillDamageEventResponse>().ToArray();
                        if (ubDmgs.Length > 0)
                            Console.WriteLine($"[BATTLE] UB damage round={round} events=[{string.Join(" ", ubDmgs.Select(d => $"m{d.MatchId}:k{d.OperatorUniqueKey}->t{d.TargetUniqueKey}:dmg{d.Damage}:sk{d.SkillId}:dead{d.IsDead}"))}]");
                    }

                    foreach (var (subFrame, _) in SplitFrameByBytes(frame))
                    {
                        if (BroadcastBoardFrames(subFrame, sentByViewer, failedByViewer)) groupBatches++;
                        ServerMetrics.BattleFrameSent(subFrame.Length, prevSendAt - target);
                    }

                    float lag = prevSendAt - target;
                    Net.NoteDrift("room", lag * 1000.0);
                    if (lag > 1f && !wasLagging)
                        Console.WriteLine($"[BATTLE] send-lag round={round} at={bucketAt:F2} lag={lag:F2}s -> catch-up");
                    else if (lag <= 0.05f && wasLagging)
                        Console.WriteLine($"[BATTLE] send-lag recovered round={round} at={bucketAt:F2}");
                    wasLagging = lag > 1f;
                    lock (_liveAxisLock) _liveAxisSent += group.Count();
                    frameCount++;
                    frameEvents += frame.Length;
                    lastFrameAt = bucketAt;
                    if (FrameLogOn)
                        Console.WriteLine($"[BATTLE] frame battle={BattleId} round={round} at={bucketAt:F2} events={frame.Length} sent(elapsed={prevSendAt:F1}s)");
                }
                lock (_liveAxisLock) { _liveAxisSent = axis.Count; _liveAxis = null; }

                Console.WriteLine($"[BATTLE] round-frames battle={BattleId} round={round} frames={frameCount} events={frameEvents} lastAt={lastFrameAt:F2} elapsed={prevSendAt:F1}s");

                {
                    var keys = viewersAtStart.Union(sentByViewer.Keys).Union(failedByViewer.Keys).Distinct().OrderBy(v => v).ToArray();
                    var sentDesc = string.Join(" ", keys.Select(v => $"v{v}:{(sentByViewer.TryGetValue(v, out int c) ? c : 0)}"));
                    var failDesc = failedByViewer.Count == 0 ? "" :
                        $" failed=[{string.Join(" ", failedByViewer.OrderBy(kv => kv.Key).Select(kv => $"v{kv.Key}:{kv.Value}"))}]";
                    Console.WriteLine($"[BATTLE] frame-delivery round={round} batches={frameCount} viewers=[{sentDesc}]{failDesc}{(groupBatches > 0 ? $" group={groupBatches}" : "")}");
                }
                Net.DumpRound(round);

                bool willDropNotify = npcId != 0 && _roundDropsEnqueued > 0;
                {
                    float lastAt = axis.Count > 0 ? axis.Max(e => e.at) : 0f;
                    float graceBefore = willDropNotify ? DropItemsLeadSeconds : ResultAnimationGraceSeconds;
                    int tailMs = (int)(((lastAt - lead) + graceBefore - RealElapsed()) * 1000);
                    if (tailMs > 0) await Delay(tailMs);
                }

                if (Finished)
                {
                    BattleRunning = false;
                    Console.WriteLine($"[BATTLE] simple-battle aborted at result boundary round={round} (finished/abandoned)");
                    return;
                }
                if (Rule == BattleRule.Boss && Boss != null)
                {

                    RecordBossOutcomeIfNeeded(_bossLeftLife);
                }
                else
                {
                    CommitRoundOutcome(npcId, matchOutcomes);
                }

                if (willDropNotify)
                {
                    foreach (var vid in LiveAttachedHumans())
                    {
                        var dropHuman = FindPlayer(vid);
                        var snapshot = dropHuman;
                        BroadcastToViewer(vid, bc => bc.OnReceiveDropitems(new[] { snapshot }));
                        Console.WriteLine($"[BATTLE] drop notify round={round} drops={_roundDropsEnqueued} viewer={vid} humanEquip={snapshot.EquipmentList?.Count ?? 0} at={DateTime.Now:HH:mm:ss.fff}");
                    }

                    await Delay((int)(ResultAnimationGraceSeconds * 1000));
                }
                BattleRunning = false;
                ProcessDeferredMerges("result");
                Phase = BattlePhase.Result;
                Broadcast(bc => bc.OnPhaseChanged(BattlePhase.Result, BattlePhase.Running));
                await Delay(200);
                var roundResults = BuildRoundResults(npcId, matchOutcomes);
                BroadcastBoundary("OnRoundResult", $"matches={matchOutcomes.Count}", bc => bc.OnRoundResult(roundResults));

                await Delay(50);
                foreach (var p in Players)
                {
                    var snapshot = p;
                    TryUnicastToViewer(p.ViewerId, bc => bc.OnRefreshShopUnit(snapshot, null));
                }
                Console.WriteLine($"[BATTLE] simple-battle done round={round} phase={Phase}");

                var ackEpoch = ++_roundResultAckEpoch;

                var waitingHumans = LiveAttachedHumans().Length;
                var ackTimeoutSec = (waitingHumans > 0 ? RoundResultAckTimeoutSeconds : EliminatedAckTimeoutSeconds) + DrainGraceSeconds;
                _ = Task.Run(async () =>
                {
                    await Delay(ackTimeoutSec * 1000);
                    if (Finished || BattleCompleteEmitted) return;
                    if (_roundResultAckEpoch != ackEpoch) return;
                    int[] exp;
                    lock (SignalLock) exp = ExpectedSignal;
                    if (exp != null && exp[0] == round && (exp[1] == 2 || exp[1] == 3))
                    {
                        Console.WriteLine($"[BATTLE] watchdog: (round={round},RoundResultCompleted) not received in {ackTimeoutSec}s (incl. drain grace {DrainGraceSeconds}s) -> force advance");
                        HandleRoundResultCompleted();
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BATTLE] simple-battle EX {ex}");
            }
        }

        private void CommitRoundOutcome(int npcId, Dictionary<int, (bool Win, int Damage, int Kills)> matchOutcomes)
        {
            var roundInfo = BattleMaster.GetRound(Round);
            var distributeGold = roundInfo?.DistributeGold ?? 1;
            var isNpcRound = npcId != 0;
            _lastRoundSettle.Clear();
            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i];
                var (win, damage, _) = matchOutcomes.TryGetValue(p.ViewerId, out var v) ? v : (true, 0, 0);

                if (p.Life <= 0 || !p.Alive)
                {
                    _lastRoundSettle[p.ViewerId] = (0, 0, 0, 0, 0);
                    p.IsShopPurchasable = false;
                    Players[i] = p;
                    Console.WriteLine($"[BATTLE] settle skip round={Round} viewer={p.ViewerId} (eliminated)");
                    continue;
                }

                _winStreaks.TryGetValue(p.ViewerId, out var winStreak);
                _loseStreaks.TryGetValue(p.ViewerId, out var loseStreak);
                int baseGold = win ? (roundInfo?.WinGold ?? 0) : (roundInfo?.LoseGold ?? 0);
                int bonusGold = win
                    ? BattleMaster.WinBonus(Math.Max(0, winStreak + (isNpcRound ? 0 : 1)))
                    : BattleMaster.LoseBonus(Math.Max(0, loseStreak + (isNpcRound ? 0 : 1)));
                int resultRewardGold = baseGold + bonusGold;
                int interestGold = BattleMaster.FindInterest(p.Gold);
                _lastRoundSettle[p.ViewerId] = (baseGold, distributeGold, bonusGold, resultRewardGold, interestGold);

                if (win) p.TotalWinsCount++;
                else p.TotalLoseCount++;

                if (!isNpcRound)
                {
                    if (win)
                    {
                        _winStreaks[p.ViewerId] = winStreak + 1;
                        _loseStreaks[p.ViewerId] = 0;
                    }
                    else
                    {
                        _loseStreaks[p.ViewerId] = loseStreak + 1;
                        _winStreaks[p.ViewerId] = 0;
                    }
                }

                p.WinsCount = _winStreaks.TryGetValue(p.ViewerId, out var wireWinStreak) ? wireWinStreak : 0;
                p.LosesCount = _loseStreaks.TryGetValue(p.ViewerId, out var wireLoseStreak) ? wireLoseStreak : 0;
                var wasAlive = p.Life > 0 && p.Alive;
                if (damage > 0)
                {
                    p.Life = Math.Max(0, p.Life - damage);
                    if (p.Life <= 0) p.Alive = false;
                }

                if (wasAlive && !p.Alive && !_placements.ContainsKey(p.ViewerId))
                {
                    var aliveOthers = 0;
                    foreach (var q in Players)
                        if (q.ViewerId != p.ViewerId && q.Alive) aliveOthers++;
                    _placements[p.ViewerId] = aliveOthers + 1;

                    if (!_eliminatedRound.ContainsKey(p.ViewerId))
                        _eliminatedRound[p.ViewerId] = Round;

                    RecordCorpse(p);
                    Console.WriteLine($"[BATTLE] placement viewer={p.ViewerId} rank={aliveOthers + 1} round={Round}");
                }
                p.IsShopPurchasable = p.Alive && p.Life > 0;

                var roundExp = roundInfo?.ExperiencePoints ?? 2;
                if (p.Alive && p.Life > 0 && roundExp > 0)
                {
                    p.Exp += roundExp;
                    var leveled = false;
                    while (true)
                    {
                        var need = BattleMaster.ExpToNext(p.Level);
                        if (need < 0 || p.Level >= BattleMaster.MaxLevel || p.Exp < need) break;
                        p.Exp -= need;
                        p.Level += 1;
                        leveled = true;
                    }
                    if (leveled)
                    {
                        p.ShopUnitLotteryRates = ProjectLotteryRates(CurrentLotteryPattern(), p.Level);
                        Console.WriteLine($"[BATTLE] auto-levelup round={Round} viewer={p.ViewerId} -> L{p.Level} expLeft={p.Exp}");
                    }
                }
                int goldDelta = distributeGold + resultRewardGold + interestGold;
                p.Gold += goldDelta;
                p.TotalGold += goldDelta;
                Players[i] = p;

                if (_friendsCommitted.Add((Round, p.ViewerId))
                    && BattleMaster.EvaluateActiveSynergyEffects(p.FieldUnitList)
                        .Any(e => e.SynergyType == 0 && e.Id == 217))
                {
                    var pool = BattleMaster.SynergyTeamUnitIds(217);
                    if (pool.Length > 0)
                    {
                        var rng = new Random(unchecked(p.ViewerId * 7919 + Round * 0x6720 + 217 + _battleSalt * 7919));
                        var friendId = pool[rng.Next(pool.Length)];
                        var reward = BattleMpoFactory.MakeUnit(_nextShopUnitKey++, friendId, 0, 0, UnitState.Deck, 0);
                        reward.IsPurchased = true;
                        if (!_friendsRewards.TryGetValue(p.ViewerId, out var queue))
                        {
                            queue = new List<BattleUnitDataMpo>();
                            _friendsRewards[p.ViewerId] = queue;
                        }
                        queue.Add(reward);
                        Console.WriteLine($"[BATTLE] friend217 round={Round} viewer={p.ViewerId} unit={friendId} queued={queue.Count}");
                    }
                }

                Console.WriteLine($"[BATTLE] commit round={Round} viewer={p.ViewerId} win={win} damage={damage} life={p.Life}");
                Console.WriteLine($"[BATTLE] settle round={Round} viewer={p.ViewerId} base={baseGold} dist={distributeGold} bonus={bonusGold} result={resultRewardGold} interest={interestGold} goldDelta={goldDelta} streakW={_winStreaks.GetValueOrDefault(p.ViewerId)} streakL={_loseStreaks.GetValueOrDefault(p.ViewerId)}");
                if (wasAlive && p.Life <= 0)
                {
                    Console.WriteLine($"[BATTLE] ELIMINATED round={Round} viewer={p.ViewerId} life=0 at={DateTime.Now:HH:mm:ss.fff}");
                }

                if (win && isNpcRound && p.Life > 0)
                {
                    CommitDropPlan(p.ViewerId);
                }

                for (int t = 0; t < Teams.Count; t++)
                {
                    if (Teams[t].TeamId == p.TeamId)
                    {
                        var team = Teams[t];
                        team.Life = p.Life;
                        Teams[t] = team;
                        break;
                    }
                }
            }
            Console.WriteLine($"[BATTLE] commit round={Round} humanLives=[{string.Join(",", HumanViewerIds.Select(vid => $"{vid}:{FindPlayer(vid).Life}"))}] distributeGold={distributeGold}");
        }

        private void DeliverFriendRewards()
        {
            if (_friendsRewards.Count == 0) return;
            foreach (var viewer in _friendsRewards.Keys.ToArray())
            {
                var queue = _friendsRewards[viewer];
                var idx = PlayerIndex(viewer);
                if (idx < 0) continue;
                var delivered = false;
                while (queue.Count > 0)
                {
                    var p = Players[idx];
                    var unit = queue[0];
                    var plan = PlanRankUp(ref p, unit, out _);
                    if (plan != null)
                    {
                        CommitRankUp(ref p, unit, plan, out _);
                        Players[idx] = p;
                        queue.RemoveAt(0);
                        delivered = true;
                        continue;
                    }
                    var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
                    if (deck.Count >= DeckCapacity) break;
                    unit.State = UnitState.Deck;
                    unit.PositionX = 0;
                    unit.PositionY = 0;
                    unit.Index = deck.Count;
                    unit.IsPurchased = true;
                    deck.Add(unit);
                    p.DeckUnitList = deck;
                    ReindexDeck(ref p);
                    Players[idx] = p;
                    queue.RemoveAt(0);
                    delivered = true;
                }
                if (delivered)
                {
                    RefreshSynergies(idx);
                    Console.WriteLine($"[BATTLE] friend217 deliver viewer={viewer} leftInQueue={queue.Count}");
                }
            }
        }

        private BattleRoundResultsMpo BuildRoundResults(int npcId, Dictionary<int, (bool Win, int Damage, int Kills)> matchOutcomes)
        {
            var distributeGold = BattleMaster.GetRound(Round)?.DistributeGold ?? 1;
            if (npcId == 0 && _pvpPairs.Length > 0)
            {

                return BuildPvpRoundResults(distributeGold, matchOutcomes);
            }
            var results = new List<BattleRoundResultMpo>();
            var rewards = new Dictionary<int, RewardDataMpo>();
            for (int i = 0; i < Players.Count; i++)
            {
                var p = Players[i];

                var (win, damage, kills) = matchOutcomes.TryGetValue(p.ViewerId, out var v)
                    ? v
                    : (p.Life > 0 && p.Alive ? (true, 0, 0) : (false, 0, 0));
                results.Add(new BattleRoundResultMpo
                {
                    MatchId = i + 1,
                    PrimaryResult = (win ? BattleResultType.Win : BattleResultType.Lose, damage),
                    SecondaryResult = (win ? BattleResultType.Lose : BattleResultType.Win, 0),

                    KillCounts = new Dictionary<int, int> { [p.ViewerId] = kills },
                });
                rewards[p.ViewerId] = BuildRoundReward(p.ViewerId, win);
            }
            return new BattleRoundResultsMpo
            {
                Round = Round,
                Results = results.ToArray(),
                Players = SnapshotPlayers(),
                Teams = Teams.ToArray(),

                LotteryEquipments = Array.Empty<BattleRoundLotteryResultMpo>(),
                Rewards = rewards,

                IsFinished = Rule == BattleRule.Boss || Players.Any(x => x.Life <= 0),
            };
        }

        private BattleRoundResultsMpo BuildPvpRoundResults(int distributeGold, Dictionary<int, (bool Win, int Damage, int Kills)> matchOutcomes)
        {
            var results = new List<BattleRoundResultMpo>();
            var rewards = new Dictionary<int, RewardDataMpo>();
            foreach (var pair in _pvpPairs)
            {
                var pw = matchOutcomes.TryGetValue(pair.PrimaryViewerId, out var pv) ? pv : (true, 0, 0);
                var sw = matchOutcomes.TryGetValue(pair.SecondaryViewerId, out var sv) ? sv : (false, 0, 0);
                results.Add(new BattleRoundResultMpo
                {
                    MatchId = pair.MatchId,
                    PrimaryResult = (pw.Item1 ? BattleResultType.Win : BattleResultType.Lose, pw.Item2),

                    SecondaryResult = (sw.Item1 ? BattleResultType.Win : BattleResultType.Lose, sw.Item2),

                    KillCounts = new Dictionary<int, int>
                    {
                        [pair.PrimaryViewerId] = pw.Item3,
                        [pair.SecondaryViewerId] = sw.Item3,
                    },
                });
            }

            foreach (var p in Players)
            {

                var (win, _, _) = matchOutcomes.TryGetValue(p.ViewerId, out var v)
                    ? v
                    : (p.Life > 0 && p.Alive ? (true, 0, 0) : (false, 0, 0));
                rewards[p.ViewerId] = BuildRoundReward(p.ViewerId, win);
            }
            return new BattleRoundResultsMpo
            {
                Round = Round,
                Results = results.ToArray(),
                Players = SnapshotPlayers(),
                Teams = Teams.ToArray(),
                LotteryEquipments = Array.Empty<BattleRoundLotteryResultMpo>(),
                Rewards = rewards,
                IsFinished = Players.Any(x => x.Life <= 0),
            };
        }

        private RewardDataMpo BuildRoundReward(int viewerId, bool win)
        {
            if (!_lastRoundSettle.TryGetValue(viewerId, out var s))
            {
                var rd = BattleMaster.GetRound(Round);
                int baseGold = win ? (rd?.WinGold ?? 0) : (rd?.LoseGold ?? 0);
                s = (baseGold, rd?.DistributeGold ?? 1, 0, baseGold, 0);
            }
            return new RewardDataMpo(
                resultType: win ? BattleResultType.Win : BattleResultType.Lose,
                baseGold: s.BaseGold,
                distributeGold: s.DistributeGold,
                bonusGold: s.BonusGold,
                resultRewardGold: s.ResultRewardGold,
                interestGold: s.InterestGold,
                units: Array.Empty<BattleUnitDataMpo>(),
                equipments: Array.Empty<BattleEquipmentDataMpo>());
        }

        public void HandleRoundResultCompleted()
        {
            if (Finished) return;
            _roundResultAckEpoch++;
            var aliveCount = Players.Count(x => x.Life > 0 && x.Alive);

            if (aliveCount <= 1)
            {
                Console.WriteLine($"[BATTLE] round result completed -> alive={aliveCount} -> final result");
                FinishBattle();
                return;
            }

            if (VerifyMode.MaxRounds > 0 && Round >= VerifyMode.MaxRounds)
            {
                Console.WriteLine($"[BATTLE] verify-mode maxRounds={VerifyMode.MaxRounds} reached -> final result");
                FinishBattle();
                return;
            }

            foreach (var vid in HumanViewerIds)
            {
                var h = FindPlayer(vid);
                if (h.ViewerId == 0) continue;
                if (h.Life > 0 && h.Alive) continue;
                SendPersonalTerminalResult(h);
            }
            Round += 1;
            lock (SignalLock)
            {
                ExpectedSignal = new[] { Round, 1 };
            }
            Phase = BattlePhase.Transitioning;
            Console.WriteLine($"[BATTLE] round advanced -> {Round}");
            _ = RunSubsequentRoundSetupSequenceAsync();
        }

        private void SendPersonalTerminalResult(in BattlePlayerDataMpo eliminated)
        {
            var viewerId = eliminated.ViewerId;
            if (!_personalResultsSent.Add(viewerId)) return;
            var rows = BuildBattleResults(pickWinner: false)
                .Where(r => r.ViewerId == viewerId)
                .ToArray();

            GrantPlacementBp(viewerId, rows.Length > 0 ? rows[0].Ranking : 8);
            var teamsSnap = Teams.ToArray();
            Console.WriteLine($"[BATTLE] personal-terminal(OnDefeatPlayer) round={Round} viewer={viewerId} rows={rows.Length} sentAt={DateTime.Now:HH:mm:ss.fff}");
            _ = Task.Run(async () =>
            {
                await Task.Delay(200);
                BroadcastToViewer(viewerId, bc => bc.OnDefeatPlayer(rows, teamsSnap));
            });
        }

        private void GrantPlacementBp(int viewerId, int ranking)
        {
            if (viewerId == 0 || !IsHuman(viewerId)) return;
            if (Rule == BattleRule.Tutorial || Rule == BattleRule.DebugMode) return;
            if (!_bpGranted.TryAdd(viewerId, true)) return;
            if (_abandonedViewers.Contains(viewerId))
            {
                Console.WriteLine($"[BATTLE] BP viewer={viewerId} rank={ranking} -> +0 (surrender grants no points)");
                return;
            }
            var current = AccountStore.BattlePointOf(viewerId);
            var gain = AccountStore.BpGain(current, ranking);
            AccountStore.AddBattlePoint(viewerId, gain);
            Console.WriteLine($"[BATTLE] BP viewer={viewerId} rank={ranking} cur={current} -> +{gain}");
        }

        private void FinishBattle()
        {
            if (BattleCompleteEmitted) return;
            BattleCompleteEmitted = true;
            Finished = true;
            FinishedAtUtc = DateTime.UtcNow;
            DumpConnectionDiagnostics();
            Net.DumpBattle();
            lock (SignalLock)
            {
                ExpectedSignal = null;
            }
            Phase = BattlePhase.Result;

            var survivors = Players.Where(x => x.Alive && x.Life > 0 && !_placements.ContainsKey(x.ViewerId))
                .OrderByDescending(x => x.Life).ThenBy(x => x.ViewerId).ToList();
            for (int si = 0; si < survivors.Count; si++) _placements[survivors[si].ViewerId] = si + 1;
            var results = BuildBattleResults();

            var champion = Array.Find(results, r => r.Ranking == 1);
            var championGotPersonal = _personalResultsSent.Contains(champion.ViewerId);
            if (Rule == BattleRule.Boss)
            {

                Console.WriteLine($"[BATTLE] OnBattleComplete round={Round} rule=Boss results={results.Length} (no bp/leaderboard/deck)");
            }
            else
            {

                foreach (var r in results)
                    GrantPlacementBp(r.ViewerId, r.Ranking);
                Console.WriteLine($"[BATTLE] OnBattleComplete round={Round} results={results.Length} champion={champion.ViewerId} championGotPersonal={championGotPersonal} finishedAt={DateTime.Now:HH:mm:ss.fff}");
                RecordLeaderboard(results);

                if (Rule != BattleRule.Tutorial && Rule != BattleRule.DebugMode)
                {
                    AccountStore.AddCompletedBattles(results.Where(r => IsHuman(r.ViewerId)).Select(r => r.ViewerId));
                }

                if (Rule == BattleRule.Normal)
                {
                    SaveBossDecksFromFinalBoards(results);
                }
            }

            _ = Task.Run(async () =>
            {
                await Task.Delay(200);
                Broadcast(bc => bc.OnBattleComplete(results));
            });
        }

        private void RecordLeaderboard(BattleResultInfoMpo[] results)
        {
            if (Rule == BattleRule.Tutorial || Rule == BattleRule.DebugMode) return;
            var rows = new List<LeaderboardStore.LineupRow>(results.Length);
            foreach (var r in results)
            {
                var corpse = CorpseOf(r.ViewerId);
                IEnumerable<BattleUnitDataMpo> units = corpse?.MirroredUnits;
                if (units == null || !units.Any()) units = r.FieldUnitList;
                rows.Add(new LeaderboardStore.LineupRow(r.ViewerId, IsHuman(r.ViewerId), r.Ranking,
                    units == null ? Enumerable.Empty<int>() : units.Where(u => u != null).Select(u => u.UnitId)));
            }
            LeaderboardStore.Record(BattleId.ToString(), Round, rows);
        }

        private void SaveBossDecksFromFinalBoards(BattleResultInfoMpo[] results)
        {
            foreach (var r in results)
            {
                try
                {
                    if (r.ViewerId == 0 || !IsHuman(r.ViewerId)) continue;
                    var p = FindPlayer(r.ViewerId);
                    var corpse = CorpseOf(r.ViewerId);
                    IEnumerable<BattleUnitDataMpo> board;
                    if (corpse != null && corpse.MirroredUnits is { Length: > 0 })
                        board = MirrorHalf(corpse.MirroredUnits);
                    else
                        board = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
                    var units = board.Where(u => u != null).ToList();
                    if (units.Count == 0) continue;
                    var syn = BattleMaster.EvaluateFieldSynergies(units);
                    var deck = new BossBattleStore.DeckRec
                    {
                        Rank = r.Ranking,
                        Level = p.Level,
                        Avatar = p.AvatarId,
                        Power = 0,
                        Roles = syn.Where(s => s != null && s.IsConfirmed && s.SynergyType == SynergyType.Role)
                            .Select(s => new BossBattleStore.SynRec { Id = s.Id, Count = s.CurrentFieldCount }).ToList(),
                        Teams = syn.Where(s => s != null && s.IsConfirmed && s.SynergyType == SynergyType.Team)
                            .Select(s => new BossBattleStore.SynRec { Id = s.Id, Count = s.CurrentFieldCount }).ToList(),
                        Units = units.Select(u => new BossBattleStore.DeckUnitRec
                        {
                            Key = u.UniqueKey,
                            UnitId = u.UnitId,
                            Rank = u.Rank,
                            X = u.PositionX,
                            Y = u.PositionY,
                            Equips = (u.EquipmentList ?? new List<BattleEquipmentDataMpo>())
                                .Where(e => e != null).Select(e => e.Id).ToArray(),
                        }).ToList(),
                    };
                    BossBattleStore.SaveDeck(r.ViewerId, deck);
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"[BATTLE] boss saveDeck FAIL viewer={r.ViewerId}: {ex.Message}");
                }
            }
        }

        public void AbandonOp(int viewerId)
        {
            var idx = -1;
            for (var i = 0; i < Players.Count; i++)
            {
                if (Players[i].ViewerId == viewerId) { idx = i; break; }
            }
            if (idx < 0) return;
            if (Finished || _abandonedViewers.Contains(viewerId))
            {
                Console.WriteLine($"[BATTLE] abandon ignored viewer={viewerId} finished={Finished} already={_abandonedViewers.Contains(viewerId)}");
                return;
            }
            _abandonedViewers.Add(viewerId);
            var p = Players[idx];
            p.Life = 0;
            p.Alive = false;

            if (!_eliminatedRound.ContainsKey(viewerId)) _eliminatedRound[viewerId] = Round;
            Players[idx] = p;
            RecordCorpse(p);
            for (var t = 0; t < Teams.Count; t++)
            {
                if (Teams[t].TeamId != p.TeamId) continue;
                var team = Teams[t];
                team.Life = 0;
                Teams[t] = team;
                break;
            }
            Console.WriteLine($"[BATTLE] abandon viewer={viewerId} round={Round} phase={Phase} life=0");

            if (_personalResultsSent.Add(viewerId))
            {

                var abandonRows = BuildBattleResults(pickWinner: false)
                    .Where(r => r.ViewerId == viewerId)
                    .ToArray();
                if (IsHuman(viewerId))
                {
                    GrantPlacementBp(viewerId, abandonRows.Length > 0 ? abandonRows[0].Ranking : 8);
                }
                BroadcastToViewer(viewerId, bc => bc.OnDefeatPlayer(abandonRows, Teams.ToArray()));
                Console.WriteLine($"[BATTLE] OnDefeatPlayer(abandon) viewer={viewerId} rows={abandonRows.Length} round={Round} at={DateTime.Now:HH:mm:ss.fff}");
            }

            if (!HasOtherLiveHumanSeat(viewerId))
            {
                FinishBattle();
            }
        }

        public void SendEmoteOp(int viewerId, int emoteId)
        {
            if (Finished)
            {
                Console.WriteLine($"[BATTLE] SendEmote reject viewer={viewerId} emote={emoteId} (finished)");
                return;
            }
            if (_abandonedViewers.Contains(viewerId))
            {
                Console.WriteLine($"[BATTLE] SendEmote reject viewer={viewerId} emote={emoteId} (abandoned)");
                return;
            }
            var p = FindPlayer(viewerId);
            if (p.ViewerId == 0 || p.Life <= 0 || !p.Alive)
            {
                Console.WriteLine($"[BATTLE] SendEmote reject viewer={viewerId} emote={emoteId} (eliminated)");
                return;
            }
            if (!BattleMaster.IsEmoteIdValid(emoteId))
            {

                Console.WriteLine($"[BATTLE] SendEmote reject viewer={viewerId} emote={emoteId} (invalid id, not in master emote_data)");
                return;
            }
            Broadcast(bc => bc.OnSendEmote(viewerId, emoteId));
            Console.WriteLine($"[BATTLE] SendEmote viewer={viewerId} emote={emoteId} round={Round} broadcast at={DateTime.Now:HH:mm:ss.fff}");
        }

        public float BattleEventIntervalRate { get; private set; } = 1f;

        public void SetBattleEventIntervalRateOp(int viewerId, float rate)
        {
            BattleEventIntervalRate = rate;
            RecordIntervalRate(viewerId, rate);
            Console.WriteLine($"[BATTLE] SetBattleEventIntervalRate battle={BattleId} viewer={viewerId} rate={rate} round={Round}");
        }

        public bool UpdateRecommendPartyDataOp(int viewerId, int partyId)
        {
            if (Finished)
            {
                Console.WriteLine($"[BATTLE] UpdateRecommendPartyData reject viewer={viewerId} party={partyId} (finished)");
                return false;
            }
            if (!BattleMaster.IsRecommendPartyIdValid(partyId))
            {
                Console.WriteLine($"[BATTLE] UpdateRecommendPartyData reject viewer={viewerId} party={partyId} (invalid id, not in master recommend_party)");
                return false;
            }
            var idx = -1;
            for (var i = 0; i < Players.Count; i++)
            {
                if (Players[i].ViewerId == viewerId) { idx = i; break; }
            }
            if (idx < 0)
            {
                Console.WriteLine($"[BATTLE] UpdateRecommendPartyData reject viewer={viewerId} party={partyId} (viewer not found)");
                return false;
            }
            var p = Players[idx];
            p.RecommendPartyDataId = partyId;
            Players[idx] = p;
            Broadcast(bc => bc.OnUpdateRecommendPartyData(viewerId, partyId));
            Console.WriteLine($"[BATTLE] UpdateRecommendPartyData viewer={viewerId} party={partyId} round={Round}");
            return true;
        }

        public void ForceCompleteWaitForCommandOp(int callerViewerId)
        {
            if (Finished || Phase != BattlePhase.Preparing)
            {
                Console.WriteLine($"[BATTLE] ForceCompleteWaitForCommand caller={callerViewerId} no-op phase={Phase} finished={Finished}");
                return;
            }
            Console.WriteLine($"[BATTLE] ForceCompleteWaitForCommand caller={callerViewerId} round={Round} marking all ready");
            foreach (var p in Players.ToArray())
            {
                if (p.ViewerId != 0 && p.Alive && p.Life > 0) RegisterReady(p.ViewerId);
            }
        }

        public void ForceSurrenderPlayersOp(int callerViewerId)
        {
            if (Finished)
            {
                Console.WriteLine($"[BATTLE] ForceSurrenderPlayers caller={callerViewerId} no-op (finished)");
                return;
            }
            var surrendered = 0;
            for (var i = 0; i < Players.Count; i++)
            {
                var p = Players[i];
                if (p.ViewerId == callerViewerId || p.ViewerId == 0) continue;
                if (_abandonedViewers.Contains(p.ViewerId)) continue;
                _abandonedViewers.Add(p.ViewerId);
                p.Life = 0;
                p.Alive = false;

                if (!_eliminatedRound.ContainsKey(p.ViewerId)) _eliminatedRound[p.ViewerId] = Round;
                Players[i] = p;
                RecordCorpse(p);
                for (var t = 0; t < Teams.Count; t++)
                {
                    if (Teams[t].TeamId != p.TeamId) continue;
                    var team = Teams[t];
                    team.Life = 0;
                    Teams[t] = team;
                    break;
                }
                surrendered++;
            }
            if (surrendered == 0)
            {
                Console.WriteLine($"[BATTLE] ForceSurrenderPlayers caller={callerViewerId} no-op (nobody else alive)");
                return;
            }
            Broadcast(bc => bc.OnUpdatePlayers(Players.ToArray()));
            Console.WriteLine($"[BATTLE] ForceSurrenderPlayers caller={callerViewerId} surrendered={surrendered} round={Round}");
            FinishBattle();
        }

        public bool EditPlayerInfoOp(int targetViewerId, int? life, int? gold, int? rankUpToken = null)
        {
            var idx = -1;
            for (var i = 0; i < Players.Count; i++)
            {
                if (Players[i].ViewerId == targetViewerId) { idx = i; break; }
            }
            if (idx < 0)
            {
                Console.WriteLine($"[BATTLE] DebugEditPlayerInfo reject viewer={targetViewerId} (not found)");
                return false;
            }
            var p = Players[idx];
            if (life.HasValue) p.Life = life.Value;
            if (gold.HasValue) p.Gold = gold.Value;
            if (rankUpToken.HasValue) p.RankUpToken = rankUpToken.Value;
            Players[idx] = p;
            Console.WriteLine($"[BATTLE] DebugEditPlayerInfo viewer={targetViewerId} life={(life.HasValue ? life.Value.ToString() : "keep")} gold={(gold.HasValue ? gold.Value.ToString() : "keep")} token={(rankUpToken.HasValue ? rankUpToken.Value.ToString() : "keep")}");
            return true;
        }

        private int _nextShopUnitKey = 300000;

        private readonly ConcurrentDictionary<int, Random> _shopRngByViewer = new ConcurrentDictionary<int, Random>();
        public const int ShopSize = 5;
        public const int DeckCapacity = 8;

        private readonly Dictionary<int, List<BattleUnitDataMpo>> _friendsRewards = new();

        private int _deploySeq;
        private readonly Dictionary<int, int> _deploySeqs = new();

        private readonly Dictionary<int, int> _placements = new();

        private readonly Dictionary<int, int> _eliminatedRound = new();

        private List<(float at, BattleBoardEventResponse ev)>? _liveAxis;
        private int _liveAxisSent;
        private int _liveAxisRound;
        private readonly object _liveAxisLock = new object();

        private Dictionary<int, int> _ownMatchByViewer = new Dictionary<int, int>();
        private int _ownMatchByViewerRound = -1;

        private void ProcessDeferredMerges(string why)
        {
            for (var idx = 0; idx < Players.Count; idx++)
            {
                var p = Players[idx];
                if (!p.Alive || p.Life <= 0) continue;
                var guard = 0;
                while (guard++ < 16 && TryDeferredMergeOnce(ref p, why))
                {
                    Players[idx] = p;
                    RefreshSynergies(idx);
                }
            }
        }

        private bool TryDeferredMergeOnce(ref BattlePlayerDataMpo p, string why)
        {
            var pool = new List<BattleUnitDataMpo>();
            foreach (var u in p.FieldUnitList ?? new List<BattleUnitDataMpo>())
                if (!u.IsToken && !u.IsHelp && u.Rank < 3) pool.Add(u);
            foreach (var u in p.DeckUnitList ?? new List<BattleUnitDataMpo>())
                if (!u.IsToken && !u.IsHelp && u.Rank < 3) pool.Add(u);
            for (var i = 0; i < pool.Count; i++)
            {
                for (var j = i + 1; j < pool.Count; j++)
                {
                    if (pool[j].UnitId != pool[i].UnitId || pool[j].Rank != pool[i].Rank) continue;
                    for (var k = j + 1; k < pool.Count; k++)
                    {
                        if (pool[k].UnitId != pool[i].UnitId || pool[k].Rank != pool[i].Rank) continue;

                        var trio = new List<BattleUnitDataMpo> { pool[i], pool[j], pool[k] };
                        var anchor = trio.FirstOrDefault(u => u.State == UnitState.Field) ?? trio[0];
                        var mats = trio.Where(u => !ReferenceEquals(u, anchor)).ToList();
                        CommitRankUp(ref p, mats[1], new List<BattleUnitDataMpo> { anchor, mats[0] }, out var survivor);
                        Console.WriteLine($"[BATTLE] deferred-merge({why}) unit={pool[i].UnitId} rank->{pool[i].Rank + 1} survivor={survivor.UniqueKey} state={survivor.State}");
                        return true;
                    }
                }
            }
            return false;
        }
        private readonly HashSet<(int Round, int Viewer)> _friendsCommitted = new();

        public sealed class ShopOpResult
        {
            public bool Ok;
            public string Reason = "";
            public int UniqueKey;
            public BattleUnitDataMpo RankUppedUnit;
            public bool DidRankUp;

            public int TokenNum;

            public BattleEquipmentDataMpo[] ReceivedEquipments = Array.Empty<BattleEquipmentDataMpo>();
        }

        private Random GetShopRng(int viewerId)
        {

            return _shopRngByViewer.GetOrAdd(viewerId,
                v => new Random(unchecked(v * 9176 + 31 + _battleSalt * 7919)));
        }

        private int PlayerIndex(int viewerId)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].ViewerId == viewerId) return i;
            }
            return -1;
        }

        public bool CanEditPreparation(int viewerId)
        {
            if (Finished) return false;
            var idx = PlayerIndex(viewerId);
            if (idx < 0 || !Players[idx].Alive || Players[idx].Life <= 0) return false;
            return Phase == BattlePhase.Preparing && PreparationOpen && !BattleRunning;
        }

        public bool CanTrade(int viewerId)
        {
            if (Finished || Phase == BattlePhase.Initialized || Phase == BattlePhase.Complete) return false;
            var idx = PlayerIndex(viewerId);
            if (idx < 0 || !Players[idx].Alive || Players[idx].Life <= 0) return false;
            return true;
        }

        public static List<float> ProjectLotteryRates(int patternId, int level)
        {
            var rates = BattleMaster.ShopLotteryRates(patternId, level);
            return new List<float> { rates[0] / 1000f, rates[1] / 1000f, rates[2] / 1000f, rates[3] / 1000f, rates[4] / 1000f };
        }

        private int CurrentLotteryPattern()
        {
            var rule = BattleMaster.GetRule((int)Rule) ?? BattleMaster.GetRule(0);
            return rule?.ShopUnitLotteryPatternId ?? 1;
        }

        private List<BattleUnitDataMpo> RollShopUnits(int viewerId, int level)
        {
            var rates = BattleMaster.ShopLotteryRates(CurrentLotteryPattern(), level);
            var total = 0;
            foreach (var r in rates) total += r;
            if (total <= 0)
            {
                rates = new[] { 1000, 0, 0, 0, 0 };
                total = 1000;
            }
            var rng = GetShopRng(viewerId);
            var list = new List<BattleUnitDataMpo>();
            for (int slot = 0; slot < ShopSize; slot++)
            {
                var roll = rng.Next(total);
                var tier = 0;
                var acc = 0;
                for (int t = 0; t < 5; t++)
                {
                    acc += rates[t];
                    if (roll < acc) { tier = t; break; }
                }
                var pool = BattleMaster.ShopTierPool(tier + 1);
                if (pool.Length == 0) pool = BattleMaster.InitialPool;
                if (pool.Length == 0) break;
                var unitId = pool[rng.Next(pool.Length)];
                var key = _nextShopUnitKey++;
                list.Add(BattleMpoFactory.MakeUnit(key, unitId, 0, 0, UnitState.Shop, slot));
            }
            return list;
        }

        public bool GenerateShop(int viewerId)
        {
            var idx = PlayerIndex(viewerId);
            if (idx < 0) return false;
            var p = Players[idx];
            p.ShopUnitList = RollShopUnits(viewerId, Math.Max(1, p.Level));
            p.ShopUnitLotteryRates = ProjectLotteryRates(CurrentLotteryPattern(), Math.Max(1, p.Level));
            Players[idx] = p;
            return true;
        }

        private static int UnitTier(BattleUnitDataMpo u)
        {
            var info = BattleMaster.GetUnit(u.UnitId);
            return info != null ? Math.Max(1, Math.Min(5, info.Price)) : 1;
        }

        private static int UnitPrice(BattleUnitDataMpo u)
        {
            var info = BattleMaster.GetUnit(u.UnitId);
            return info?.Price ?? 1;
        }

        public ShopOpResult RefreshShop(int viewerId)
        {
            var res = new ShopOpResult();

            if (!CanTrade(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];
            var price = (BattleMaster.GetRule((int)Rule) ?? BattleMaster.GetRule(0))?.RefreshShopPrice ?? 2;
            if (p.Gold < price) { res.Reason = "refresh_unavailable"; return res; }
            p.Gold -= price;
            p.ShopUnitList = RollShopUnits(viewerId, Math.Max(1, p.Level));
            Players[idx] = p;
            res.Ok = true;
            return res;
        }

        public ShopOpResult BuyShopUnit(int viewerId, int uniqueKey, int? deckIndex, int? fx, int? fy, int? fIndex)
        {
            var res = new ShopOpResult { UniqueKey = uniqueKey };
            if (!CanTrade(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];
            var shop = p.ShopUnitList ?? new List<BattleUnitDataMpo>();
            var shopIdx = shop.FindIndex(u => u.UniqueKey == uniqueKey);
            var shopUnit = shopIdx >= 0 ? shop[shopIdx] : default;
            if (shopIdx < 0 || p.Gold < UnitPrice(shopUnit))
            {

                if (shopIdx < 0) SendOwnShopPulse(viewerId);
                res.Reason = "shop_unit_unavailable";
                return res;
            }

            if (!CanEditPreparation(viewerId))
            {
                var deckPlan = PlanRankUp(ref p, shopUnit, out _, deckOnly: true);
                shop.RemoveAt(shopIdx);
                p.ShopUnitList = shop;
                p.Gold -= UnitPrice(shopUnit);
                if (deckPlan != null)
                {

                    CommitRankUp(ref p, shopUnit, deckPlan, out var survivor, deckOnly: true);
                    Players[idx] = p;
                    RefreshSynergies(idx);
                    res.DidRankUp = true;
                    res.RankUppedUnit = survivor;
                    res.Ok = true;
                    return res;
                }
                if ((p.DeckUnitList?.Count ?? 0) >= DeckCapacity) { res.Reason = "deck_full"; return res; }
                shopUnit.State = UnitState.Deck;
                shopUnit.PositionX = 0;
                shopUnit.PositionY = 0;
                shopUnit.Index = p.DeckUnitList.Count;
                shopUnit.IsPurchased = true;
                p.DeckUnitList = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
                p.DeckUnitList.Add(shopUnit);
                ReindexDeck(ref p);
                Players[idx] = p;
                RefreshSynergies(idx);
                res.Ok = true;
                return res;
            }

            var mergePlan = PlanRankUp(ref p, shopUnit, out var mergeErr);
            if (mergeErr != null) { res.Reason = mergeErr; return res; }

            if (mergePlan == null)
            {

                if (fx.HasValue)
                {
                    var err = ValidateFieldDestination(p, fx.Value, fy.Value);

                    if (err != null && err != "occupied_cell") { res.Reason = err; return res; }
                    if (p.FieldUnitList.Count >= Math.Max(1, p.Level)) { res.Reason = "field_limit_reached"; return res; }
                    if (err == "occupied_cell") fx = null;
                }
                else
                {
                    var cap = deckIndex ?? p.DeckUnitList.Count;
                    if (p.DeckUnitList.Count >= DeckCapacity) { res.Reason = "deck_full"; return res; }
                    if (cap < 0 || cap > p.DeckUnitList.Count) cap = p.DeckUnitList.Count;
                }
            }

            shop.RemoveAt(shopIdx);
            p.ShopUnitList = shop;
            p.Gold -= UnitPrice(shopUnit);

            if (mergePlan != null)
            {
                CommitRankUp(ref p, shopUnit, mergePlan, out var survivor);
                res.DidRankUp = true;
                res.RankUppedUnit = survivor;
            }
            else if (fx.HasValue)
            {
                shopUnit.State = UnitState.Field;
                shopUnit.PositionX = fx.Value;
                shopUnit.PositionY = fy.Value;
                shopUnit.Index = fIndex ?? p.FieldUnitList.Count;
                shopUnit.IsPurchased = true;
                p.FieldUnitList = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
                p.FieldUnitList.Add(shopUnit);
            }
            else
            {
                shopUnit.State = UnitState.Deck;
                shopUnit.PositionX = 0;
                shopUnit.PositionY = 0;
                shopUnit.Index = deckIndex ?? p.DeckUnitList.Count;
                shopUnit.IsPurchased = true;
                p.DeckUnitList = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
                if (deckIndex.HasValue && deckIndex.Value < p.DeckUnitList.Count)
                {
                    p.DeckUnitList.Insert(Math.Max(0, deckIndex.Value), shopUnit);
                }
                else
                {
                    p.DeckUnitList.Add(shopUnit);
                }
                ReindexDeck(ref p);
            }

            Players[idx] = p;
            RefreshSynergies(idx);
            res.Ok = true;
            return res;
        }

        public ShopOpResult SellUnitOp(int viewerId, int uniqueKey)
        {
            var res = new ShopOpResult { UniqueKey = uniqueKey };
            if (!CanTrade(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];

            var fromField = (p.FieldUnitList ?? new List<BattleUnitDataMpo>()).Exists(u => u.UniqueKey == uniqueKey);
            if (fromField && !CanEditPreparation(viewerId)) { res.Reason = "field_unit_battle_locked"; return res; }
            BattleUnitDataMpo unit = default;
            var found = false;
            foreach (var u in p.FieldUnitList ?? new List<BattleUnitDataMpo>())
            {
                if (u.UniqueKey == uniqueKey) { unit = u; found = true; break; }
            }
            if (!found)
            {
                foreach (var u in p.DeckUnitList ?? new List<BattleUnitDataMpo>())
                {
                    if (u.UniqueKey == uniqueKey) { unit = u; found = true; break; }
                }
            }
            if (!found) { res.Reason = "unit_not_found"; return res; }

            var price = BattleMaster.SellUnitPrice(unit.Rank, UnitTier(unit));
            p.Gold += price;

            if (unit.EquipmentList != null && unit.EquipmentList.Count > 0)
            {
                var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
                inv.AddRange(unit.EquipmentList);
                p.EquipmentList = inv;
            }
            p.FieldUnitList?.RemoveAll(u => u.UniqueKey == uniqueKey);
            p.DeckUnitList?.RemoveAll(u => u.UniqueKey == uniqueKey);
            if (p.FieldUnitList != null) ReindexField(ref p);
            if (p.DeckUnitList != null) ReindexDeck(ref p);
            Players[idx] = p;
            RefreshSynergies(idx);
            res.Ok = true;
            return res;
        }

        public ShopOpResult BuyExpOp(int viewerId)
        {
            var res = new ShopOpResult();
            if (!CanTrade(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];

            var need = BattleMaster.ExpToNext(p.Level) - p.Exp;
            if (BattleMaster.ExpToNext(p.Level) < 0) { res.Reason = "buy_exp_unavailable"; return res; }
            var levelBefore = p.Level;
            var expBefore = p.Exp;
            var goldBefore = p.Gold;
            if (need <= 0)
            {

                p.Level += 1;
                p.Exp = 0;
                p.ShopUnitLotteryRates = ProjectLotteryRates(CurrentLotteryPattern(), p.Level);
                Players[idx] = p;
                res.Ok = true;

                Console.WriteLine($"[BATTLE] buyexp free viewer={viewerId} L{levelBefore}->L{p.Level} exp={expBefore} need={need} gold={goldBefore}");
                return res;
            }
            if (p.Gold < need)
            {
                res.Reason = "buy_exp_unavailable";
                Console.WriteLine($"[BATTLE] buyexp reject viewer={viewerId} L={levelBefore} exp={expBefore} need={need} gold={goldBefore}");
                return res;
            }
            p.Gold -= need;
            p.Level += 1;
            p.Exp = 0;
            p.ShopUnitLotteryRates = ProjectLotteryRates(CurrentLotteryPattern(), p.Level);
            Players[idx] = p;
            res.Ok = true;

            Console.WriteLine($"[BATTLE] buyexp viewer={viewerId} L{levelBefore}->L{p.Level} exp={expBefore} need={need} gold={goldBefore}->{p.Gold}");
            return res;
        }

        public BattlePlayerDataMpo ToggleShopLockOp(int viewerId)
        {
            var idx = PlayerIndex(viewerId);
            if (idx < 0) return default;

            if (CanTrade(viewerId))
            {
                var p = Players[idx];
                p.IsShopLock = !p.IsShopLock;
                Players[idx] = p;
                Console.WriteLine($"[BATTLE] shop lock -> {p.IsShopLock} viewer={viewerId}");
            }
            return Players[idx];
        }

        private ShopOpResult FieldDeployOrMoveOp(int viewerId, int uniqueKey, int x, int y, int index, bool countAsReady)
        {
            var res = new ShopOpResult { UniqueKey = uniqueKey };
            if (!CanEditPreparation(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];
            var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            var field = p.FieldUnitList ?? new List<BattleUnitDataMpo>();

            var deckIdx = deck.FindIndex(u => u.UniqueKey == uniqueKey);
            var fieldIdx = field.FindIndex(u => u.UniqueKey == uniqueKey);
            if (deckIdx < 0 && fieldIdx < 0) { res.Reason = "unit_not_owned"; return res; }
            var fromDeck = deckIdx >= 0;
            var unit = fromDeck ? deck[deckIdx] : field[fieldIdx];

            if (x < 0 || x > 5 || y < 4 || y > 7) { res.Reason = "not_player_deploy_cell"; return res; }
            var otherIdx = field.FindIndex(u => u.UniqueKey != uniqueKey && u.PositionX == x && u.PositionY == y);

            if (fromDeck)
            {
                deck.RemoveAt(deckIdx);
                if (otherIdx >= 0)
                {
                    var other = field[otherIdx];
                    field.RemoveAt(otherIdx);
                    other.State = UnitState.Deck;
                    other.PositionX = 0;
                    other.PositionY = 0;
                    deck.Insert(Math.Min(deckIdx, deck.Count), other);
                }
                unit.State = UnitState.Field;
                unit.PositionX = x;
                unit.PositionY = y;
                field.Add(unit);
                _deploySeqs[uniqueKey] = ++_deploySeq;
            }
            else
            {
                if (otherIdx >= 0)
                {
                    var other = field[otherIdx];
                    (unit.PositionX, other.PositionX) = (other.PositionX, unit.PositionX);
                    (unit.PositionY, other.PositionY) = (other.PositionY, unit.PositionY);
                    (unit.Index, other.Index) = (other.Index, unit.Index);
                }
                else
                {
                    unit.PositionX = x;
                    unit.PositionY = y;
                }
            }

            p.FieldUnitList = field;
            p.DeckUnitList = deck;
            ReindexDeck(ref p);
            ReindexField(ref p);

            MergeAfterCompositionChange(ref p, "move", idx);
            Players[idx] = p;
            RefreshSynergies(idx);
            res.Ok = true;
            res.RankUppedUnit = unit;
            if (VerifyMode.InstantReady && countAsReady && IsHuman(viewerId))
            {
                RegisterReady(viewerId);
            }
            return res;
        }

        public ShopOpResult DeployDeckUnitOp(int viewerId, int uniqueKey, int x, int y, int index)
            => FieldDeployOrMoveOp(viewerId, uniqueKey, x, y, index, countAsReady: true);

        public ShopOpResult ChangeFieldUnitPositionOp(int viewerId, int uniqueKey, int x, int y, int index)
            => FieldDeployOrMoveOp(viewerId, uniqueKey, x, y, index, countAsReady: false);

        private bool MergeAfterCompositionChange(ref BattlePlayerDataMpo p, string why, int idx)
        {
            var merged = false;
            var guard = 0;
            while (guard++ < 16 && TryDeferredMergeOnce(ref p, why))
            {
                merged = true;
                RefreshSynergies(idx);
            }
            if (merged) Players[idx] = p;
            return merged;
        }

        public ShopOpResult ReturnFieldUnitToDeckOp(int viewerId, int uniqueKey, int index)
        {
            var res = new ShopOpResult { UniqueKey = uniqueKey };
            if (!CanEditPreparation(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];
            var field = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
            var unit = field.Find(u => u.UniqueKey == uniqueKey);
            if (unit == null) { res.Reason = "field_unit_not_found"; return res; }
            if (p.DeckUnitList != null && p.DeckUnitList.Count >= DeckCapacity) { res.Reason = "deck_full"; return res; }

            field.Remove(unit);
            p.FieldUnitList = field;
            unit.State = UnitState.Deck;
            unit.PositionX = 0;
            unit.PositionY = 0;
            unit.Index = 0;
            p.DeckUnitList = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            var deck = p.DeckUnitList;
            var insertAt = Math.Max(0, Math.Min(index, deck.Count));
            deck.Insert(insertAt, unit);
            p.DeckUnitList = deck;
            ReindexDeck(ref p);
            ReindexField(ref p);

            MergeAfterCompositionChange(ref p, "return", idx);
            Players[idx] = p;
            RefreshSynergies(idx);
            res.Ok = true;
            res.RankUppedUnit = unit;
            return res;
        }

        public ShopOpResult ChangeDeckUnitPositionOp(int viewerId, int beforeIndex, int afterIndex)
        {
            var res = new ShopOpResult();
            if (!CanEditPreparation(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];
            var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            if (deck.Count == 0) { res.Reason = "deck_empty"; return res; }
            if (beforeIndex < 0 || beforeIndex >= deck.Count)
            {
                Console.WriteLine($"[BATTLE] deck reorder reject viewer={viewerId} before={beforeIndex} after={afterIndex} deck={deck.Count} round={Round} phase={Phase}");
                res.Reason = "deck_index_out_of_bounds";
                return res;
            }

            var unit = deck[beforeIndex];
            deck.RemoveAt(beforeIndex);
            var insertAt = Math.Max(0, Math.Min(afterIndex, deck.Count));
            deck.Insert(insertAt, unit);
            p.DeckUnitList = deck;
            ReindexDeck(ref p);
            Players[idx] = p;
            res.Ok = true;
            res.RankUppedUnit = unit;
            return res;
        }

        private List<BattleUnitDataMpo> PlanRankUp(ref BattlePlayerDataMpo p, BattleUnitDataMpo incoming, out string mergeErr, bool deckOnly = false, int excludeKey = 0)
        {
            mergeErr = null;
            if (incoming.Rank >= 3) return null;

            var same = new List<BattleUnitDataMpo>();
            if (!deckOnly)
            {
                foreach (var u in p.FieldUnitList ?? new List<BattleUnitDataMpo>())
                {
                    if (u.UniqueKey == excludeKey) continue;
                    if (u.UnitId == incoming.UnitId && u.Rank == incoming.Rank && !u.IsToken && !u.IsHelp) same.Add(u);
                }
            }
            foreach (var u in p.DeckUnitList ?? new List<BattleUnitDataMpo>())
            {
                if (u.UniqueKey == excludeKey) continue;
                if (u.UnitId == incoming.UnitId && u.Rank == incoming.Rank && !u.IsToken && !u.IsHelp) same.Add(u);
            }
            if (same.Count + 1 < 3) return null;
            return same.GetRange(0, 2);
        }

        public const int MaxUnitSynergies = 3;

        private static bool ExceedsSynergyCap(BattleUnitDataMpo unit, BattleEquipmentDataMpo incoming)
        {
            var info = BattleMaster.GetUnit(unit.UnitId);
            var roles = new HashSet<int>(info?.RoleTypes ?? Array.Empty<int>());
            var types = new HashSet<int>(info?.TeamTypes ?? Array.Empty<int>());
            foreach (var e in unit.EquipmentList ?? Enumerable.Empty<BattleEquipmentDataMpo>())
            {
                if ((int)e.RoleType != 0) roles.Add((int)e.RoleType);
                if ((int)e.TeamType != 0) types.Add((int)e.TeamType);
            }
            if ((int)incoming.RoleType != 0) roles.Add((int)incoming.RoleType);
            if ((int)incoming.TeamType != 0) types.Add((int)incoming.TeamType);
            return roles.Count + types.Count > MaxUnitSynergies;
        }

        private static void CarryMergedGear(ref BattlePlayerDataMpo p, int survivorUnitId, List<BattleEquipmentDataMpo> merged)
        {
            var overflow = new List<BattleEquipmentDataMpo>();
            var info = BattleMaster.GetUnit(survivorUnitId);
            var roles = new HashSet<int>(info?.RoleTypes ?? Array.Empty<int>());
            var types = new HashSet<int>(info?.TeamTypes ?? Array.Empty<int>());
            var kept = new List<BattleEquipmentDataMpo>();
            foreach (var g in merged)
            {
                var fits = kept.Count < MaxUnitEquipment;
                int r = (int)g.RoleType, t = (int)g.TeamType;
                if (fits && (r != 0 || t != 0))
                {
                    var nr = roles.Count + ((r != 0 && !roles.Contains(r)) ? 1 : 0);
                    var nt = types.Count + ((t != 0 && !types.Contains(t)) ? 1 : 0);
                    fits = nr + nt <= MaxUnitSynergies;
                }
                if (!fits) { overflow.Add(g); continue; }
                if (r != 0) roles.Add(r);
                if (t != 0) types.Add(t);
                kept.Add(g);
            }
            merged.Clear();
            merged.AddRange(kept);
            ReturnGearToInventory(ref p, overflow);
        }

        private static void ReturnGearToInventory(ref BattlePlayerDataMpo p, List<BattleEquipmentDataMpo> gear)
        {
            if (gear == null || gear.Count == 0) return;
            var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
            inv.AddRange(gear);
            p.EquipmentList = inv;
        }

        private void CommitRankUp(ref BattlePlayerDataMpo p, BattleUnitDataMpo incoming, List<BattleUnitDataMpo> materials, out BattleUnitDataMpo survivor, bool deckOnly = false)
        {

            var template = materials[0];
            var consumeKeys = new HashSet<int>();
            foreach (var m in materials) consumeKeys.Add(m.UniqueKey);
            consumeKeys.Add(incoming.UniqueKey);

            var result = BattleMpoFactory.MakeUnit(template.UniqueKey, template.UnitId, template.PositionX, template.PositionY, template.State, template.Index);
            result.Rank = template.Rank + 1;
            result.IsPurchased = true;

            var mergedGear = new List<BattleEquipmentDataMpo>();
            if (template.EquipmentList != null) mergedGear.AddRange(template.EquipmentList);
            var gearCounted = new HashSet<int> { template.UniqueKey };
            foreach (var u in (p.FieldUnitList ?? new List<BattleUnitDataMpo>()).Concat(p.DeckUnitList ?? new List<BattleUnitDataMpo>()))
            {
                if (consumeKeys.Contains(u.UniqueKey) && gearCounted.Add(u.UniqueKey) && u.EquipmentList != null)
                    mergedGear.AddRange(u.EquipmentList);
            }
            if (gearCounted.Add(incoming.UniqueKey) && incoming.EquipmentList != null) mergedGear.AddRange(incoming.EquipmentList);
            CarryMergedGear(ref p, template.UnitId, mergedGear);
            result.EquipmentList = mergedGear;
            if (template.State != UnitState.Field)
            {
                result.State = UnitState.Deck;
                result.PositionX = 0;
                result.PositionY = 0;
                result.Index = (p.DeckUnitList ?? new List<BattleUnitDataMpo>()).Count;
            }

            p.FieldUnitList?.RemoveAll(u => consumeKeys.Contains(u.UniqueKey));
            p.DeckUnitList?.RemoveAll(u => consumeKeys.Contains(u.UniqueKey));
            if (result.State == UnitState.Field)
            {
                p.FieldUnitList = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
                p.FieldUnitList.Add(result);
                ReindexField(ref p);
            }
            else
            {
                p.DeckUnitList = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
                p.DeckUnitList.Add(result);
                ReindexDeck(ref p);
            }

            while (result.Rank < 3)
            {

                var next = PlanRankUp(ref p, new BattleUnitDataMpo { UnitId = result.UnitId, Rank = result.Rank }, out _, deckOnly, result.UniqueKey);
                if (next == null) break;
                var nextKeys = new HashSet<int>();
                foreach (var m in next) nextKeys.Add(m.UniqueKey);
                var keep = next[0];
                var prevKey = result.UniqueKey;

                var all = (p.FieldUnitList ?? new List<BattleUnitDataMpo>()).Concat(p.DeckUnitList ?? new List<BattleUnitDataMpo>()).ToList();
                var prevUnit = all.FirstOrDefault(u => u.UniqueKey == prevKey);

                var anchor = keep.State != UnitState.Field && prevUnit != null && prevUnit.State == UnitState.Field
                    ? prevUnit : keep;
                var chainGear = new List<BattleEquipmentDataMpo>();
                if (anchor.EquipmentList != null) chainGear.AddRange(anchor.EquipmentList);
                var chainCounted = new HashSet<int> { anchor.UniqueKey };
                foreach (var u in all)
                {
                    if (!(nextKeys.Contains(u.UniqueKey) || u.UniqueKey == prevKey)) continue;
                    if (chainCounted.Add(u.UniqueKey) && u.EquipmentList != null) chainGear.AddRange(u.EquipmentList);
                }
                p.FieldUnitList?.RemoveAll(u => nextKeys.Contains(u.UniqueKey) || u.UniqueKey == prevKey);
                p.DeckUnitList?.RemoveAll(u => nextKeys.Contains(u.UniqueKey) || u.UniqueKey == prevKey);
                CarryMergedGear(ref p, anchor.UnitId, chainGear);
                var chained = BattleMpoFactory.MakeUnit(anchor.UniqueKey, anchor.UnitId,
                    anchor.State == UnitState.Field ? anchor.PositionX : 0,
                    anchor.State == UnitState.Field ? anchor.PositionY : 0,
                    anchor.State,
                    anchor.State == UnitState.Field ? anchor.Index : (p.DeckUnitList ?? new List<BattleUnitDataMpo>()).Count);
                chained.Rank = anchor.Rank + 1;
                chained.IsPurchased = true;
                chained.EquipmentList = chainGear;
                if (chained.State == UnitState.Field)
                {
                    p.FieldUnitList = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
                    p.FieldUnitList.Add(chained);
                    ReindexField(ref p);
                }
                else
                {
                    p.DeckUnitList = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
                    p.DeckUnitList.Add(chained);
                    ReindexDeck(ref p);
                }
                result = chained;
            }
            survivor = result;
        }

        public ShopOpResult RankUpOwnedUnitOp(int viewerId, int unitKey, bool withEquipment)
        {
            var res = new ShopOpResult { UniqueKey = unitKey };
            if (!CanEditPreparation(viewerId)) { res.Reason = "rank_up_unavailable"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];
            var field = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
            var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            var survivor = field.Find(u => u.UniqueKey == unitKey) ?? deck.Find(u => u.UniqueKey == unitKey);
            if (survivor == null) { res.Reason = "rank_up_unavailable"; return res; }
            if (survivor.Rank < 1 || survivor.Rank > 2 || survivor.IsToken || survivor.IsHelp)
            { res.Reason = "unit_not_eligible"; return res; }
            var survivorHasEquip = survivor.EquipmentList != null && survivor.EquipmentList.Count > 0;
            if (survivorHasEquip && !withEquipment) { res.Reason = "equipment_requires_with_equipment"; return res; }

            var materials = new List<BattleUnitDataMpo>();
            foreach (var u in field)
            {
                if (u.UniqueKey == unitKey) continue;
                if (u.UnitId != survivor.UnitId || u.Rank != survivor.Rank) continue;
                if (u.IsToken || u.IsHelp) continue;
                if (!withEquipment && u.EquipmentList != null && u.EquipmentList.Count > 0) continue;
                materials.Add(u);
            }
            foreach (var u in deck)
            {
                if (u.UniqueKey == unitKey) continue;
                if (u.UnitId != survivor.UnitId || u.Rank != survivor.Rank) continue;
                if (u.IsToken || u.IsHelp) continue;
                if (!withEquipment && u.EquipmentList != null && u.EquipmentList.Count > 0) continue;
                materials.Add(u);
            }
            if (materials.Count < 2) { res.Reason = "three_matching_copies_required"; return res; }

            var consume = new HashSet<int> { unitKey, materials[0].UniqueKey, materials[1].UniqueKey };

            var mergedGear = new List<BattleEquipmentDataMpo>();
            if (survivor.EquipmentList != null) mergedGear.AddRange(survivor.EquipmentList);
            if (withEquipment)
            {
                foreach (var m in new[] { materials[0], materials[1] })
                {
                    if (m.EquipmentList != null) mergedGear.AddRange(m.EquipmentList);
                }
            }
            CarryMergedGear(ref p, survivor.UnitId, mergedGear);

            var result = BattleMpoFactory.MakeUnit(survivor.UniqueKey, survivor.UnitId,
                survivor.PositionX, survivor.PositionY, survivor.State, survivor.Index);
            result.Rank = survivor.Rank + 1;
            result.IsPurchased = true;
            result.EquipmentList = mergedGear;
            field.RemoveAll(u => consume.Contains(u.UniqueKey));
            deck.RemoveAll(u => consume.Contains(u.UniqueKey));
            if (result.State == UnitState.Field)
            {
                field.Add(result);
            }
            else
            {
                result.PositionX = 0;
                result.PositionY = 0;
                result.Index = deck.Count;
                deck.Add(result);
            }
            p.FieldUnitList = field;
            p.DeckUnitList = deck;
            ReindexField(ref p);
            ReindexDeck(ref p);
            Players[idx] = p;
            RefreshSynergies(idx);
            res.Ok = true;
            res.DidRankUp = true;
            res.RankUppedUnit = result;
            return res;
        }

        private static int FindUnitIndex(List<BattleUnitDataMpo> units, int uniqueKey)
        {
            if (units == null) return -1;
            for (int i = 0; i < units.Count; i++) if (units[i].UniqueKey == uniqueKey) return i;
            return -1;
        }

        private static BattleEquipmentDataMpo FindGear(BattlePlayerDataMpo p, int equipmentKey)
        {
            var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
            foreach (var e in inv) if (e.UniqueKey == equipmentKey) return e;
            foreach (var u in (p.FieldUnitList ?? new List<BattleUnitDataMpo>()).Concat(p.DeckUnitList ?? new List<BattleUnitDataMpo>()))
            {
                if (u.EquipmentList == null) continue;
                foreach (var e in u.EquipmentList) if (e.UniqueKey == equipmentKey) return e;
            }
            return null;
        }

        private static bool ConsumeGear(ref BattlePlayerDataMpo p, int equipmentKey)
        {
            var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
            var gi = inv.FindIndex(e => e.UniqueKey == equipmentKey);
            if (gi >= 0) { inv.RemoveAt(gi); p.EquipmentList = inv; return true; }
            foreach (var u in (p.FieldUnitList ?? new List<BattleUnitDataMpo>()).Concat(p.DeckUnitList ?? new List<BattleUnitDataMpo>()))
            {
                if (u.EquipmentList == null) continue;
                var ui = u.EquipmentList.FindIndex(e => e.UniqueKey == equipmentKey);
                if (ui >= 0) { u.EquipmentList.RemoveAt(ui); return true; }
            }
            return false;
        }

        public const int MaxUnitEquipment = 3;

        public ShopOpResult EquipmentToUnitOp(int viewerId, int unitKey, int equipmentKey)
        {
            var res = new ShopOpResult { UniqueKey = unitKey };
            if (!CanTrade(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];
            var field = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
            var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            var unitIdx = FindUnitIndex(field, unitKey);
            var isField = unitIdx >= 0;
            if (!isField) unitIdx = FindUnitIndex(deck, unitKey);
            if (unitIdx < 0) { res.Reason = "unit_not_owned"; return res; }
            var unit = isField ? field[unitIdx] : deck[unitIdx];

            var badge = FindGear(p, equipmentKey);
            if (badge != null && badge.Type == EquipmentType.UnitCopy)
            {
                if (unit.IsToken || unit.IsHelp) { res.Reason = "unit_not_eligible"; return res; }
                var deckOnly = !CanEditPreparation(viewerId);
                p.FieldUnitList = field;
                p.DeckUnitList = deck;
                var copy = BattleMpoFactory.MakeUnit(_nextShopUnitKey++, unit.UnitId, 0, 0, UnitState.Deck, deck.Count);
                copy.IsPurchased = true;
                var badgePlan = PlanRankUp(ref p, copy, out _, deckOnly);

                if (badgePlan == null && deck.Count >= DeckCapacity) { res.Reason = "deck_full"; return res; }
                if (!ConsumeGear(ref p, equipmentKey)) { res.Reason = "equipment_not_owned"; return res; }
                if (badgePlan != null)
                {
                    CommitRankUp(ref p, copy, badgePlan, out var survivor, deckOnly);
                    res.DidRankUp = true;
                    res.RankUppedUnit = survivor;
                }
                else
                {
                    deck.Add(copy);
                    ReindexDeck(ref p);
                    res.RankUppedUnit = copy;
                }
                Players[idx] = p;
                RefreshSynergies(idx);
                res.Ok = true;
                Console.WriteLine($"[BATTLE] unit-copy badge viewer={viewerId} unit={unit.UnitId} key={copy.UniqueKey} rankUp={res.DidRankUp} deck={(p.DeckUnitList?.Count ?? 0)}");
                return res;
            }

            if (unit.EquipmentList == null) unit.EquipmentList = new List<BattleEquipmentDataMpo>();
            if (unit.EquipmentList.Any(e => e.UniqueKey == equipmentKey)) { res.Reason = "equipment_already_attached"; return res; }

            if (unit.EquipmentList.Count >= MaxUnitEquipment) { res.Reason = "equipment_slot_full"; return res; }

            var gear = badge;
            if (gear == null) { res.Reason = "equipment_not_owned"; return res; }

            if (ExceedsSynergyCap(unit, gear)) { res.Reason = "synergy_cap"; return res; }
            ConsumeGear(ref p, equipmentKey);
            unit.EquipmentList.Add(gear);
            if (isField) field[unitIdx] = unit; else deck[unitIdx] = unit;
            p.FieldUnitList = field;
            p.DeckUnitList = deck;
            Players[idx] = p;
            res.Ok = true;
            res.RankUppedUnit = unit;
            RefreshSynergies(idx);
            return res;
        }

        public ShopOpResult RemoveEquipmentFromUnitOp(int viewerId, int unitKey, int[] equipmentKeys, bool removeAll)
        {
            var res = new ShopOpResult { UniqueKey = unitKey };
            if (!CanTrade(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];
            var field = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
            var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            var unitIdx = FindUnitIndex(field, unitKey);
            var isField = unitIdx >= 0;
            if (!isField) unitIdx = FindUnitIndex(deck, unitKey);
            if (unitIdx < 0) { res.Reason = "unit_not_owned"; return res; }
            var unit = isField ? field[unitIdx] : deck[unitIdx];
            var gear = unit.EquipmentList ?? new List<BattleEquipmentDataMpo>();
            if (gear.Count == 0) { res.Reason = "unit_has_no_equipment"; return res; }
            List<BattleEquipmentDataMpo> removed;
            if (removeAll) { removed = new List<BattleEquipmentDataMpo>(gear); gear.Clear(); }
            else
            {
                var keys = new HashSet<int>(equipmentKeys ?? Array.Empty<int>());
                if (keys.Count == 0) { res.Reason = "no_keys"; return res; }
                removed = gear.Where(e => keys.Contains(e.UniqueKey)).ToList();
                if (removed.Count == 0) { res.Reason = "equipment_not_on_unit"; return res; }
                gear.RemoveAll(e => keys.Contains(e.UniqueKey));
            }
            unit.EquipmentList = gear;
            if (isField) field[unitIdx] = unit; else deck[unitIdx] = unit;
            var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
            inv.AddRange(removed);
            p.EquipmentList = inv;
            p.FieldUnitList = field;
            p.DeckUnitList = deck;
            Players[idx] = p;
            res.Ok = true;
            res.RankUppedUnit = unit;
            RefreshSynergies(idx);
            return res;
        }

        public ShopOpResult MigrateEquipmentOp(int viewerId, int fromUnitKey, int toUnitKey, int equipmentKey)
        {
            var res = new ShopOpResult { UniqueKey = toUnitKey };
            if (!CanTrade(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            if (fromUnitKey == toUnitKey) { res.Reason = "source_equals_target"; return res; }
            var idx = PlayerIndex(viewerId);
            var p = Players[idx];
            var field = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
            var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            var fromIdx = FindUnitIndex(field, fromUnitKey);
            var fromIsField = fromIdx >= 0;
            if (!fromIsField) fromIdx = FindUnitIndex(deck, fromUnitKey);
            var toIdx = FindUnitIndex(field, toUnitKey);
            var toIsField = toIdx >= 0;
            if (!toIsField) toIdx = FindUnitIndex(deck, toUnitKey);
            if (fromIdx < 0 || toIdx < 0) { res.Reason = "unit_not_owned"; return res; }
            var from = fromIsField ? field[fromIdx] : deck[fromIdx];
            var to = toIsField ? field[toIdx] : deck[toIdx];

            if ((to.EquipmentList?.Count ?? 0) >= MaxUnitEquipment) { res.Reason = "equipment_slot_full"; return res; }
            var gear = from.EquipmentList ?? new List<BattleEquipmentDataMpo>();
            var gi = gear.FindIndex(e => e.UniqueKey == equipmentKey);
            if (gi < 0) { res.Reason = "equipment_not_on_source"; return res; }
            var moved = gear[gi];
            gear.RemoveAt(gi);
            from.EquipmentList = gear;
            if (to.EquipmentList == null) to.EquipmentList = new List<BattleEquipmentDataMpo>();
            to.EquipmentList.Add(moved);
            if (fromIsField) field[fromIdx] = from; else deck[fromIdx] = from;
            if (toIsField) field[toIdx] = to; else deck[toIdx] = to;
            p.FieldUnitList = field;
            p.DeckUnitList = deck;
            Players[idx] = p;
            res.Ok = true;
            res.RankUppedUnit = to;
            res.UniqueKey = fromUnitKey;
            RefreshSynergies(idx);
            return res;
        }

        private int _nextEquipmentKey = 400000;

        private readonly Dictionary<int, List<BattleEquipmentDataMpo>> _pendingDropEquipment = new Dictionary<int, List<BattleEquipmentDataMpo>>();
        private readonly Dictionary<int, Dictionary<int, List<BattleEquipmentDataMpo>>> _pendingLotteryEquipment = new Dictionary<int, Dictionary<int, List<BattleEquipmentDataMpo>>>();

        private sealed class DropPlan
        {
            public int DeadKey;
            public float At;
            public int MatchId;
            public NpcBoxDropResultMpo[] Boxes;
            public List<BattleEquipmentDataMpo> Gears = new List<BattleEquipmentDataMpo>();
            public int Gold;
        }
        private readonly Dictionary<int, List<DropPlan>> _roundDropPlans = new Dictionary<int, List<DropPlan>>();

        public ShopOpResult DebugAddEquipmentOp(int viewerId, int equipmentId)
        {
            var res = new ShopOpResult();
            var info = BattleMaster.GetEquipment(equipmentId);
            if (info == null) { res.Reason = "equipment_master_missing"; return res; }
            var idx = PlayerIndex(viewerId);
            if (idx < 0) { res.Reason = "player_not_found"; return res; }
            var p = Players[idx];
            var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
            inv.Add(BattleMpoFactory.MakeEquipment(_nextEquipmentKey++, equipmentId));
            p.EquipmentList = inv;
            Players[idx] = p;
            res.Ok = true;
            res.UniqueKey = _nextEquipmentKey - 1;
            return res;
        }

        public ShopOpResult DebugAddUnitOp(int viewerId, int unitId, int rank)
        {
            var res = new ShopOpResult();
            var info = BattleMaster.GetUnit(unitId);
            if (info == null) { res.Reason = "unit_master_missing"; return res; }
            var idx = PlayerIndex(viewerId);
            if (idx < 0) { res.Reason = "player_not_found"; return res; }
            var p = Players[idx];
            var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            if (deck.Count >= DeckCapacity) { res.Reason = "deck_full"; return res; }
            var key = _nextShopUnitKey++;
            var unit = BattleMpoFactory.MakeUnit(key, unitId, 0, 0, UnitState.Deck, deck.Count);
            unit.Rank = Math.Max(1, Math.Min(3, rank));
            unit.IsPurchased = true;
            deck.Add(unit);
            p.DeckUnitList = deck;
            ReindexDeck(ref p);
            Players[idx] = p;
            res.Ok = true;
            res.UniqueKey = key;
            res.RankUppedUnit = unit;
            return res;
        }

        public ShopOpResult DisassembleEquipmentOp(int viewerId, int[] equipmentKeys)
        {
            return DisassembleEquipmentCore(viewerId, 0, equipmentKeys, false);
        }

        public ShopOpResult DisassembleEquipmentWithEquippedOp(int viewerId, int unitKey, int[] equipmentKeys)
        {
            return DisassembleEquipmentCore(viewerId, unitKey, equipmentKeys, true);
        }

        private ShopOpResult DisassembleEquipmentCore(int viewerId, int unitKey, int[] equipmentKeys, bool equipped)
        {
            var res = new ShopOpResult { UniqueKey = unitKey };
            if (!CanEditPreparation(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var keys = new HashSet<int>(equipmentKeys ?? Array.Empty<int>());
            if (keys.Count == 0) { res.Reason = "no_keys"; return res; }
            var idx = PlayerIndex(viewerId);
            if (idx < 0) { res.Reason = "player_not_found"; return res; }
            var p = Players[idx];
            BattleUnitDataMpo unit = null;
            List<BattleEquipmentDataMpo> source;
            if (equipped)
            {
                var field = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
                var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
                var ui = FindUnitIndex(field, unitKey);
                var isField = ui >= 0;
                if (!isField) ui = FindUnitIndex(deck, unitKey);
                if (ui < 0) { res.Reason = "unit_not_owned"; return res; }
                unit = isField ? field[ui] : deck[ui];
                source = unit.EquipmentList ?? new List<BattleEquipmentDataMpo>();
            }
            else
            {
                source = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
            }

            var owned = source.Where(e => keys.Contains(e.UniqueKey)).ToList();
            if (owned.Count != keys.Count) { res.Reason = "equipment_not_owned"; return res; }

            int gained = 0;
            foreach (var g in owned)
            {
                var info = BattleMaster.GetEquipment(g.Id);
                var rank = info?.Rank ?? 0;
                if (BattleMaster.TryGetDisassembleToken(rank, out var token)) gained += token;
                else Console.WriteLine($"[BATTLE] disassemble no master rule -> 0 viewer={viewerId} equipId={g.Id} rank={rank} (disassemble_equipment_data 2.0.0 has Tier1..3 only)");
            }

            source.RemoveAll(e => keys.Contains(e.UniqueKey));
            if (equipped) unit.EquipmentList = source;
            else p.EquipmentList = source;

            p.RankUpToken += gained;
            Players[idx] = p;
            res.Ok = true;
            res.TokenNum = gained;
            res.RankUppedUnit = unit;

            if (equipped) RefreshSynergies(idx);
            return res;
        }

        public ShopOpResult SelectEquipmentOp(int viewerId, int equipmentKey, int selectEquipmentId, int unitKey)
        {
            var res = new ShopOpResult { UniqueKey = unitKey };
            if (!CanEditPreparation(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            if (idx < 0) { res.Reason = "player_not_found"; return res; }
            var p = Players[idx];
            var field = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
            var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();

            List<BattleEquipmentDataMpo> holder = null;
            BattleUnitDataMpo ownerUnit = null;
            if (unitKey > 0)
            {
                var ui = FindUnitIndex(field, unitKey);
                var isField = ui >= 0;
                if (!isField) ui = FindUnitIndex(deck, unitKey);
                if (ui < 0) { res.Reason = "unit_not_owned"; return res; }
                ownerUnit = isField ? field[ui] : deck[ui];
                holder = ownerUnit.EquipmentList ?? new List<BattleEquipmentDataMpo>();
                if (!holder.Any(e => e.UniqueKey == equipmentKey)) { res.Reason = "equipment_not_on_unit"; return res; }
            }
            else
            {
                if (inv.Any(e => e.UniqueKey == equipmentKey)) holder = inv;
                else
                {
                    foreach (var u in field.Concat(deck))
                    {
                        if (u.EquipmentList != null && u.EquipmentList.Any(e => e.UniqueKey == equipmentKey))
                        {
                            holder = u.EquipmentList;
                            ownerUnit = u;
                            break;
                        }
                    }
                    if (holder == null) { res.Reason = "equipment_not_owned"; return res; }
                }
            }

            var gi = holder.FindIndex(e => e.UniqueKey == equipmentKey);
            if (gi < 0) { res.Reason = "equipment_not_owned"; return res; }
            var gear = holder[gi];
            var config = BattleMaster.GetEquipment(gear.Id);
            if (config == null) { res.Reason = "equipment_master_missing"; return res; }
            var selectable = config.SelectableEquipmentIds ?? Array.Empty<int>();
            if (selectEquipmentId <= 0 || Array.IndexOf(selectable, selectEquipmentId) < 0)
            {
                res.Reason = "selected_equipment_not_allowed";
                return res;
            }
            if (BattleMaster.GetEquipment(selectEquipmentId) == null)
            {
                res.Reason = "selected_equipment_master_missing";
                return res;
            }

            holder[gi] = BattleMpoFactory.MakeEquipment(equipmentKey, selectEquipmentId);
            if (ownerUnit == null) p.EquipmentList = holder;
            else ownerUnit.EquipmentList = holder;
            Players[idx] = p;

            RefreshSynergies(idx);
            res.Ok = true;
            res.RankUppedUnit = ownerUnit;
            return res;
        }

        public ShopOpResult SelectEquipmentAndEquipToUnitOp(int viewerId, int equipmentKey, int selectEquipmentId, int targetUnitKey)
        {
            var res = new ShopOpResult { UniqueKey = targetUnitKey };
            if (!CanEditPreparation(viewerId)) { res.Reason = "phase_not_preparing"; return res; }
            var idx = PlayerIndex(viewerId);
            if (idx < 0) { res.Reason = "player_not_found"; return res; }
            var p = Players[idx];
            var field = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
            var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            var ui = FindUnitIndex(field, targetUnitKey);
            var isField = ui >= 0;
            if (!isField) ui = FindUnitIndex(deck, targetUnitKey);
            if (ui < 0) { res.Reason = "unit_not_owned"; return res; }
            var target = isField ? field[ui] : deck[ui];

            var sel = SelectEquipmentOp(viewerId, equipmentKey, selectEquipmentId, 0);
            if (!sel.Ok) return sel;

            if (target.EquipmentList != null && target.EquipmentList.Any(e => e.UniqueKey == equipmentKey))
            {

                res.Ok = true;
                res.RankUppedUnit = target;
                return res;
            }

            var eq = EquipmentToUnitOp(viewerId, targetUnitKey, equipmentKey);
            if (!eq.Ok)
            {
                Console.WriteLine($"[BATTLE] select+equip: transform landed but equip rejected viewer={viewerId} gear={equipmentKey} unit={targetUnitKey} reason={eq.Reason}");
                return eq;
            }
            res.Ok = true;
            res.RankUppedUnit = eq.RankUppedUnit;
            return res;
        }

        public ShopOpResult ReceiveDropEquipmentOp(int viewerId, int[] uniqueKeys)
        {
            var res = new ShopOpResult();

            int dropModeBits = (int)BattleOption.DropBoost;
            if ((dropModeBits & 4) == 0) { res.Reason = "drop_mode_disabled"; return res; }
            if (Finished) { res.Reason = "battle_finished"; return res; }
            var idx = PlayerIndex(viewerId);
            if (idx < 0) { res.Reason = "player_not_found"; return res; }
            if (!_pendingDropEquipment.TryGetValue(viewerId, out var pending) || pending.Count == 0)
            {
                res.Reason = "no_pending_drop_equipment";
                return res;
            }
            res = ClaimDropEquipment(idx, pending, uniqueKeys, res);
            if (res.Ok && pending.Count == 0) _pendingDropEquipment.Remove(viewerId);
            return res;
        }

        private ShopOpResult ClaimDropEquipment(int playerIdx, List<BattleEquipmentDataMpo> pending, int[] uniqueKeys, ShopOpResult res)
        {
            var requested = uniqueKeys ?? Array.Empty<int>();
            var keys = new HashSet<int>(requested);
            if (keys.Count == 0) { res.Reason = "no_keys"; return res; }
            if (keys.Count != requested.Length) { res.Reason = "duplicate_requested_key"; return res; }
            foreach (var k in keys)
            {
                if (!pending.Any(x => x.UniqueKey == k)) { res.Reason = "equipment_not_pending"; return res; }
            }
            var received = pending.Where(x => keys.Contains(x.UniqueKey)).ToList();
            pending.RemoveAll(x => keys.Contains(x.UniqueKey));
            res.Ok = true;
            res.ReceivedEquipments = received.ToArray();
            return res;
        }

        public ShopOpResult ReceiveLotteryEquipmentOp(int viewerId, int stockIndex, int[] uniqueKeys)
        {
            var res = new ShopOpResult { UniqueKey = stockIndex };
            if (Finished) { res.Reason = "battle_finished"; return res; }
            var idx = PlayerIndex(viewerId);
            if (idx < 0) { res.Reason = "player_not_found"; return res; }
            if (!_pendingLotteryEquipment.TryGetValue(viewerId, out var stocks)
                || !stocks.TryGetValue(stockIndex, out var pending) || pending.Count == 0)
            {
                res.Reason = "lottery_stock_not_found";
                return res;
            }
            res = ClaimPendingEquipment(idx, pending, uniqueKeys, res);
            if (res.Ok)
            {
                if (pending.Count == 0) stocks.Remove(stockIndex);
                if (stocks.Count == 0) _pendingLotteryEquipment.Remove(viewerId);
            }
            return res;
        }

        private ShopOpResult ClaimPendingEquipment(int playerIdx, List<BattleEquipmentDataMpo> pending, int[] uniqueKeys, ShopOpResult res)
        {
            var requested = uniqueKeys ?? Array.Empty<int>();
            var keys = new HashSet<int>(requested);
            if (keys.Count == 0) { res.Reason = "no_keys"; return res; }
            if (keys.Count != requested.Length) { res.Reason = "duplicate_requested_key"; return res; }
            foreach (var k in keys)
            {
                if (!pending.Any(x => x.UniqueKey == k)) { res.Reason = "equipment_not_pending"; return res; }
            }

            var p = Players[playerIdx];
            var ownedKeys = new HashSet<int>();
            foreach (var e in p.EquipmentList ?? new List<BattleEquipmentDataMpo>()) ownedKeys.Add(e.UniqueKey);
            foreach (var u in (p.FieldUnitList ?? new List<BattleUnitDataMpo>()).Concat(p.DeckUnitList ?? new List<BattleUnitDataMpo>()))
            {
                if (u.EquipmentList == null) continue;
                foreach (var e in u.EquipmentList) ownedKeys.Add(e.UniqueKey);
            }
            foreach (var k in keys)
            {
                if (ownedKeys.Contains(k)) { res.Reason = "equipment_already_owned"; return res; }
            }

            var received = pending.Where(x => keys.Contains(x.UniqueKey)).ToList();
            var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
            inv.AddRange(received);
            p.EquipmentList = inv;
            Players[playerIdx] = p;
            pending.RemoveAll(x => keys.Contains(x.UniqueKey));
            res.Ok = true;
            res.ReceivedEquipments = received.ToArray();
            return res;
        }

        private void PlanRoundDrops(BattlePlayerDataMpo p, List<(float at, BattleBoardEventResponse ev)> simEvents,
            BattleUnitDataMpo[] npcUnits, int matchId, int round, List<(float at, BattleBoardEventResponse ev)> axis)
        {
            int playerIdx = PlayerIndex(p.ViewerId);
            if (playerIdx < 0) return;
            var npcKeys = new HashSet<int>();
            foreach (var u in npcUnits) npcKeys.Add(u.UniqueKey);
            var deaths = new List<(int key, float at)>();
            var seen = new HashSet<int>();
            foreach (var (at, ev) in simEvents)
            {
                int deadKey = 0;
                if (ev is BattleBoardAttackOperateEventResponse a)
                {
                    if (a.IsDead) deadKey = a.TargetUniqueKey;
                }
                else if (ev is BattleBoardSkillDamageEventResponse s)
                {
                    if (s.IsDead) deadKey = s.TargetUniqueKey;
                }
                if (deadKey != 0 && npcKeys.Contains(deadKey) && seen.Add(deadKey)) deaths.Add((deadKey, at));
            }
            if (deaths.Count == 0) return;
            deaths.Sort((x, y) =>
            {
                int c = x.at.CompareTo(y.at);
                return c != 0 ? c : x.key.CompareTo(y.key);
            });

            int encounter = NpcEncounterIndex(round);
            if (!_roundDropPlans.TryGetValue(p.ViewerId, out var plans))
            {
                plans = new List<DropPlan>();
                _roundDropPlans[p.ViewerId] = plans;
            }
            foreach (var (deadKey, at) in deaths)
            {
                var drop = BattleMaster.ChooseItemDrop(encounter, _rng);
                if (drop == null) break;
                var plan = new DropPlan { DeadKey = deadKey, At = at, MatchId = matchId };
                NpcBoxDropResultMpo box;
                if (drop.ItemId == 201010)
                {
                    int pick = _rng.Next(4);
                    if (pick == 0)
                    {
                        plan.Gold = 3 * drop.RewardNumber;
                        box = new NpcBoxDropResultMpo
                        {
                            Gold = plan.Gold,
                            Units = Array.Empty<BattleUnitDataMpo>(),
                            Equipments = Array.Empty<BattleEquipmentDataMpo>(),
                            RankUpToken = null,
                            BoxGrade = 1,
                        };
                    }
                    else
                    {
                        int specialId = pick == 1 ? 600110 : pick == 2 ? 600210 : 500110;
                        var config = BattleMaster.GetEquipment(specialId);
                        if (config == null)
                        {
                            Console.WriteLine($"[BATTLE] drop special missing id={specialId} (201010 box) viewer={p.ViewerId}");
                            continue;
                        }
                        for (int r = 0; r < drop.RewardNumber; r++)
                        {
                            plan.Gears.Add(BattleMpoFactory.MakeEquipment(_nextEquipmentKey++, specialId));
                        }
                        box = new NpcBoxDropResultMpo
                        {
                            Gold = null,
                            Units = Array.Empty<BattleUnitDataMpo>(),
                            Equipments = plan.Gears.ToArray(),
                            RankUpToken = null,
                            BoxGrade = config.Rank > 0 ? config.Rank : 1,
                        };
                    }
                }
                else
                {
                    for (int r = 0; r < drop.RewardNumber; r++)
                    {
                        var choices = new List<int>(2);
                        int c1 = drop.ItemId + 100000;
                        int c2 = drop.ItemId + 100010;
                        if (BattleMaster.GetEquipment(c1) != null) choices.Add(c1);
                        if (BattleMaster.GetEquipment(c2) != null) choices.Add(c2);
                        if (choices.Count == 0)
                        {
                            Console.WriteLine($"[BATTLE] drop category unresolved ItemId={drop.ItemId} viewer={p.ViewerId}");
                            break;
                        }
                        plan.Gears.Add(BattleMpoFactory.MakeEquipment(_nextEquipmentKey++, choices[_rng.Next(choices.Count)]));
                    }
                    if (plan.Gears.Count == 0) continue;
                    int maxRank = 1;
                    foreach (var g in plan.Gears)
                    {
                        if (g.Rank > maxRank) maxRank = g.Rank;
                    }
                    box = new NpcBoxDropResultMpo
                    {
                        Gold = null,
                        Units = Array.Empty<BattleUnitDataMpo>(),
                        Equipments = plan.Gears.ToArray(),
                        RankUpToken = null,
                        BoxGrade = maxRank,
                    };
                }
                plan.Boxes = new[] { box };
                plans.Add(plan);
                axis.Add((at, new BattleBoardBoxDropEventResponse(matchId, at, deadKey, plan.Boxes)));
                _roundDropsEnqueued += 1;
            }
            Console.WriteLine($"[BATTLE] drop plan viewer={p.ViewerId} round={round} match={matchId} encounter={encounter} deaths={deaths.Count} boxes={plans.Count} poolBoxes={_roundDropsEnqueued}");
        }

        private void CommitDropPlan(int viewerId)
        {
            int playerIdx = PlayerIndex(viewerId);
            if (playerIdx < 0) return;
            int encounter = NpcEncounterIndex(Round);
            var p = Players[playerIdx];
            int gearCount = 0;
            int goldGained = 0;
            int planCount = 0;
            if (_roundDropPlans.TryGetValue(viewerId, out var plans))
            {
                _roundDropPlans.Remove(viewerId);
                planCount = plans.Count;
                foreach (var plan in plans)
                {
                    goldGained += plan.Gold;
                    foreach (var gear in plan.Gears)
                    {
                        var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
                        inv.Add(gear);
                        p.EquipmentList = inv;
                        if (!_pendingDropEquipment.TryGetValue(viewerId, out var pending))
                        {
                            pending = new List<BattleEquipmentDataMpo>();
                            _pendingDropEquipment[viewerId] = pending;
                        }
                        pending.Add(gear);
                        gearCount++;
                    }
                }
            }

            if (goldGained > 0)
            {
                p.Gold += goldGained;
                p.TotalGold += goldGained;
            }
            Players[playerIdx] = p;

            var pool = BattleMaster.OrdinaryEquipmentIds;
            int lotteryId = 0;
            if (pool.Length > 0)
            {
                lotteryId = pool[_rng.Next(pool.Length)];
                var lotteryGear = BattleMpoFactory.MakeEquipment(_nextEquipmentKey++, lotteryId);
                var inv = p.EquipmentList ?? new List<BattleEquipmentDataMpo>();
                inv.Add(lotteryGear);
                p.EquipmentList = inv;
                Players[playerIdx] = p;
            }

            Console.WriteLine($"[BATTLE] drop commit viewer={viewerId} round={Round} encounter={encounter} plans={planCount} gear={gearCount} gold={goldGained} lotteryEquip={lotteryId} poolTotal={_roundDropsEnqueued}");
        }

        private static int NpcEncounterIndex(int round)
        {
            int n = 0;
            for (int r = 1; r <= round; r++)
            {
                if (BattleMaster.NpcIdForRound(r) != 0) n++;
            }
            return n;
        }

        private static string ValidateFieldDestination(in BattlePlayerDataMpo p, int x, int y, int ignoreUniqueKey = 0)
        {
            if (x < 0 || x > 5) return "coordinate_out_of_range";
            if (y < 4 || y > 7) return "not_player_deploy_cell";
            foreach (var u in p.FieldUnitList ?? new List<BattleUnitDataMpo>())
            {
                if (u.UniqueKey == ignoreUniqueKey) continue;
                if (u.PositionX == x && u.PositionY == y) return "occupied_cell";
            }
            return null;
        }

        private static void ReindexDeck(ref BattlePlayerDataMpo p)
        {
            var deck = p.DeckUnitList ?? new List<BattleUnitDataMpo>();
            for (int i = 0; i < deck.Count; i++) deck[i].Index = i;
            p.DeckUnitList = deck;
        }

        private static void ReindexField(ref BattlePlayerDataMpo p)
        {
            var field = p.FieldUnitList ?? new List<BattleUnitDataMpo>();
            for (int i = 0; i < field.Count; i++) field[i].Index = i;
            p.FieldUnitList = field;
        }

        private void RefreshShopsForNewRound()
        {
            if (Round == 1) return;
            foreach (var vid in PlayerViewerIds)
            {
                var idx = PlayerIndex(vid);
                if (idx < 0) continue;
                var p = Players[idx];
                if (p.IsShopLock || !p.Alive) continue;
                p.ShopUnitList = RollShopUnits(vid, Math.Max(1, p.Level));
                Players[idx] = p;
                var snapshot = Players[idx];

                TryUnicastToViewer(vid, bc => bc.OnRefreshShopUnit(snapshot, null));
                Console.WriteLine($"[BATTLE] round {Round} shop regenerated viewer={vid} slots={snapshot.ShopUnitList?.Count ?? 0}");
            }
        }

        internal BattleResultInfoMpo[] BuildBattleResults(bool pickWinner = true)
        {

            var aliveCount = 0;
            foreach (var p in Players)
            {
                if (p.Life > 0) aliveCount++;
            }

            int? winnerId = null;
            var bestLife = int.MinValue;
            if (pickWinner)
            {
                foreach (var p in Players)
                {
                    if (p.Life > 0 && p.Life > bestLife)
                    {
                        bestLife = p.Life;
                        winnerId = p.ViewerId;
                    }
                }
            }

            var results = new List<BattleResultInfoMpo>();
            foreach (var p in Players)
            {
                var isWinner = winnerId.HasValue && p.ViewerId == winnerId.Value;

                var ranking = _placements.TryGetValue(p.ViewerId, out var placed)
                    ? placed
                    : (isWinner ? 1 : (p.Life <= 0 ? 8 : 0));
                results.Add(new BattleResultInfoMpo
                {
                    Ranking = ranking,
                    ViewerId = p.ViewerId,
                    PlayerName = p.Name,
                    TeamId = p.TeamId,
                    WinCount = p.TotalWinsCount,
                    LoseCount = p.TotalLoseCount,

                    LivedRoundCount = _eliminatedRound.TryGetValue(p.ViewerId, out var elimRound)
                        ? elimRound : Round,

                    KillCount = 0,
                    TimeOfSurvival = TimeSpan.Zero,
                    TakenDamageCount = Math.Max(0, p.MaxLife - p.Life),
                    Gold = p.Gold,
                    Rank = p.Level,
                    FieldUnitList = p.FieldUnitList ?? new List<BattleUnitDataMpo>(),
                    IsWin = isWinner,
                    TeamRanking = ranking,
                    TotalGold = p.TotalGold,

                    Synergies = BattleMaster.EvaluateFieldSynergies(p.FieldUnitList),

                    IsAbandoned = _abandonedViewers.Contains(p.ViewerId),
                });
            }

            var arr = results.ToArray();
            int leaderGold = arr.Length > 0 ? arr.Max(r => r.Gold) : 0;
            foreach (var r in arr)
                Console.WriteLine($"[BATTLE] results row viewer={r.ViewerId} rank={r.Ranking} lived={r.LivedRoundCount} wins={r.WinCount} gold={r.Gold} leader={leaderGold}");
            Console.WriteLine($"[BATTLE] results aliveCount={aliveCount} winnerId={(winnerId.HasValue ? winnerId.Value.ToString() : "none")}");
            return arr;
        }
    }

    public static class BattleRegistry
    {
        private static readonly ConcurrentDictionary<Ulid, BattleSession> Sessions = new ConcurrentDictionary<Ulid, BattleSession>();

        public static BattleSession[] Snapshot()
        {
            PruneStale();
            return Sessions.Values.ToArray();
        }

        public static int ActiveCount => Sessions.Values.Count(s => !s.Finished);

        public static (int Active, int Retained, int AttachedHumans) Stats()
        {
            var active = 0;
            var attached = 0;
            foreach (var s in Sessions.Values)
            {
                if (s.Finished) continue;
                active++;
                var vids = s.HumanViewerIds;
                foreach (var vid in vids)
                {
                    if (s.IsAttached(vid)) attached++;
                }
            }
            return (active, Sessions.Count, attached);
        }

        private static readonly TimeSpan FinishedRetention = TimeSpan.FromMinutes(30);
        private static void PruneStale()
        {
            var cutoff = DateTime.UtcNow - FinishedRetention;
            foreach (var kv in Sessions)
            {
                var s = kv.Value;
                var finished = s.Finished && s.FinishedAtUtc is { } t && t < cutoff;
                var neverStarted = !s.Finished && s.CreatedAtUtc < cutoff && !s.EverHadHumanAttached;
                if ((finished || neverStarted) && Sessions.TryRemove(kv.Key, out _))
                {
                    Console.WriteLine($"[BATTLE] session pruned {kv.Key} finished={s.Finished}" +
                        $" createdAt={s.CreatedAtUtc:o}");
                }
            }
        }

        public static readonly int[] AiAvatarPool = { 1100101, 1100201, 1100301, 1105801, 1105901, 1106001 };

        public static readonly string[] AiNames =
            { "Player1", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7" };

        public const int SeatCount = 8;

        public static int AiViewerId(int i) => 135112345 + i;
        public static int AiGrade(int i) => Math.Min(5 + i, 8);
        public static int AiAvatarId(int i) => AiAvatarPool[(i - 1) % AiAvatarPool.Length];

        public sealed class HumanEntrant
        {
            public int ViewerId;
            public string Name;
            public int AvatarId;
            public int Grade;
        }

        public static BattleSession CreateMatch(int dummyKindId, IReadOnlyList<HumanEntrant> humans)
        {
            if (humans == null || humans.Count == 0)
                throw new ArgumentException("match needs at least one human", nameof(humans));

            PruneStale();

            var ids = new int[SeatCount];
            var humanSeats = Math.Min(humans.Count, SeatCount);
            for (var i = 0; i < humanSeats; i++) ids[i] = humans[i].ViewerId;

            var session = new BattleSession(BattleRule.Normal, ids);
            session.SetDummyKindId(dummyKindId);
            session.DeclareHumanSeats(ids.Take(humanSeats).ToArray());

            for (var i = 0; i < humanSeats; i++)
            {
                var h = humans[i];
                session.Players.Add(MakePlayer(h.ViewerId, h.Name, h.AvatarId, h.Grade, i));
            }

            for (var i = humanSeats; i < SeatCount; i++)
            {
                ids[i] = AiViewerId(i);
                session.Players.Add(MakePlayer(ids[i], AiNames[(i - 1) % AiNames.Length],
                    AiAvatarId(i), AiGrade(i), i));
            }

            session.AssignAiKinds(session.Round);

            session.AssignAiMirrors();

            foreach (var p in session.Players)
            {
                session.Teams.Add(new BattleTeamDataMpo
                {
                    TeamId = p.TeamId,
                    Life = p.Life,

                    DefeatAt = DateTimeOffset.FromUnixTimeSeconds(0)
                });
            }

            Sessions[session.BattleId] = session;
            ServerMetrics.BattleCreated();
            Console.WriteLine($"[BATTLE] created {session.BattleId} rule=Normal dummyKind={dummyKindId} humans={humanSeats} ai={SeatCount - humanSeats} players={session.Players.Count}");
            return session;
        }

        public static BattleSession CreateSingleVsAi(int dummyKindId, int humanViewerId, string humanName, int humanAvatarId)
        {

            return CreateMatch(dummyKindId, new[]
            {
                new HumanEntrant
                {
                    ViewerId = humanViewerId, Name = humanName, AvatarId = humanAvatarId,
                    Grade = BattleMaster.GradeOf(AccountStore.BattlePointOf(humanViewerId)),
                }
            });
        }

        public static BattleSession CreateBossBattle(int viewerId, string name, int avatarId, BossBattleStore.DeckRec deck)
        {
            if (deck == null) throw new ArgumentNullException(nameof(deck));
            PruneStale();

            var session = new BattleSession(BattleRule.Boss, new[] { viewerId });
            session.DeclareHumanSeats(new[] { viewerId });

            var p = MakePlayer(viewerId, name, avatarId, BattleMaster.GradeOf(AccountStore.BattlePointOf(viewerId)), 0);

            p.Gold = 0;
            p.TotalGold = 0;
            p.IsShopPurchasable = false;
            p.Level = Math.Max(1, deck.Level);
            p.BoardId = BattleMaster.BossRow.BoardId;
            var field = new List<BattleUnitDataMpo>();
            for (var i = 0; i < deck.Units.Count; i++)
            {
                var du = deck.Units[i];
                var u = BattleMpoFactory.MakeUnit(du.Key, du.UnitId, du.X, du.Y, UnitState.Field, i, isNpc: false);
                u.Rank = Math.Max(1, du.Rank);

                u.EquipmentList = (du.Equips ?? Array.Empty<int>())
                    .Select(e => BattleMpoFactory.MakeEquipment(session.TakeNextEquipmentKey(), e))
                    .ToList();
                field.Add(u);
            }
            p.FieldUnitList = field;
            p.DeckUnitList = new List<BattleUnitDataMpo>();
            p.ShopUnitList = new List<BattleUnitDataMpo>();
            p.CalculatedStatus = BattleMpoFactory.MakeStatusSetForUnits(
                field, BattleMaster.EvaluateFieldSynergies(field), field, session.GuildBuffEffectsByUnitId);
            session.Players.Add(p);
            session.SeedBasePlayerStatus(p.CalculatedStatus);
            session.Teams.Add(new BattleTeamDataMpo
            {
                TeamId = p.TeamId,
                Life = p.Life,
                DefeatAt = DateTimeOffset.FromUnixTimeSeconds(0)
            });
            session.SetBossContext(new BattleSession.BossContext
            {
                DeckId = deck.Id,
                PoolHp = BossBattleStore.RemainHp(viewerId),
                MaxHp = BossBattleStore.MaxHp(),
            });
            Sessions[session.BattleId] = session;
            ServerMetrics.BattleCreated();
            Console.WriteLine($"[BATTLE] created {session.BattleId} rule=Boss viewer={viewerId} deck={deck.Id} units={field.Count} pool={session.Boss.PoolHp}/{session.Boss.MaxHp}");
            return session;
        }

        public static BattleSession? Get(Ulid battleId, int viewerId = 0)
        {
            PruneStale();
            if (Sessions.TryGetValue(battleId, out var s)) return s;
            Console.WriteLine($"[BATTLE] !! session not found: {battleId} viewer={viewerId}");

            BattleSession? mine = null, newest = null;
            foreach (var kv in Sessions)
            {
                if (kv.Value.Finished) continue;
                if (newest == null || kv.Key.CompareTo(newest.BattleId) > 0) newest = kv.Value;
                if (viewerId != 0 && kv.Value.IsHuman(viewerId) &&
                    (mine == null || kv.Key.CompareTo(mine.BattleId) > 0)) mine = kv.Value;
            }
            var picked = mine ?? newest;
            if (picked != null)
            {
                Console.WriteLine($"[BATTLE] fallback to existing session {picked.BattleId} (mine={mine != null})");
            }
            return picked;
        }

        public static BattleSession? FindActiveHumanSession(int viewerId, bool excludeBoss = false)
        {
            PruneStale();
            BattleSession? best = null;
            foreach (var kv in Sessions)
            {
                var s = kv.Value;
                if (s.Finished) continue;
                if (excludeBoss && s.Rule == BattleRule.Boss) continue;
                if (!s.IsHuman(viewerId)) continue;

                var human = s.FindPlayer(viewerId);
                if (human.ViewerId == 0 || human.Life <= 0 || !human.Alive) continue;
                if (best == null || s.BattleId.CompareTo(best.BattleId) > 0) best = s;
            }
            return best;
        }

        private static BattlePlayerDataMpo MakePlayer(int viewerId, string name, int avatarId, int grade, int index)
        {
            return new BattlePlayerDataMpo
            {
                ViewerId = viewerId,
                Name = name,

                TeamId = viewerId,
                AvatarId = avatarId,
                Grade = grade,
                BoardId = 1,
                Index = index,
                Level = 1,
                Exp = 0,

                MaxLife = 50,
                Life = 50,
                Alive = true,

                Gold = 3,
                IsShopPurchasable = true,
                RankUpToken = 0,
                EquipmentList = new List<BattleEquipmentDataMpo>(),
                FieldUnitList = new List<BattleUnitDataMpo>(),
                DeckUnitList = new List<BattleUnitDataMpo>(),
                ShopUnitList = new List<BattleUnitDataMpo>(),
                InitialDistributeUnitList = new List<BattleUnitDataMpo>(),
                TeamTransferUnit = null,
                TeamTakeOverUnit = null,
                WinsCount = 0,
                LosesCount = 0,
                IsShopLock = false,
                CalculatedStatus = new BattleUnitCalculatedStatusSetMpo
                {
                    Synergies = new BattleSynergyMpo[0],
                    UnitStatuses = new BattleUnitCalculatedStatusMpo[0]
                },
                RecommendPartyDataId = 0,
                ShopUnitLotteryRates = BattleSession.ProjectLotteryRates(1, 1),
                TotalWinsCount = 0,
                TotalLoseCount = 0,
                TotalGold = 3,
                UnitMirroringPairs = new BattleUnitMirroringPairDataMpo[0],
                OverallBuffList = new List<int>(),

                PartyPower = 0
            };
        }
    }
}
