using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Server.Hubs;
using Byac.ServerShared.Hub;
using Byac.ServerShared.MessagePackObjects;
using Byac.Services.Battle;

namespace Byac.Services
{

    public class MatchMakingHub : StreamingHubBase<IMatchMakingHub, IMatchMakingHubReceiver>, IMatchMakingHub
    {
        private IGroup? _own;
        private IGroup? _lobby;
        private string? _lobbyKey;
        private Window? _window;

        private static int? _windowSecondsOverride;

        public static int WindowSeconds =>
            _windowSecondsOverride ??= ParseWindowSeconds();

        public static bool WaitFullHouse => WindowSeconds == 0;

        private static int ParseWindowSeconds() =>
            int.TryParse(Environment.GetEnvironmentVariable("BYAC_MM_WINDOW_SEC"), out var mmw) && mmw >= 0 ? mmw : 60;

        public static int JoinCadenceMs = 400;
        public static int PreStartMs = 2500;

        public static int VoteFillCadenceMs = 2000;

        public static int ReEntryToggleMs = 15000;

        public const string EarlyStartMark = "[早]";

        private static bool? _earlyStartEnabled;

        public static bool EarlyStartEnabled => _earlyStartEnabled ??=
            Environment.GetEnvironmentVariable("BYAC_MM_EARLY_START") != "0";

        private static readonly object Gate = new object();
        private static readonly Dictionary<Guid, Window> SeatOwner = new Dictionary<Guid, Window>();

        private static readonly List<Window> Open = new List<Window>();
        private static int _generation;
        private static long _entryTotal, _settledTotal, _duplicateRejects;

        private static readonly Dictionary<int, LeftVote> LeftVotes = new Dictionary<int, LeftVote>();

        private struct LeftVote
        {
            public int Kind;
            public int Gen;
            public bool Vote;
            public DateTime LeftUtc;
        }

        private sealed class Seat
        {
            public Guid ConnectionId;
            public int ViewerId;
            public string Name = "";
            public int AvatarId;
            public int Grade;
            public int DesiredWindowSec = WindowSeconds;

            public bool EarlyStart;

            public DateTime? StaleAtUtc;
        }

        private sealed class Window
        {
            public int Generation;
            public string GroupKey = "";
            public Ulid EntryId;
            public int DummyKind;
            public DateTime DeadlineUtc;
            public readonly List<Seat> Seats = new List<Seat>();

            public readonly List<Seat> AiSeats = new List<Seat>();

            public bool Filling;

            public bool FillingCancellable;
            public int FillCadenceMs;
            public DateTime NextAiAtUtc;

            public int FillHumanBase;

            public bool RosterDirty;

            public IGroup? Group;
            public bool Accepting = true;
            public bool Settled;

            public readonly List<IGroup> OwnChannels = new List<IGroup>();
        }

        protected override ValueTask OnConnecting()
        {
            Console.WriteLine($"[MM] connecting ctx={Context.ContextId}");
            foreach (var h in Context.CallContext.RequestHeaders)
            {
                Console.WriteLine($"[MM]   header {h.Key}={LogSafe.Header(h.Key, h.Value)}");
            }
            return default;
        }

        protected override ValueTask OnDisconnected()
        {
            Console.WriteLine($"[MM] disconnected ctx={Context.ContextId} gen={_window?.Generation} settled={_window?.Settled}");
            LeaveWindow(false);
            return default;
        }

