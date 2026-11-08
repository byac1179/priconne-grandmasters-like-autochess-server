using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using Byac.ServerShared.Hub;

namespace Byac.Services.Battle
{

    public sealed class NetTrace
    {
        private static readonly object InitLock = new object();
        private static bool _initialized;
        private static bool _enabled;

        public static bool Enabled
        {
            get
            {
                if (_initialized) return _enabled;
                lock (InitLock)
                {
                    if (_initialized) return _enabled;
                    var v = Environment.GetEnvironmentVariable("BYAC_NETTRACE");
                    _enabled = !(string.Equals(v, "0", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(v, "off", StringComparison.OrdinalIgnoreCase));

                    var cfg = $"nettrace={(_enabled ? 1 : 0)} " +
                              $"frameLog={(BattleSession.FrameLogOn ? 1 : 0)} " +
                              $"snap={(BattleSession.BoardSnapshotOn ? 1 : 0)} " +
                              $"bhFilter={(Byac.Services.TraceHubFilterAttribute.TraceCallLog ? 1 : 0)} " +
                              $"simLog={(SimpleTickSimulator.VerboseSimLog ? 1 : 0)}";
                    if (_enabled)
                    {

                        var svr = false;
                        try { svr = System.Runtime.GCSettings.IsServerGC; } catch { }
                        Console.WriteLine($"[NETTRACE] enabled (disable: BYAC_NETTRACE=0): round / in / tcp / gc lines, gc face serverGC={(svr ? 1 : 0)} {cfg}");
                    }
                    else Console.WriteLine($"[NETTRACE] disabled (enable: unset BYAC_NETTRACE=0): existing log surfaces only {cfg}");
                    _initialized = true;
                }
                return _enabled;
            }
        }

        private static DateTime _lastActivityUtc = DateTime.MinValue;
        private static int _samplerStarted;

        private readonly string _battleId;
        private readonly ConcurrentDictionary<string, Profile> _round = new();
        private readonly ConcurrentDictionary<string, Profile> _total = new();
        private readonly Dictionary<int, DateTime> _lastInboundByViewer = new();
        private readonly object _inLock = new object();
        private readonly Hist _inGap = new Hist(500, 1000, 2000, 5000, 10000, 30000);
        private readonly object _lock = new object();
        private long _inboundCount;
        private DateTime _roundStartUtc = DateTime.UtcNow;

        public NetTrace(string battleId)
        {
            _battleId = battleId ?? "";
        }

        public IBattleHubReceiver Wrap(IBattleHubReceiver inner, string target)
        {
            var proxy = DispatchProxy.Create<IBattleHubReceiver, NetTraceProxy>();
            var p = (NetTraceProxy)(object)proxy;
            p.Inner = inner;
            p.Trace = this;
            p.Target = target;
            return proxy;
        }

        public void NoteFrame(string target, string method, object[] args, double sendMs)
        {
            if (!Enabled) return;
            long bytes;
            try { bytes = EstimateBytes(args); }
            catch { bytes = 0; }
            var now = DateTime.UtcNow;
            lock (_lock)
            {
                var p = Pick(_round, Key(target));
                p.Frames++;
                p.Bytes += bytes;
                if (bytes > p.BytesMax) p.BytesMax = bytes;
                p.SendMs.Add(sendMs);
                p.Methods.TryGetValue(method, out var m);
                p.Methods[method] = (m.Frames + 1, m.Bytes + bytes);
                var gap = (now - p.LastUtc).TotalMilliseconds;
                if (gap >= 0 && gap < 60_000) p.Gap.Add(gap);
                p.LastUtc = now;
                _lastActivityUtc = now;
            }
        }

        public void NoteDrift(string target, double driftMs)
        {
            if (!Enabled) return;
            lock (_lock) Pick(_round, Key(target)).DriftMs.Add(Math.Max(0, driftMs));
        }

        public void NoteInbound(int viewerId)
        {
            if (!Enabled) return;
            var now = DateTime.UtcNow;
            lock (_inLock)
            {
                if (_lastInboundByViewer.TryGetValue(viewerId, out var last))
                {
                    var gap = (now - last).TotalMilliseconds;
                    if (gap >= 0 && gap < 60_000) _inGap.Add(gap);
                }
                _lastInboundByViewer[viewerId] = now;
                _inboundCount++;
            }
        }

        private string Key(string target)
        {

            var via = target != null && target.StartsWith("viewer", StringComparison.OrdinalIgnoreCase) ? "uni" : "room";
            return _battleId + "|" + via;
        }

        private static Profile Pick(ConcurrentDictionary<string, Profile> map, string key)
        {
            if (!map.TryGetValue(key, out var p))
            {
                p = new Profile();
                map[key] = p;
            }
            return p;
        }

        public void DumpRound(int round)
        {
            if (!Enabled) return;
            var now = DateTime.UtcNow;
            lock (_lock)
            {
                var secs = Math.Max(1, (int)Math.Round((now - _roundStartUtc).TotalSeconds));
                _roundStartUtc = now;
                var mine = _round.Keys.Where(k => k.StartsWith(_battleId + "|")).ToList();
                if (mine.Count == 0)
                    Console.WriteLine($"[NETTRACE] round={round} battle={_battleId} no-frames secs={secs}");
                foreach (var k in mine)
                {
                    if (!_round.TryGetValue(k, out var r)) continue;
                    _round.TryRemove(k, out _);
                    var t = Pick(_total, k);
                    t.MergeFrom(r);
                    Console.WriteLine($"[NETTRACE] round={round} battle={_battleId} via={k.Split('|')[1]} secs={secs} {r.Render(secs)}");
                }
                long inN;
                Hist snap;
                lock (_inLock)
                {
                    inN = _inboundCount;
                    snap = new Hist(500, 1000, 2000, 5000, 10000, 30000);
                    snap.Merge(_inGap);
                    _inboundCount = 0;
                    _inGap.Reset();
                }
                Console.WriteLine($"[NETTRACE] round={round} battle={_battleId} in={inN} seats={_lastInboundByViewer.Count} {snap.Render("inGapMs")}");
            }
            StartSamplerOnce();
        }

        public void DumpBattle()
        {
            if (!Enabled) return;
            lock (_lock)
            {
                var mine = _total.Keys.Where(k => k.StartsWith(_battleId + "|")).ToList();
                if (mine.Count == 0)
                {
                    Console.WriteLine($"[NETTRACE] battle={_battleId} total has no delivery samples (hub not connected / frames did not take this path)");
                    return;
                }
                foreach (var k in mine)
                {
                    if (!_total.TryRemove(k, out var t)) continue;
                    Console.WriteLine($"[NETTRACE] battle={_battleId} total via={k.Split('|')[1]} {t.Render(0)}");
                }
            }
        }

        private sealed class Profile
        {
            public long Frames;
            public long Bytes;
            public long BytesMax;
            public DateTime LastUtc = DateTime.MinValue;
            public readonly Hist Gap = new Hist(20, 50, 100, 250, 500, 1000);
            public readonly Hist SendMs = new Hist(0.1, 0.5, 2, 10, 50);
            public readonly Hist DriftMs = new Hist(0, 20, 50, 100, 250, 1000);
            public readonly Dictionary<string, (long Frames, long Bytes)> Methods = new();

            public void MergeFrom(Profile o)
            {
                Frames += o.Frames;
                Bytes += o.Bytes;
                if (o.BytesMax > BytesMax) BytesMax = o.BytesMax;
                Gap.Merge(o.Gap);
                SendMs.Merge(o.SendMs);
                DriftMs.Merge(o.DriftMs);
                foreach (var kv in o.Methods)
                {
                    Methods.TryGetValue(kv.Key, out var cur);
                    Methods[kv.Key] = (cur.Frames + kv.Value.Frames, cur.Bytes + kv.Value.Bytes);
                }
            }

            public string Render(int windowSeconds)
            {
                var top = Methods.Count == 0
                    ? "-"
                    : string.Join(" ", Methods.OrderByDescending(m => m.Value.Bytes).Take(3)
                        .Select(m => $"{m.Key}#{m.Value.Frames}/{m.Value.Bytes / 1024}K"));
                var peak = windowSeconds <= 0 ? 0 : Bytes / 1024 / windowSeconds;
                return $"frames={Frames} kb={Bytes / 1024} peakKBs={peak} maxFrameB={BytesMax} " +
                       $"{Gap.Render("gapMs")} {SendMs.Render("sendMs")} {DriftMs.Render("driftMs")} top=[{top}]";
            }
        }

        private sealed class Hist
        {
            private readonly long[] _counts;
            private readonly double[] _edges;
            public long N;
            public double Sum;
            public double Max;
            public long Over;

            public Hist(params double[] edges)
            {
                _edges = edges;
                _counts = new long[edges.Length];
            }

            public void Add(double v)
            {
                N++;
                Sum += v;
                if (v > Max) Max = v;
                for (var i = 0; i < _edges.Length; i++)
                {
                    if (v <= _edges[i]) { _counts[i]++; return; }
                }
                _counts[_edges.Length - 1]++;
                Over++;
            }

            public void Merge(Hist o)
            {
                if (o._counts.Length != _counts.Length) return;
                N += o.N;
                Sum += o.Sum;
                Over += o.Over;
                if (o.Max > Max) Max = o.Max;
                for (var i = 0; i < _counts.Length; i++) _counts[i] += o._counts[i];
            }

            public void Reset()
            {
                N = 0;
                Sum = 0;
                Max = 0;
                Over = 0;
                for (var i = 0; i < _counts.Length; i++) _counts[i] = 0;
            }

            public double Mean => N == 0 ? 0 : Sum / N;

            public double P50Bucket()
            {
                if (N == 0) return -1;
                long acc = 0;
                var half = (N + 1) / 2;
                for (var i = 0; i < _edges.Length; i++)
                {
                    acc += _counts[i];
                    if (acc >= half) return _edges[i];
                }
                return _edges[_edges.Length - 1];
            }

            public string Render(string name)
            {
                if (N == 0) return name + ":n0";
                var s = $"{name}:n{N}/avg{F(Mean)}/p50<={F(P50Bucket())}/max{F(Max)}";
                if (Over > 0) s += $"/>{F(_edges[_edges.Length - 1])}:{Over}";
                return s;
            }

            private static string F(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);
        }

        [ThreadStatic] private static MemoryStream JsonStream;
        [ThreadStatic] private static Utf8JsonWriter JsonWriter;

        private static long EstimateBytes(object[] args)
        {
            if (args == null || args.Length == 0) return 0;
            var total = 0;
            for (var i = 0; i < args.Length; i++)
            {
                var a = args[i];
                if (a == null) continue;

                if (a is Array arr)
                {
                    for (var j = 0; j < arr.Length; j++)
                    {
                        var el = arr.GetValue(j);
                        if (el != null) total += SizeOf(el);
                    }
                }
                else total += SizeOf(a);
            }
            return total;
        }

        private static int SizeOf(object value)
        {
            var ms = JsonStream ??= new MemoryStream(8192);
            var w = JsonWriter ??= new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false });
            try
            {
                JsonSerializer.Serialize(w, value, value.GetType());
                w.Flush();
                return (int)ms.Length;
            }
            finally
            {
                w.Reset();
                ms.SetLength(0);
                ms.Position = 0;
            }
        }

