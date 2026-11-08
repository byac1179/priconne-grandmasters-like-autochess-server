using System;
using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Hubs;
using Byac.ServerShared.Enums;
using Byac.ServerShared.Hub;
using Byac.ServerShared.MessagePackObjects;
using Byac.Services.Battle;

namespace Byac.Services
{
    public class TraceHubFilterAttribute : StreamingHubFilterAttribute
    {

        public static readonly bool TraceCallLog =
            Environment.GetEnvironmentVariable("BYAC_BH_FILTER") == "1";

        public override async ValueTask Invoke(StreamingHubContext context, Func<StreamingHubContext, ValueTask> next)
        {
            if (TraceCallLog) Console.WriteLine($"[BH-FILTER] {context.Path} enter");
            try
            {
                await next(context);
                if (TraceCallLog) Console.WriteLine($"[BH-FILTER] {context.Path} OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BH-FILTER] {context.Path} EXCEPTION {ex}");
                throw;
            }
        }
    }

    [TraceHubFilter]
    public class BattleHub : StreamingHubBase<IBattleHub, IBattleHubReceiver>, IBattleHub
    {
        private IGroup _group;
        private IGroup _own;
        private BattleSession _session;
        private int _viewerId;

        protected override ValueTask OnConnecting()
        {
            Console.WriteLine($"[BH] connecting ctx={Context.ContextId}");
            string? gzipAdv = null;
            foreach (var h in Context.CallContext.RequestHeaders)
            {
                Console.WriteLine($"[BH]   header {h.Key}={LogSafe.Header(h.Key, h.Value)}");
                if (h.Key.Equals("grpc-accept-encoding", StringComparison.OrdinalIgnoreCase)) gzipAdv = h.Value;
            }

            Console.WriteLine($"[BH]   gzip-judge ctx={Context.ContextId} grpc-accept-encoding={(string.IsNullOrEmpty(gzipAdv) ? "<absent> => server gzip DEAD for this client" : gzipAdv)}");
            return default;
        }

        protected override ValueTask OnDisconnected()
        {

            var bc15s = _session != null ? _session.BroadcastsInLastSeconds(15) : -1;
            Console.WriteLine($"[BH] disconnected viewer={_viewerId} session={_session?.BattleId} bc15s={(bc15s >= 0 ? bc15s.ToString() : "n/a")}");
            if (_session != null)
            {
                if (_viewerId != 0)
                {
                    if (bc15s >= 0) _session.RecordViewerDrop(_viewerId, bc15s);
                    _session.MarkHumanDisconnected(_viewerId, Context.ContextId);
                    _session.UnregisterChannel(_viewerId, _own);
                }
            }
            return default;
        }

        public async Task<BattleAttachResponse> AttachAsync(Ulid battleId)
        {
            Console.WriteLine($"[BH] AttachAsync battleId={battleId}");
            foreach (var h in Context.CallContext.RequestHeaders)
            {
                Console.WriteLine($"[BH]   attach-header {h.Key}={LogSafe.Header(h.Key, h.Value)}");
            }

            var tokenViewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            _session = BattleRegistry.Get(battleId, tokenViewerId);

            if (_session != null && tokenViewerId != 0 && !_session.IsHuman(tokenViewerId))
            {
                Console.WriteLine($"[BH] !! viewer={tokenViewerId} not seated in {_session.BattleId} → own/fresh session");
                _session = BattleRegistry.FindActiveHumanSession(tokenViewerId);
            }
            if (_session == null)
            {

                var human = AccountStore.GetOrDefault(tokenViewerId);
                _session = BattleRegistry.CreateSingleVsAi(1, human.ViewerId, human.Name, human.AvatarId);
            }

            _viewerId = tokenViewerId != 0 ? tokenViewerId : _session.HumanViewerId;
            if (!_session.IsHuman(_viewerId))
            {
                Console.WriteLine($"[BH] !! viewer={_viewerId} not seated in {_session.BattleId} seats=[{string.Join(",", _session.PlayerViewerIds)}]");
            }

            _group = await Group.AddAsync($"battle-{_session.BattleId}");
            _session.Group = _group;

            _own = await Group.AddAsync($"battle-own-{Context.ContextId:N}");
            _session.RegisterChannel(_viewerId, _own);
            _session.MarkHumanAttached(_viewerId, Context.ContextId);

            FrameDump.Record(nameof(IBattleHubReceiver.OnAttached), new object[] { _viewerId });
            _group.CreateBroadcaster<IBattleHubReceiver>().OnAttached(_viewerId);
            _session.InitializeInitialDistribution();

            var resp = new BattleAttachResponse
            {
                AttachedViewerId = _viewerId,
                Players = _session.Players.ToArray(),
                Teams = _session.Teams.ToArray(),
                GuildBuffData = _session.GuildBuffData,
                CurrentPhase = _session.Phase
            };
            Console.WriteLine($"[BH] Attach done viewer={_viewerId} players={resp.Players.Length} phase={resp.CurrentPhase}");
            return resp;
        }