        public async Task EntryAsync(MatchMakingEntryRequest request)
        {

            var viewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
            var human = AccountStore.GetOrDefault(viewerId);

            var activeBattle = BattleRegistry.FindActiveHumanSession(viewerId, excludeBoss: true);
            if (activeBattle != null)
            {
                Window? retireAfterSweep = null;
                lock (Gate)
                {
                    foreach (var w in Open)
                    {
                        var ghost = w.Seats.Find(s => s.ViewerId == viewerId);
                        if (ghost == null) continue;
                        w.Seats.Remove(ghost);
                        w.RosterDirty = true;
                        Console.WriteLine($"[MM] ghost seat swept gen={w.Generation} viewer={viewerId}");

                        if (w.Accepting && w.Seats.Count == 0)
                            retireAfterSweep = w;
                    }
                    if (retireAfterSweep != null)
                        RetireLocked(retireAfterSweep, "abandoned");
                }
                if (_own == null) _own = await Group.AddAsync($"mm-own-{Context.ContextId:N}");
                _own.CreateBroadcaster<IMatchMakingHubReceiver>().OnEntry(new MatchMakingEntryResponse
                {
                    EntryId = activeBattle.BattleId,
                    DummyKindId = request.DummyKindId,
                });
                await Task.Delay(PreStartMs);
                _own.CreateBroadcaster<IMatchMakingHubReceiver>().OnComplete(new MatchMakingCompleteResponse
                {
                    BattleId = activeBattle.BattleId,
                    RouteToken = activeBattle.RouteToken,
                });
                Console.WriteLine($"[MM] redirect-to-battle viewer={viewerId} battle={activeBattle.BattleId} " +
                    $"round={activeBattle.Round} phase={activeBattle.Phase}");
                return;
            }

            var seat = new Seat
            {
                ConnectionId = Context.ContextId,
                ViewerId = human.ViewerId,
                Name = human.Name,
                AvatarId = human.AvatarId,
                Grade = BattleMaster.GradeOf(human.BattlePoint),
                DesiredWindowSec = WindowSeconds,
            };

            Window win;
            Seat stale = null;
            bool newSeat = true;
            lock (Gate)
            {

                Window? seated = null;
                foreach (var w in Open)
                {
                    if (w.Seats.Exists(s => s.ViewerId == seat.ViewerId)) { seated = w; break; }
                }
                if (seated != null)
                {
                    stale = seated.Seats.Find(s => s.ViewerId == seat.ViewerId
                                                    && s.ConnectionId != seat.ConnectionId);
                    if (stale != null)
                    {
                        Console.WriteLine($"[MM] !! viewer={seat.ViewerId} seat takeover: old ctx={stale.ConnectionId:N} " +
                            $"-> new ctx={seat.ConnectionId:N} (gen={seated.Generation}; background reconnect no longer rejected)");
                        seated.Seats.Remove(stale);
                        SeatOwner.Remove(stale.ConnectionId);

                        if (stale.EarlyStart) seat.EarlyStart = true;
                    }
                    win = seated;
                    var mine = win.Seats.Find(s => s.ConnectionId == seat.ConnectionId);
                    if (mine == null)
                    {
                        win.Seats.Add(seat);
                        SeatOwner[seat.ConnectionId] = win;
                    }
                    else if (EarlyStartEnabled && !win.Settled)
                    {

                        mine.EarlyStart = !mine.EarlyStart;
                        newSeat = false;
                        Console.WriteLine($"[MM] early-start gen={win.Generation} viewer={mine.ViewerId} " +
                            $"pressed -> {mine.EarlyStart}");
                    }
                }
                else
                {

                    Window? pick = null;
                    foreach (var w in Open)
                    {
                        if (w.Accepting && w.DummyKind == request.DummyKindId
                            && w.Seats.Count < BattleRegistry.SeatCount)
                        {
                            pick = w;
                            break;
                        }
                    }
                    win = pick ?? OpenWindow(request.DummyKindId, seat.DesiredWindowSec);
                    win.Seats.Add(seat);
                    SeatOwner[seat.ConnectionId] = win;
                    Interlocked.Increment(ref _entryTotal);

                    if (EarlyStartEnabled &&
                        TakeReEntryVoteLocked(seat.ViewerId, request.DummyKindId, DateTime.UtcNow) is bool flip)
                        seat.EarlyStart = flip;
                }

                if (newSeat)
                {
                    var laterDeadline = DateTime.UtcNow.AddSeconds(seat.DesiredWindowSec);
                    if (laterDeadline > win.DeadlineUtc)
                    {
                        Console.WriteLine($"[MM] window extended gen={win.Generation} seat={seat.ViewerId} " +
                            $"sec={seat.DesiredWindowSec} new deadline={laterDeadline:o}");
                        win.DeadlineUtc = laterDeadline;
                    }
                }
            }

            _window = win;

            if (_own == null) _own = await Group.AddAsync($"mm-own-{seat.ConnectionId:N}");
            if (_lobbyKey != win.GroupKey)
            {
                _lobby = await Group.AddAsync(win.GroupKey);
                _lobbyKey = win.GroupKey;
            }
            win.Group ??= _lobby;

            if (newSeat)
            {
                lock (Gate) win.OwnChannels.Add(_own);
            }

            _own.CreateBroadcaster<IMatchMakingHubReceiver>().OnEntry(new MatchMakingEntryResponse
            {
                EntryId = win.EntryId,
                DummyKindId = win.DummyKind
            });
            Console.WriteLine($"[MM] EntryAsync viewer={seat.ViewerId} seat={win.Seats.Count} gen={win.Generation} entry={win.EntryId}");

            BroadcastRoster(win);

            List<MatchMakingUserDataMpo> joinerRoster;
            lock (Gate) joinerRoster = RosterOf(win.Seats, win.AiSeats);
            _own.CreateBroadcaster<IMatchMakingHubReceiver>().OnJoin(
                new MatchMakingJoinResponse { GroupUsers = joinerRoster.ToArray() });
        }