        private static int _gc0 = -1, _gc1 = -1, _gc2 = -1;
        private static long _gcAlloc = -1;
        private static TimeSpan _gcPause = TimeSpan.Zero;
        private static DateTime _lastGcLineUtc = DateTime.MinValue;

        private static void SampleGc()
        {
            var now = DateTime.UtcNow;
            if ((now - _lastActivityUtc).TotalSeconds > 90) return;
            if ((now - _lastGcLineUtc).TotalSeconds < ReportEverySec) return;

            var c0 = GC.CollectionCount(0);
            var c1 = GC.CollectionCount(1);
            var c2 = GC.CollectionCount(2);
            long alloc;
            try { alloc = GC.GetTotalAllocatedBytes(true); } catch { alloc = -1; }
            var pause = TimeSpan.Zero;
            try { pause = GC.GetTotalPauseDuration(); } catch { }
            long heap;

            try { heap = GC.GetTotalMemory(false); } catch { heap = -1; }

            var win = (int)Math.Round((now - _lastGcLineUtc).TotalSeconds);
            _lastGcLineUtc = now;

            if (_gc0 < 0 || win <= 0)
            {
                _gc0 = c0; _gc1 = c1; _gc2 = c2; _gcAlloc = alloc; _gcPause = pause;
                return;
            }

            var mb = 1024.0 * 1024.0;
            Console.WriteLine($"[NETTRACE-gc] win={win}s gen0=+{c0 - _gc0} gen1=+{c1 - _gc1} gen2=+{c2 - _gc2} " +
                              $"pauseMs={(pause - _gcPause).TotalMilliseconds.ToString("0.#", CultureInfo.InvariantCulture)} " +
                              $"allocMB={(alloc < 0 || _gcAlloc < 0 ? -1 : (alloc - _gcAlloc) / mb).ToString("0", CultureInfo.InvariantCulture)} " +
                              $"heapMB={(heap < 0 ? -1 : heap / mb).ToString("0", CultureInfo.InvariantCulture)}");
            _gc0 = c0; _gc1 = c1; _gc2 = c2; _gcAlloc = alloc; _gcPause = pause;
        }