        private void Trace(string method)
        {
            Console.WriteLine($"[BH] {method} viewer={_viewerId} session={_session?.BattleId} phase={_session?.Phase} round={_session?.Round}");

            if (_session != null && NetTrace.Enabled) _session.Net.NoteInbound(_viewerId);
        }

        public Task<bool> TrySignalAsync(int round, BattleSynchronizeSignal signal)
        {

            if (_session != null && NetTrace.Enabled) _session.Net.NoteInbound(_viewerId);
            if (_session == null) return Task.FromResult(false);
            var startSequence = false;
            lock (_session.SignalLock)
            {
                var exp = _session.ExpectedSignal;

                if (exp == null)
                {
                    Console.WriteLine($"[BH] signal (round={round},{signal}) ignored: battle finished (expected=null) viewer={_viewerId}");
                    return Task.FromResult(false);
                }
                if (round == exp[0] && (int)signal == exp[1])
                {
                    if (round == 0 && (int)signal == 0 && !_session.SequenceStarted)
                    {

                        _session.ExpectedSignal = new[] { _session.Round, 1 };
                        _session.SequenceStarted = true;
                        startSequence = true;
                        Console.WriteLine($"[BH] signal (0,Initialized) accepted -> ({_session.Round},BeginRound) + start sequence viewer={_viewerId}");
                    }
                    else if ((int)signal == 1)
                    {

                        if (_session.Phase == BattlePhase.Prepared && !_session.BattleRunning && !_session.Finished)
                        {
                            _session.ExpectedSignal = new[] { _session.Round, 2 };
                            _session.BattleRunning = true;
                            _ = _session.RunSimpleBattleAsync();

                            var rb = _session.RoundBeginSentAtUtc;
                            var drainMs = rb == default ? -1 : (DateTime.UtcNow - rb).TotalMilliseconds;
                            Console.WriteLine($"[BH] signal (round={round},BeginRound) -> simplified battle start (drain={drainMs:F0}ms viewer={_viewerId})");
                        }
                        else
                        {
                            Console.WriteLine($"[BH] signal (round={round},BeginRound) ignored phase={_session.Phase} viewer={_viewerId}");
                        }
                    }
                    else if ((int)signal == 2)
                    {

                        _session.ExpectedSignal = new[] { _session.Round, 3 };
                        Console.WriteLine($"[BH] signal (round={round},RoundResultReady) accepted -> expect ({round},RoundResultCompleted) viewer={_viewerId}");
                    }
                    else if ((int)signal == 3)
                    {

                        if (_session.Phase == BattlePhase.Result)
                        {
                            Console.WriteLine($"[BH] signal (round={round},RoundResultCompleted) -> advance/finish viewer={_viewerId}");
                            _session.HandleRoundResultCompleted();
                        }
                        else
                        {
                            Console.WriteLine($"[BH] signal (round={round},RoundResultCompleted) ignored phase={_session.Phase} viewer={_viewerId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[BH] signal (round={round},{signal}) stale/ignored viewer={_viewerId}");
                    }
                }
                else if (round == exp[0] && (int)signal == 3 && exp[1] == 2 && _session.Phase == BattlePhase.Result)
                {

                    Console.WriteLine($"[BH] signal (round={round},RoundResultCompleted) accepted (skipped Ready) -> advance/finish viewer={_viewerId}");
                    _session.HandleRoundResultCompleted();
                }
                else
                {
                    Console.WriteLine($"[BH] signal (round={round},{signal}) stale/ignored viewer={_viewerId}");
                }
            }
            if (startSequence)
            {
                _ = _session.RunRoundSetupSequenceAsync();
            }
            return Task.FromResult(true);
        }

        private IBattleHubReceiver Broadcaster => _group?.CreateBroadcaster<IBattleHubReceiver>();

        private void SendToSelf(Action<IBattleHubReceiver> send)
        {
            var bc = _own?.CreateBroadcaster<IBattleHubReceiver>();
            if (bc == null)
            {
                Console.WriteLine($"[BH] !! SendToSelf skipped viewer={_viewerId} (no own channel)");
                return;
            }
            send(bc);
        }

        private void RejectRoster(string reason)
        {
            if (_session == null) return;
            if (reason == "field_limit_reached" || reason == "occupied_cell")
            {
                Broadcaster?.OnCheckFieldUnitCount(_session.Players.ToArray());
            }
        }

        public Task RefreshShopUnit()
        {
            Trace(nameof(RefreshShopUnit));
            var res = _session?.RefreshShop(_viewerId);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);

                SendToSelf(bc => bc.OnRefreshShopUnit(p, null));
                Console.WriteLine($"[BH] RefreshShop OK gold={p.Gold} slots={p.ShopUnitList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] RefreshShop reject: {res?.Reason}");

                if (_session != null) SendToSelf(bc => bc.OnRefreshShopUnit(_session.FindPlayer(_viewerId), null));
            }
            return Task.CompletedTask;
        }