        public async Task CancelAsync(MatchMakingCancelRequest request)
        {

            Console.WriteLine($"[MM] CancelAsync entryId={request.EntryId} gen={_window?.Generation} settled={_window?.Settled}");

            if (_window?.Settled == true)
            {
                var cancelViewerId = AccountStore.ViewerIdFromHeaders(Context.CallContext.RequestHeaders);
                var active = BattleRegistry.FindActiveHumanSession(cancelViewerId, excludeBoss: true);
                if (active != null)
                {
                    if (_own == null) _own = await Group.AddAsync($"mm-own-{Context.ContextId:N}");
                    _own.CreateBroadcaster<IMatchMakingHubReceiver>().OnEntry(new MatchMakingEntryResponse
                    {
                        EntryId = active.BattleId,
                        DummyKindId = _window.DummyKind,
                    });
                    await Task.Delay(PreStartMs);
                    _own.CreateBroadcaster<IMatchMakingHubReceiver>().OnComplete(new MatchMakingCompleteResponse
                    {
                        BattleId = active.BattleId,
                        RouteToken = active.RouteToken,
                    });
                    Console.WriteLine($"[MM] cancel-on-settled redirect-to-battle viewer={cancelViewerId} " +
                        $"battle={active.BattleId} round={active.Round} phase={active.Phase}");
                    _window = null;
                    _lobby = null;
                    _lobbyKey = null;
                    return;
                }
            }

            LeaveWindow(true);
            if (_own != null)
            {
                _own.CreateBroadcaster<IMatchMakingHubReceiver>().OnCancel(new MatchMakingCancelResponse
                {
                    CanceledEntryId = request.EntryId,
                    GroupUsers = new MatchMakingUserDataMpo[0]
                });
            }

            if (_lobby != null) await _lobby.RemoveAsync(Context);

            _lobby = null;
            _lobbyKey = null;
            _window = null;
        }

        public static bool HasOpenWindow
        {
            get { lock (Gate) return Open.Count > 0; }
        }

        public readonly struct MmMetrics
        {
            public MmMetrics(int openWindows, int acceptingWindows, int waitingSeats,
                long entryTotal, long settledTotal, long duplicateRejects)
            {
                OpenWindows = openWindows;
                AcceptingWindows = acceptingWindows;
                WaitingSeats = waitingSeats;
                EntryTotal = entryTotal;
                SettledTotal = settledTotal;
                DuplicateRejects = duplicateRejects;
            }

            public int OpenWindows { get; }
            public int AcceptingWindows { get; }
            public int WaitingSeats { get; }
            public long EntryTotal { get; }
            public long SettledTotal { get; }
            public long DuplicateRejects { get; }
        }

        public static MmMetrics Metrics()
        {
            lock (Gate)
            {
                var accepting = 0;
                var seats = 0;
                foreach (var w in Open)
                {
                    if (w.Accepting) accepting++;
                    seats += w.Seats.Count;
                }
                return new MmMetrics(Open.Count, accepting, seats,
                    Interlocked.Read(ref _entryTotal),
                    Interlocked.Read(ref _settledTotal),
                    Interlocked.Read(ref _duplicateRejects));
            }
        }