        private static void StartSamplerOnce()
        {
            if (Interlocked.CompareExchange(ref _samplerStarted, 1, 0) != 0) return;
            new Thread(SamplerLoop) { IsBackground = true, Name = "nettrace-tcp" }.Start();
        }

        private static void SamplerLoop()
        {
            var ports = ListenPorts();
            var tcpDead = false;
            while (true)
            {
                Thread.Sleep(SampleEveryMs);
                SampleGc();
                if (tcpDead) continue;
                List<TcpPortSnap> snap;
                try { snap = Sample(ports); }
                catch
                {

                    Console.WriteLine("[NETTRACE-tcp] cannot read /proc/net/tcp, this surface is off (delivery/uplink/GC unaffected)");
                    tcpDead = true;
                    continue;
                }
                var now = DateTime.UtcNow;
                if ((now - _lastActivityUtc).TotalSeconds > 90) continue;
                foreach (var s in snap)
                {
                    var due = (now - _lastTcpLineUtc).TotalSeconds >= ReportEverySec;
                    if (!due && s.TxMax < AlertTxQueueBytes) continue;
                    Console.WriteLine($"[NETTRACE-tcp] port={s.Port} estab={s.Estab} txqMax={s.TxMax} " +
                                      $"txqSum={s.TxSum} rxqMax={s.RxMax} retrMax={s.Retr}");
                    _lastTcpLineUtc = now;
                }
            }
        }