        public Task BuyShopUnit(int uniqueKey)
        {
            Trace(nameof(BuyShopUnit));
            var res = _session?.BuyShopUnit(_viewerId, uniqueKey, null, null, null, null);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                if (res.DidRankUp)
                {
                    SendToSelf(bc => bc.OnBuyShopUnitAndRankUp(p, res.RankUppedUnit));
                }
                else
                {
                    SendToSelf(bc => bc.OnBuyShopUnit(p, uniqueKey));
                }
                Console.WriteLine($"[BH] BuyShopUnit OK key={uniqueKey} gold={p.Gold} rankUp={res.DidRankUp} deck={p.DeckUnitList?.Count ?? 0}");
            }
            else
            {
                RejectRoster(res?.Reason);
                Console.WriteLine($"[BH] BuyShopUnit reject key={uniqueKey}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task BuyShopUnitToDeck(int uniqueKey, int deckIndex)
        {
            Trace(nameof(BuyShopUnitToDeck));
            var res = _session?.BuyShopUnit(_viewerId, uniqueKey, deckIndex, null, null, null);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                if (res.DidRankUp)
                {
                    SendToSelf(bc => bc.OnBuyShopUnitToDeckAndRankUp(p, res.RankUppedUnit));
                }
                else
                {
                    SendToSelf(bc => bc.OnBuyShopUnitToDeck(p));
                }
                Console.WriteLine($"[BH] BuyShopUnitToDeck OK key={uniqueKey} idx={deckIndex} gold={p.Gold} rankUp={res.DidRankUp}");
            }
            else
            {
                RejectRoster(res?.Reason);
                Console.WriteLine($"[BH] BuyShopUnitToDeck reject key={uniqueKey} idx={deckIndex}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task BuyShopUnitToField(int uniqueKey, int positionX, int positionY, int fieldIndex)
        {
            Trace(nameof(BuyShopUnitToField));
            var res = _session?.BuyShopUnit(_viewerId, uniqueKey, null, positionX, positionY, fieldIndex);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                if (res.DidRankUp)
                {
                    SendToSelf(bc => bc.OnBuyShopUnitToFieldAndRankUp(p, res.RankUppedUnit));
                }
                else
                {
                    SendToSelf(bc => bc.OnBuyShopUnitToField(p));
                }
                Console.WriteLine($"[BH] BuyShopUnitToField OK key={uniqueKey} pos=({positionX},{positionY}) gold={p.Gold} rankUp={res.DidRankUp} field={p.FieldUnitList?.Count ?? 0}");
            }
            else
            {
                RejectRoster(res?.Reason);
                Console.WriteLine($"[BH] BuyShopUnitToField reject key={uniqueKey} pos=({positionX},{positionY}): {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task ChangeFieldUnitPosition(int uniqueKey, int positionX, int positionY, int index)
        {
            Trace(nameof(ChangeFieldUnitPosition));
            var res = _session?.ChangeFieldUnitPositionOp(_viewerId, uniqueKey, positionX, positionY, index);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnChangeFieldUnitPosition(p));

                SendToSelf(bc => bc.OnFieldDeployDeckUnit(p));
                Console.WriteLine($"[BH] ChangeFieldUnitPosition OK key={uniqueKey} pos=({positionX},{positionY}) field={p.FieldUnitList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] ChangeFieldUnitPosition reject key={uniqueKey}: {res?.Reason}");
                if (_session != null)
                {
                    SendToSelf(bc => bc.OnChangeFieldUnitPosition(_session.FindPlayer(_viewerId)));
                    SendToSelf(bc => bc.OnFieldDeployDeckUnit(_session.FindPlayer(_viewerId)));
                }
                RejectRoster(res?.Reason);
            }
            return Task.CompletedTask;
        }

        public Task ChangeDeckUnitPosition(int beforeIndex, int afterIndex)
        {
            Trace(nameof(ChangeDeckUnitPosition));
            var res = _session?.ChangeDeckUnitPositionOp(_viewerId, beforeIndex, afterIndex);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnChangeDeckUnitPosition(p));
                Console.WriteLine($"[BH] ChangeDeckUnitPosition OK {beforeIndex}->{afterIndex} deck={p.DeckUnitList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] ChangeDeckUnitPosition reject {beforeIndex}->{afterIndex}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task SellUnit(int uniqueKey)
        {
            Trace(nameof(SellUnit));
            var res = _session?.SellUnitOp(_viewerId, uniqueKey);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnSellUnit(p));
                Console.WriteLine($"[BH] SellUnit OK key={uniqueKey} gold={p.Gold} field={p.FieldUnitList?.Count ?? 0} deck={p.DeckUnitList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] SellUnit reject key={uniqueKey}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task BuyExp()
        {
            Trace(nameof(BuyExp));
            var res = _session?.BuyExpOp(_viewerId);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnBuyExp(p));
                Console.WriteLine($"[BH] BuyExp OK level={p.Level} gold={p.Gold}");
            }
            else
            {
                Console.WriteLine($"[BH] BuyExp reject: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task<BattlePlayerDataMpo> ToggleShopLock()
        {
            Trace(nameof(ToggleShopLock));
            var p = _session != null ? _session.ToggleShopLockOp(_viewerId) : default;
            return Task.FromResult(p);
        }

        public Task FieldDeployDeckUnit(int uniqueKey, int positionX, int positionY, int index)
        {
            Trace(nameof(FieldDeployDeckUnit));
            var res = _session?.DeployDeckUnitOp(_viewerId, uniqueKey, positionX, positionY, index);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnFieldDeployDeckUnit(p));
                Console.WriteLine($"[BH] FieldDeployDeckUnit OK key={uniqueKey} pos=({positionX},{positionY}) field={p.FieldUnitList?.Count ?? 0} deck={p.DeckUnitList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] FieldDeployDeckUnit reject key={uniqueKey}: {res?.Reason}");

                if (_session != null)
                    SendToSelf(bc => bc.OnFieldDeployDeckUnit(_session.FindPlayer(_viewerId)));
                RejectRoster(res?.Reason);
            }
            return Task.CompletedTask;
        }

        public Task RankupUnit(int unitKey)
        {
            Trace(nameof(RankupUnit));
            var res = _session?.RankUpOwnedUnitOp(_viewerId, unitKey, false);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnRankupUnit(p, res.RankUppedUnit));
                Console.WriteLine($"[BH] RankupUnit OK key={unitKey} -> rank={res.RankUppedUnit?.Rank} unitId={res.RankUppedUnit?.UnitId}");
            }
            else
            {
                Console.WriteLine($"[BH] RankupUnit reject key={unitKey}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task RankupUnitWithEquipment(int unitKey)
        {
            Trace(nameof(RankupUnitWithEquipment));
            var res = _session?.RankUpOwnedUnitOp(_viewerId, unitKey, true);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnRankupUnit(p, res.RankUppedUnit));
                Console.WriteLine($"[BH] RankupUnitWithEquipment OK key={unitKey} -> rank={res.RankUppedUnit?.Rank} unitId={res.RankUppedUnit?.UnitId}");
            }
            else
            {
                Console.WriteLine($"[BH] RankupUnitWithEquipment reject key={unitKey}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task RankupUnitByToken(int unitKey) { Trace(nameof(RankupUnitByToken)); return Task.CompletedTask; }

        public Task ReturnFieldUnitToDeck(int uniqueKey, int index)
        {
            Trace(nameof(ReturnFieldUnitToDeck));
            var res = _session?.ReturnFieldUnitToDeckOp(_viewerId, uniqueKey, index);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnReturnFieldUnitToDeck(p));
                Console.WriteLine($"[BH] ReturnFieldUnitToDeck OK key={uniqueKey} idx={index} field={p.FieldUnitList?.Count ?? 0} deck={p.DeckUnitList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] ReturnFieldUnitToDeck reject key={uniqueKey}: {res?.Reason}");
                RejectRoster(res?.Reason);
            }
            return Task.CompletedTask;
        }
        public Task EquipmentToUnit(int unitKey, int equipmentKey)
        {
            Trace(nameof(EquipmentToUnit));
            var res = _session?.EquipmentToUnitOp(_viewerId, unitKey, equipmentKey);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnEquipmentToUnit(new EquipmentToUnitResponse
                {
                    ViewerId = _viewerId,
                    PlayerData = p,
                    UnitData = res.RankUppedUnit
                }));
                Console.WriteLine($"[BH] EquipmentToUnit OK unit={unitKey} gear={equipmentKey} attached={res.RankUppedUnit?.EquipmentList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] EquipmentToUnit reject unit={unitKey} gear={equipmentKey}: {res?.Reason}");
                if (res?.Reason == "phase_not_preparing") RejectRoster(res.Reason);
            }
            return Task.CompletedTask;
        }

        public Task ReceiveDropEquipment(params int[] uniqueKeys)
        {
            Trace(nameof(ReceiveDropEquipment));
            var keys = uniqueKeys ?? Array.Empty<int>();
            var res = _session?.ReceiveDropEquipmentOp(_viewerId, keys);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                var received = res.ReceivedEquipments ?? Array.Empty<BattleEquipmentDataMpo>();
                SendToSelf(bc => bc.OnReceiveDropEquipment(p, received));
                Console.WriteLine($"[BH] ReceiveDropEquipment OK keys={string.Join(",", keys)} count={received.Length} inv={p.EquipmentList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] ReceiveDropEquipment reject keys={string.Join(",", keys)}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task ReceiveLotteryEquipment(int stockIndex, params int[] uniqueKeys)
        {
            Trace(nameof(ReceiveLotteryEquipment));
            var keys = uniqueKeys ?? Array.Empty<int>();
            var res = _session?.ReceiveLotteryEquipmentOp(_viewerId, stockIndex, keys);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                var received = res.ReceivedEquipments ?? Array.Empty<BattleEquipmentDataMpo>();
                SendToSelf(bc => bc.OnReceiveLotteryEquipment(p, stockIndex, received));
                Console.WriteLine($"[BH] ReceiveLotteryEquipment OK stock={stockIndex} keys={string.Join(",", keys)} count={received.Length} inv={p.EquipmentList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] ReceiveLotteryEquipment reject stock={stockIndex} keys={string.Join(",", keys)}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task DistributeUnit() { Trace(nameof(DistributeUnit)); return Task.CompletedTask; }
        public Task PickUpUnit(int uniqueKey) { Trace(nameof(PickUpUnit)); return Task.CompletedTask; }
        public Task NotifyPickUpUnit(int uniqueKey) { Trace(nameof(NotifyPickUpUnit)); return Task.CompletedTask; }
        public Task PickUpItem(int itemIndex) { Trace(nameof(PickUpItem)); return Task.CompletedTask; }
        public Task NotifyPickUpItem(int itemIndex) { Trace(nameof(NotifyPickUpItem)); return Task.CompletedTask; }
        public Task PickUpOverallBuff(int selectedOverallBuffId) { Trace(nameof(PickUpOverallBuff)); return Task.CompletedTask; }
        public Task InitialPickUpSelectUnit(int unitUniqueKey) { Trace(nameof(InitialPickUpSelectUnit)); return Task.CompletedTask; }
        public Task InitialPickUpCancelSelectUnit(int unitUniqueKey) { Trace(nameof(InitialPickUpCancelSelectUnit)); return Task.CompletedTask; }
        public Task ReceiveTeamTakeOverUnit() { Trace(nameof(ReceiveTeamTakeOverUnit)); return Task.CompletedTask; }
        public Task ReceiveTeamTakeOverUnitToDeck(int deckIndex) { Trace(nameof(ReceiveTeamTakeOverUnitToDeck)); return Task.CompletedTask; }
        public Task ReceiveTeamTakeOverUnitToField(int positionX, int positionY, int index) { Trace(nameof(ReceiveTeamTakeOverUnitToField)); return Task.CompletedTask; }
        public Task SetTeamTransferUnit(int uniqueKey) { Trace(nameof(SetTeamTransferUnit)); return Task.CompletedTask; }
        public Task ReturnTeamTransferUnitToDeck(int index) { Trace(nameof(ReturnTeamTransferUnitToDeck)); return Task.CompletedTask; }
        public Task ReturnTeamTransferUnitToField(int positionX, int positionY, int index) { Trace(nameof(ReturnTeamTransferUnitToField)); return Task.CompletedTask; }

        public Task<UserReconnectResponse> NotifyPlayerReConnected()
        {
            Trace(nameof(NotifyPlayerReConnected));
            var autoStartSequence = false;
            if (_session != null && _session.Phase == BattlePhase.Initialized && !_session.SequenceStarted)
            {

                lock (_session.SignalLock)
                {
                    _session.ExpectedSignal = new[] { _session.Round, 1 };
                    _session.SequenceStarted = true;
                }
                autoStartSequence = true;
                Console.WriteLine($"[BH] NotifyPlayerReConnected on fresh session={_session.BattleId} -> auto-start sequence");
            }

            var phase = _session?.Phase ?? BattlePhase.Invalid;

            var phaseWire = phase == BattlePhase.Complete || phase == BattlePhase.Reconnect
                ? BattlePhase.Result
                : phase;
            var npcResponses = Array.Empty<NpcRoundStartResponse>();
            var battleResults = Array.Empty<BattleResultInfoMpo>();
            TimeSpan? remain = null;
            if (_session != null)
            {
                var npcId = BattleMaster.NpcIdForRound(_session.Round);

                if (_session.Rule == BattleRule.Boss) npcId = BattleMaster.BossRow.NpcId;

                if (npcId != 0 && phase >= BattlePhase.Preparing && phase <= BattlePhase.Result)
                {
                    npcResponses = _session.BuildNpcRoundStartResponses(npcId);
                }

                if (phase == BattlePhase.Result)
                {
                    battleResults = _session.Finished
                        ? _session.BuildBattleResults()
                        : Array.Empty<BattleResultInfoMpo>();
                }

                if (phase == BattlePhase.Preparing && _session.PreparationOpen && _session.PreparationDeadlineUtc.HasValue)
                {
                    var left = _session.PreparationDeadlineUtc.Value - DateTime.UtcNow;
                    remain = left < TimeSpan.Zero ? TimeSpan.Zero : left;
                }
            }

            var resp = new UserReconnectResponse
            {
                Phase = phaseWire,
                Players = _session?.Players.ToArray() ?? new BattlePlayerDataMpo[0],
                NpcRoundStartResponse = npcResponses,
                Round = _session?.Round ?? 0,
                RemainTime = remain,
                BattleResults = battleResults,
                Teams = _session?.Teams.ToArray() ?? new BattleTeamDataMpo[0]
            };
            Console.WriteLine($"[BH] reconnect resp phase={phase} wire={phaseWire} round={resp.Round} players={resp.Players.Length} npc={npcResponses.Length} results={battleResults.Length} remainMs={(remain.HasValue ? remain.Value.TotalMilliseconds.ToString("F0") : "null")} autoStart={autoStartSequence}");
            if (autoStartSequence)
            {
                _ = _session.RunRoundSetupSequenceAsync();
            }
            else if (_session != null && _session.Phase == BattlePhase.Prepared)
            {

                _session.SendRoundBeginToViewer(_viewerId);
            }
            else if (_session != null && _session.Phase == BattlePhase.Running)
            {

                _ = _session.SendRemainingAxisToViewer(_viewerId);
            }
            return Task.FromResult(resp);
        }

        public Task EquipmentToUnitSynthesis(int unitKey, int srcEquipmentKey, int afterSynthesisId, params int[] synthesisMaterialKeys) { Trace(nameof(EquipmentToUnitSynthesis)); return Task.CompletedTask; }
        public Task OwnedEquipmentSynthesis(int srcEquipmentKey, int afterSynthesisId, params int[] synthesisMaterialKeys) { Trace(nameof(OwnedEquipmentSynthesis)); return Task.CompletedTask; }
        public Task RankUpEquipment(int equipmentKey, int afterEquipmentId) { Trace(nameof(RankUpEquipment)); return Task.CompletedTask; }
        public Task RankUpEquipmentWithEquipped(int unitKey, int equipmentKey, int afterEquipmentId) { Trace(nameof(RankUpEquipmentWithEquipped)); return Task.CompletedTask; }

        public Task Ready()
        {
            Trace(nameof(Ready));
            if (_session != null)
            {
                _session.RegisterReady(_viewerId);
            }
            return Task.CompletedTask;
        }

        public Task Unready()
        {
            Trace(nameof(Unready));
            if (_session != null)
            {
                _session.RemoveReady(_viewerId);

                FrameDump.Record(nameof(IBattleHubReceiver.OnUnready), new object[] { new[] { _viewerId } });
                _group.CreateBroadcaster<IBattleHubReceiver>().OnUnready(new[] { _viewerId });
            }
            return Task.CompletedTask;
        }

        public Task<ResumeResponse> OnResume()
        {
            Trace(nameof(OnResume));
            TimeSpan? remain = null;
            if (_session != null && _session.Phase == BattlePhase.Preparing && _session.PreparationDeadlineUtc.HasValue)
            {
                var left = _session.PreparationDeadlineUtc.Value - DateTime.UtcNow;
                remain = left > TimeSpan.Zero ? left : TimeSpan.Zero;
            }
            Console.WriteLine($"[BH] OnResume phase={_session?.Phase} remainMs={(remain.HasValue ? remain.Value.TotalMilliseconds.ToString("F0") : "null")}");
            return Task.FromResult(new ResumeResponse { RemainTime = remain });
        }

        public Task SetBattleEventIntervalRate(float rate)
        {
            Trace(nameof(SetBattleEventIntervalRate));
            _session?.SetBattleEventIntervalRateOp(_viewerId, rate);
            return Task.CompletedTask;
        }

        public Task SendEmote(int emoteId)
        {
            Trace(nameof(SendEmote));
            _session?.SendEmoteOp(_viewerId, emoteId);
            return Task.CompletedTask;
        }

        public Task UpdateRecommendPartyData(int id)
        {
            Trace(nameof(UpdateRecommendPartyData));
            _session?.UpdateRecommendPartyDataOp(_viewerId, id);
            return Task.CompletedTask;
        }

        public Task DisassembleEquipment(params int[] equipmentKeys)
        {
            Trace(nameof(DisassembleEquipment));
            var keys = equipmentKeys ?? Array.Empty<int>();
            var res = _session?.DisassembleEquipmentOp(_viewerId, keys);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnDisassembleEquipment(p, res.TokenNum));
                Console.WriteLine($"[BH] DisassembleEquipment OK keys={string.Join(",", keys)} token={res.TokenNum} inv={p.EquipmentList?.Count ?? 0} rankUpToken={p.RankUpToken}");
            }
            else
            {
                Console.WriteLine($"[BH] DisassembleEquipment reject keys={string.Join(",", keys)}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task DisassembleEquipmentWithEquipped(int unitKey, params int[] equipmentKeys)
        {
            Trace(nameof(DisassembleEquipmentWithEquipped));
            var keys = equipmentKeys ?? Array.Empty<int>();
            var res = _session?.DisassembleEquipmentWithEquippedOp(_viewerId, unitKey, keys);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnDisassembleEquipmentWithEquipped(p, res.TokenNum, new EquipmentToUnitResponse
                {
                    ViewerId = _viewerId,
                    PlayerData = p,
                    UnitData = res.RankUppedUnit
                }));
                Console.WriteLine($"[BH] DisassembleEquipmentWithEquipped OK unit={unitKey} keys={string.Join(",", keys)} token={res.TokenNum} attached={res.RankUppedUnit?.EquipmentList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] DisassembleEquipmentWithEquipped reject unit={unitKey} keys={string.Join(",", keys)}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task SkipToRoundMatchCompletionAsync() { Trace(nameof(SkipToRoundMatchCompletionAsync)); return Task.CompletedTask; }

        public Task SelectEquipmentAsync(int equipmentKey, int selectEquipmentId)
        {
            Trace(nameof(SelectEquipmentAsync));
            var res = _session?.SelectEquipmentOp(_viewerId, equipmentKey, selectEquipmentId, 0);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnSelectEquipment(p));
                Console.WriteLine($"[BH] SelectEquipmentAsync OK gear={equipmentKey} -> id={selectEquipmentId} inv={p.EquipmentList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] SelectEquipmentAsync reject gear={equipmentKey} selectId={selectEquipmentId}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task SelectEquipmentAndEquipToUnit(int equipmentKey, int selectEquipmentId, int targetUnitKey)
        {
            Trace(nameof(SelectEquipmentAndEquipToUnit));
            var res = _session?.SelectEquipmentAndEquipToUnitOp(_viewerId, equipmentKey, selectEquipmentId, targetUnitKey);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnEquipmentToUnit(new EquipmentToUnitResponse
                {
                    ViewerId = _viewerId,
                    PlayerData = p,
                    UnitData = res.RankUppedUnit
                }));
                Console.WriteLine($"[BH] SelectEquipmentAndEquipToUnit OK gear={equipmentKey} -> id={selectEquipmentId} unit={targetUnitKey} attached={res.RankUppedUnit?.EquipmentList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] SelectEquipmentAndEquipToUnit reject gear={equipmentKey} selectId={selectEquipmentId} unit={targetUnitKey}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task AbandonAsync()
        {
            Trace(nameof(AbandonAsync));
            _session?.AbandonOp(_viewerId);
            return Task.CompletedTask;
        }
        public Task RemoveEquipmentFromUnit(int unitKey, params int[] equipmentKeys)
        {
            Trace(nameof(RemoveEquipmentFromUnit));
            var res = _session?.RemoveEquipmentFromUnitOp(_viewerId, unitKey, equipmentKeys, false);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnRemoveEquipmentFromUnit(new EquipmentToUnitResponse
                {
                    ViewerId = _viewerId,
                    PlayerData = p,
                    UnitData = res.RankUppedUnit
                }));
                Console.WriteLine($"[BH] RemoveEquipmentFromUnit OK unit={unitKey} keys={string.Join(",", equipmentKeys ?? Array.Empty<int>())} remaining={res.RankUppedUnit?.EquipmentList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] RemoveEquipmentFromUnit reject unit={unitKey}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task RemoveAllEquipmentFromUnit(int unitKey)
        {
            Trace(nameof(RemoveAllEquipmentFromUnit));
            var res = _session?.RemoveEquipmentFromUnitOp(_viewerId, unitKey, null, true);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                SendToSelf(bc => bc.OnRemoveEquipmentFromUnit(new EquipmentToUnitResponse
                {
                    ViewerId = _viewerId,
                    PlayerData = p,
                    UnitData = res.RankUppedUnit
                }));
                Console.WriteLine($"[BH] RemoveAllEquipmentFromUnit OK unit={unitKey} remaining={res.RankUppedUnit?.EquipmentList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] RemoveAllEquipmentFromUnit reject unit={unitKey}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task MigrateEquipmentBetweenUnits(int fromUnitKey, int toUnitKey, int equipmentKey)
        {
            Trace(nameof(MigrateEquipmentBetweenUnits));
            var res = _session?.MigrateEquipmentOp(_viewerId, fromUnitKey, toUnitKey, equipmentKey);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                var fromUnit = (p.FieldUnitList ?? new System.Collections.Generic.List<BattleUnitDataMpo>()).Find(u => u.UniqueKey == fromUnitKey)
                    ?? (p.DeckUnitList ?? new System.Collections.Generic.List<BattleUnitDataMpo>()).Find(u => u.UniqueKey == fromUnitKey);
                var toUnit = res.RankUppedUnit;
                SendToSelf(bc => bc.OnMigrateEquipmentBetweenUnits(
                    new EquipmentToUnitResponse { ViewerId = _viewerId, PlayerData = p, UnitData = fromUnit },
                    new EquipmentToUnitResponse { ViewerId = _viewerId, PlayerData = p, UnitData = toUnit }));
                Console.WriteLine($"[BH] MigrateEquipmentBetweenUnits OK {fromUnitKey}->{toUnitKey} gear={equipmentKey}");
            }
            else
            {
                Console.WriteLine($"[BH] MigrateEquipmentBetweenUnits reject {fromUnitKey}->{toUnitKey}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task UseSupporterItem(int itemId) { Trace(nameof(UseSupporterItem)); return Task.CompletedTask; }

        public Task RequestRedistributeNpcData() { Trace(nameof(RequestRedistributeNpcData)); return Task.CompletedTask; }
        public Task DebugMenuAddUnit(int id, int rank)
        {
            Trace(nameof(DebugMenuAddUnit));
            var res = _session?.DebugAddUnitOp(_viewerId, id, rank);
            if (res != null && res.Ok)
            {
                var p = _session.FindPlayer(_viewerId);
                Broadcaster.OnUpdatePlayers(_session.Players.ToArray());
                Console.WriteLine($"[BH] DebugMenuAddUnit OK unitId={id} rank={rank} key={res.UniqueKey} deck={p.DeckUnitList?.Count ?? 0}");
            }
            else
            {
                Console.WriteLine($"[BH] DebugMenuAddUnit reject unitId={id} rank={rank}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task DebugMenuAddEquipment(int id)
        {
            Trace(nameof(DebugMenuAddEquipment));
            var res = _session?.DebugAddEquipmentOp(_viewerId, id);
            if (res != null && res.Ok)
            {
                Broadcaster.OnUpdatePlayers(_session.Players.ToArray());
                Console.WriteLine($"[BH] DebugMenuAddEquipment OK equipId={id} key={res.UniqueKey}");
            }
            else
            {
                Console.WriteLine($"[BH] DebugMenuAddEquipment reject equipId={id}: {res?.Reason}");
            }
            return Task.CompletedTask;
        }

        public Task DebugMenuEditPlayerInfo(int viewerId, int? life, int? gold)
        {
            Trace(nameof(DebugMenuEditPlayerInfo));
            if (_session != null && _session.EditPlayerInfoOp(viewerId, life, gold))
            {
                Broadcaster.OnUpdatePlayers(_session.Players.ToArray());
            }
            return Task.CompletedTask;
        }

        public Task DebugMenuForceCompleteWaitForCommand()
        {
            Trace(nameof(DebugMenuForceCompleteWaitForCommand));
            _session?.ForceCompleteWaitForCommandOp(_viewerId);
            return Task.CompletedTask;
        }

        public Task DebugMenuForceSurrenderPlayers()
        {
            Trace(nameof(DebugMenuForceSurrenderPlayers));
            _session?.ForceSurrenderPlayersOp(_viewerId);
            return Task.CompletedTask;
        }
    }
}