        private static Window OpenWindow(int dummyKind, int desiredWindowSec)
        {
            _generation++;
            var win = new Window
            {
                Generation = _generation,
                GroupKey = $"mm-lobby-{_generation}",
                EntryId = Ulid.NewUlid(),
                DummyKind = dummyKind,

                DeadlineUtc = WaitFullHouse ? DateTime.MaxValue
                    : DateTime.UtcNow.AddSeconds(Math.Max(WindowSeconds, desiredWindowSec)),
            };
            Console.WriteLine($"[MM] window opened gen={win.Generation} entry={win.EntryId} " +
                $"dummyKind={dummyKind} deadline={(WaitFullHouse ? "none(wait-full-house)" : $"{win.DeadlineUtc:o}")} " +
                $"windowSec=local:{WindowSeconds}/seat:{desiredWindowSec}");
            Open.Add(win);
            Task.Run(() => RunWindowAsync(win));
            return win;
        }

        private void LeaveWindow(bool deliberate)
        {
            Window win;
            bool rebroadcast;
            lock (Gate)
            {
                if (!SeatOwner.TryGetValue(Context.ContextId, out win)) return;
                SeatOwner.Remove(Context.ContextId);

                if (deliberate && _own != null) win.OwnChannels.Remove(_own);
                var me = win.Seats.Find(s => s.ConnectionId == Context.ContextId);
                if (me != null)
                {
                    if (deliberate && EarlyStartEnabled)
                        RecordLeftVoteLocked(me.ViewerId, win.DummyKind, win.Generation,
                                            me.EarlyStart, DateTime.UtcNow);
                    if (!deliberate && !win.Settled)
                    {

                        me.StaleAtUtc = DateTime.UtcNow;
                        Console.WriteLine($"[MM] stale seat kept gen={win.Generation} viewer={me.ViewerId} " +
                            $"(seat kept on disconnect; rejoin restores the same room and slot)");
                    }
                    else
                    {
                        win.Seats.Remove(me);
                    }
                }

                rebroadcast = win.Seats.Count > 0 && (win.Accepting || win.AiSeats.Count > 0);
                if (win.Accepting && win.Seats.Count == 0)
                    RetireLocked(win, "abandoned");
            }
            if (!rebroadcast || _lobby == null) return;
            try { BroadcastRoster(win); }
            catch (Exception ex)
            {
                Console.WriteLine($"[MM] roster after leave failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        internal static void RecordLeftVoteLocked(int viewerId, int dummyKind, int generation,
                                                 bool vote, DateTime nowUtc)
        {
            LeftVotes[viewerId] = new LeftVote
            { Kind = dummyKind, Gen = generation, Vote = vote, LeftUtc = nowUtc };
            List<int>? stale = null;
            foreach (var kv in LeftVotes)
                if ((nowUtc - kv.Value.LeftUtc).TotalMilliseconds > ReEntryToggleMs)
                    (stale ??= new List<int>()).Add(kv.Key);
            if (stale != null)
                foreach (var k in stale) LeftVotes.Remove(k);
        }

        internal static bool? TakeReEntryVoteLocked(int viewerId, int dummyKind, DateTime nowUtc)
        {
            if (!LeftVotes.TryGetValue(viewerId, out var left)) return null;
            LeftVotes.Remove(viewerId);
            if (left.Kind != dummyKind) return null;
            if ((nowUtc - left.LeftUtc).TotalMilliseconds > ReEntryToggleMs) return null;
            bool flip = !left.Vote;
            Console.WriteLine($"[MM] early-start viewer={viewerId} re-enter after cancel " +
                $"gen={left.Gen} -> new vote {flip} ({(left.Vote ? "withdraw" : "press")})");
            return flip;
        }

        private static void RetireLocked(Window win, string reason)
        {
            if (!Open.Remove(win)) return;
            foreach (var s in win.Seats)
            {
                if (SeatOwner.TryGetValue(s.ConnectionId, out var owner) && ReferenceEquals(owner, win))
                    SeatOwner.Remove(s.ConnectionId);
            }
            Console.WriteLine($"[MM] window retired gen={win.Generation} reason={reason} seats={win.Seats.Count}");
        }

        private static List<MatchMakingUserDataMpo> RosterOf(List<Seat> seats)
        {
            var list = new List<MatchMakingUserDataMpo>();
            foreach (var s in seats)
                list.Add(new MatchMakingUserDataMpo(LobbyNameOf(s, seats), s.AvatarId, s.Grade, s.ViewerId));
            return list;
        }

        private static List<MatchMakingUserDataMpo> RosterOf(List<Seat> humans, List<Seat> aiSeats)
        {
            var list = RosterOf(humans);
            foreach (var s in aiSeats)
                list.Add(new MatchMakingUserDataMpo(s.Name, s.AvatarId, s.Grade, s.ViewerId));
            return list;
        }

        private static Seat MakeAiSeat(int seatIndex) => new Seat
        {
            ViewerId = BattleRegistry.AiViewerId(seatIndex),
            Name = BattleRegistry.AiNames[(seatIndex - 1) % BattleRegistry.AiNames.Length],
            AvatarId = BattleRegistry.AiAvatarId(seatIndex),
            Grade = BattleRegistry.AiGrade(seatIndex),
        };

        private static string LobbyNameOf(Seat s, List<Seat> seats)
        {
            var display = DisplayNameOf(s, seats);

            return s.EarlyStart && EarlyStartEnabled ? EarlyStartMark + display : display;
        }

        private static string DisplayNameOf(Seat s, List<Seat> seats)
        {
            var dup = false;
            foreach (var o in seats)
            {
                if (o == s || o.Name != s.Name) continue;
                dup = true;
                break;
            }
            if (!dup) return s.Name;
            return s.Name + "#" + (s.ViewerId % 1000).ToString("000");
        }

        private static void BroadcastRoster(Window win)
        {
            List<MatchMakingUserDataMpo> roster;
            lock (Gate) roster = RosterOf(win.Seats, win.AiSeats);
            if (roster.Count == 0) return;
            BroadcastJoin(win, roster);
        }

        private static void BroadcastJoin(Window win, List<MatchMakingUserDataMpo> roster)
        {
            var group = win.Group;
            if (group == null) return;

            group.CreateBroadcaster<IMatchMakingHubReceiver>().OnJoin(
                new MatchMakingJoinResponse { GroupUsers = roster.ToArray() });
            Console.WriteLine($"[MM] OnJoin gen={win.Generation} users={roster.Count}");
        }

        private static async Task RunWindowAsync(Window win)
        {
            try
            {
                var nextHeartbeatUtc = DateTime.UtcNow.AddSeconds(8);
                while (true)
                {
                    int phase;
                    bool dirty;
                    bool allStale;
                    lock (Gate)
                    {
                        phase = EvaluateLocked(win);
                        dirty = win.RosterDirty;
                        win.RosterDirty = false;

                        allStale = win.Seats.Count > 0;
                        foreach (var s in win.Seats)
                            if (!s.StaleAtUtc.HasValue) { allStale = false; break; }
                    }
                    if (phase < 0) return;
                    if (dirty) BroadcastRoster(win);
                    if (phase > 0) break;

                    if (!allStale && DateTime.UtcNow >= nextHeartbeatUtc)
                    {
                        nextHeartbeatUtc = DateTime.UtcNow.AddSeconds(8);
                        BroadcastRoster(win);
                    }
                    await Task.Delay(allStale ? 5000 : 100);
                }

                await Task.Delay(PreStartMs);

                List<Seat> humans;
                int dummyKind;
                lock (Gate)
                {
                    humans = new List<Seat>(win.Seats);
                    dummyKind = win.DummyKind;

                    if (humans.Count == 0)
                    {
                        RetireLocked(win, "empty");
                        return;
                    }
                }

                var session = BattleRegistry.CreateMatch(dummyKind, ToEntrants(humans));
                lock (Gate)
                {
                    win.Settled = true;
                    Interlocked.Increment(ref _settledTotal);
                    RetireLocked(win, "settled");
                }

                try
                {

                    List<IGroup> targets;
                    lock (Gate) targets = win.OwnChannels.ToList();
                    var complete = new MatchMakingCompleteResponse
                    {
                        BattleId = session.BattleId,
                        RouteToken = session.RouteToken,
                    };
                    var delivered = 0;
                    foreach (var own in targets)
                    {
                        try
                        {
                            own.CreateBroadcaster<IMatchMakingHubReceiver>().OnComplete(complete);
                            delivered++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[MM] OnComplete unicast FAIL battle={session.BattleId} " +
                                $"own={own} : {ex.GetType().Name}: {ex.Message}");
                        }
                    }
                    Console.WriteLine($"[MM] OnComplete sent battleId={session.BattleId} gen={win.Generation} " +
                        $"humans={humans.Count} delivered={delivered}/{targets.Count}");
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"[MM] completion FAILED gen={win.Generation} {ex.GetType().Name}: " +
                        $"{ex.Message} -> all players OnCancel, back to matchmaking");
                    List<IGroup> targets;
                    lock (Gate) targets = win.OwnChannels.ToList();
                    foreach (var own in targets)
                    {
                        try
                        {
                            own.CreateBroadcaster<IMatchMakingHubReceiver>().OnCancel(new MatchMakingCancelResponse
                            {
                                CanceledEntryId = win.EntryId,
                                GroupUsers = new MatchMakingUserDataMpo[0]
                            });
                        }
                        catch { }
                    }
                    lock (Gate) RetireLocked(win, "completion-failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MM] window EX gen={win.Generation} {ex.GetType().Name}: {ex.Message}");
                lock (Gate) RetireLocked(win, "exception");
            }
        }

        private static int EvaluateLocked(Window win)
        {
            if (win.Seats.Count == 0)
            {
                RetireLocked(win, "empty");
                return -1;
            }
            if (win.Seats.Count >= BattleRegistry.SeatCount)
            {

                win.Accepting = false;
                return 1;
            }

            int votes = 0;
            for (int i = 0; i < win.Seats.Count; i++)
                if (win.Seats[i].EarlyStart) votes++;
            bool majorityVote = EarlyStartEnabled && votes * 2 > win.Seats.Count;

            if (!win.Filling)
            {
                if (DateTime.UtcNow >= win.DeadlineUtc)
                {
                    win.Accepting = false;
                    StartFillLocked(win, JoinCadenceMs, false);
                }
                else if (majorityVote) StartFillLocked(win, VoteFillCadenceMs, true);
                return 0;
            }
            if (win.AiSeats.Count > 0 && (win.Seats.Count != win.FillHumanBase
                    || (win.FillingCancellable && !majorityVote)))
            {

                WithdrawAiLocked(win);
                return 0;
            }
            if (win.Seats.Count + win.AiSeats.Count >= BattleRegistry.SeatCount)
            {
                win.Filling = false;
                win.Accepting = false;
                return 1;
            }
            if (DateTime.UtcNow >= win.NextAiAtUtc)
            {
                win.AiSeats.Add(MakeAiSeat(win.Seats.Count + win.AiSeats.Count));
                win.NextAiAtUtc = DateTime.UtcNow.AddMilliseconds(win.FillCadenceMs);
                win.RosterDirty = true;
            }
            return 0;
        }

        private static void StartFillLocked(Window win, int cadenceMs, bool cancellable)
        {
            win.Filling = true;
            win.FillingCancellable = cancellable;
            win.FillCadenceMs = cadenceMs;
            win.FillHumanBase = win.Seats.Count;
            win.NextAiAtUtc = DateTime.UtcNow;
            int votes = 0;
            for (int i = 0; i < win.Seats.Count; i++)
                if (win.Seats[i].EarlyStart) votes++;
            Console.WriteLine($"[MM] fill start gen={win.Generation} cadence={cadenceMs}ms " +
                $"cancellable={cancellable} humans={win.FillHumanBase} votes={votes} " +
                $"need={BattleRegistry.SeatCount - win.FillHumanBase}");
        }

        private static void WithdrawAiLocked(Window win)
        {
            var n = win.AiSeats.Count;
            win.AiSeats.Clear();
            win.Filling = false;
            win.FillingCancellable = false;
            win.FillHumanBase = 0;
            win.RosterDirty = true;
            Console.WriteLine($"[MM] fill withdrawn gen={win.Generation} ai={n} humans={win.Seats.Count} " +
                $"deadlinePassed={DateTime.UtcNow >= win.DeadlineUtc}");
        }

        private static List<BattleRegistry.HumanEntrant> ToEntrants(List<Seat> seats)
        {
            var list = new List<BattleRegistry.HumanEntrant>(seats.Count);
            foreach (var s in seats)
            {
                var display = DisplayNameOf(s, seats);
                if (display != s.Name)
                    Console.WriteLine($"[MM] duplicate-name handling viewer={s.ViewerId} name={s.Name} -> table display {display}");
                list.Add(new BattleRegistry.HumanEntrant
                {
                    ViewerId = s.ViewerId, Name = display, AvatarId = s.AvatarId, Grade = s.Grade
                });
            }
            return list;
        }
    }
}