        private sealed class TcpPortSnap
        {
            public int Port;
            public int Estab;
            public long TxMax;
            public long TxSum;
            public long RxMax;
            public long Retr;
        }

        private const int SampleEveryMs = 5000;
        private const int ReportEverySec = 30;
        private const long AlertTxQueueBytes = 64 * 1024;

        private static HashSet<int> ListenPorts()
        {
            var set = new HashSet<int>();
            void Add(int p) { if (p > 0) set.Add(p); }
            try { Add(Byac.Server.Security.PublicGuardOptions.H2cPort); } catch { }
            try { Add(Byac.Server.Security.PublicGuardOptions.TlsPort); } catch { }
            Add(ReadPort("BYAC_GM_PORT", 9445));
            try { Add(Byac.Server.MetricsEndpoint.Port); } catch { }
            return set;
        }

        private static int ReadPort(string env, int dflt)
        {
            var v = Environment.GetEnvironmentVariable(env);
            return int.TryParse(v, out var p) ? p : dflt;
        }

        private static DateTime _lastTcpLineUtc = DateTime.MinValue;

        private static List<TcpPortSnap> Sample(HashSet<int> ports)
        {
            var byPort = new Dictionary<int, TcpPortSnap>();
            foreach (var path in new[] { "/proc/net/tcp", "/proc/net/tcp6" })
            {
                if (!File.Exists(path)) continue;
                var lines = File.ReadAllLines(path);
                for (var i = 1; i < lines.Length; i++)
                {
                    var f = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (f.Length < 7) continue;
                    var local = ParsePort(f[1]);
                    if (local <= 0 || !ports.Contains(local)) continue;
                    var st = Convert.ToInt32(f[3], 16);
                    if (st == 0x0A) continue;
                    var q = f[4].Split('/');
                    var tx = ParseHex(q.Length > 0 ? q[0] : null);
                    var rx = q.Length > 1 ? ParseHex(q[1]) : 0;
                    var retr = ParseHex(f[6]);
                    if (!byPort.TryGetValue(local, out var s))
                    {
                        s = new TcpPortSnap { Port = local };
                        byPort[local] = s;
                    }
                    if (st == 0x01) s.Estab++;
                    s.TxSum += tx;
                    if (tx > s.TxMax) s.TxMax = tx;
                    if (rx > s.RxMax) s.RxMax = rx;
                    if (retr > s.Retr) s.Retr = retr;
                }
            }
            return byPort.Values.OrderBy(s => s.Port).ToList();
        }

        private static int ParsePort(string addr)
        {
            var k = addr.IndexOf(':');
            if (k < 0 || k + 5 > addr.Length) return -1;
            try { return Convert.ToInt32(addr.Substring(k + 1, 4), 16); }
            catch { return -1; }
        }

        private static long ParseHex(string s)
        {
            try { return string.IsNullOrEmpty(s) ? 0 : Convert.ToInt64(s, 16); }
            catch { return 0; }
        }
    }

    public class NetTraceProxy : DispatchProxy
    {
        internal IBattleHubReceiver Inner;
        internal NetTrace Trace;
        internal string Target;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var start = Stopwatch.GetTimestamp();
            try
            {
                return targetMethod.Invoke(Inner, args);
            }
            finally
            {
                var ms = (Stopwatch.GetTimestamp() - start) * 1000.0 / Stopwatch.Frequency;
                try { Trace?.NoteFrame(Target, targetMethod?.Name ?? "?", args, ms); }
                catch { }
            }
        }
    }
}
