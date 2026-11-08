using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using Byac.Services.Battle;

namespace Byac.Services
{

    public static class ServerMetrics
    {
        private static readonly DateTime StartedAtUtc = DateTime.UtcNow;

        private static long _battlesCreated;
        private static long _framesSent;
        private static long _eventsSent;
        private static long _reconnectReplays;
        private static long _reconnectReplayEvents;
        private static long _broadcastErrors;

        private static readonly double[] LagBuckets = { 0.05, 0.1, 0.25, 0.5, 1, 2, 5 };
        private static readonly long[] LagBucketCounts = new long[LagBuckets.Length];
        private static long _lagSumMicros;
        private static long _lagCount;

        private sealed class PortCounters
        {
            public long Requests;
            public long Active;
        }

        private static readonly ConcurrentDictionary<int, PortCounters> Ports = new();

        public static void BattleCreated() => Interlocked.Increment(ref _battlesCreated);
        public static void BroadcastError() => Interlocked.Increment(ref _broadcastErrors);

        public static void BattleFrameSent(int eventCount, float lagSeconds)
        {
            Interlocked.Increment(ref _framesSent);
            Interlocked.Add(ref _eventsSent, eventCount);
            var lag = Math.Max(0f, lagSeconds);
            for (var i = 0; i < LagBuckets.Length; i++)
            {
                if (lag <= LagBuckets[i])
                {
                    Interlocked.Increment(ref LagBucketCounts[i]);
                    break;
                }
            }
            Interlocked.Add(ref _lagSumMicros, (long)(lag * 1_000_000f));
            Interlocked.Increment(ref _lagCount);
        }

        public static void ReconnectReplay(int events)
        {
            Interlocked.Increment(ref _reconnectReplays);
            Interlocked.Add(ref _reconnectReplayEvents, events);
        }

        public static void HttpRequestStarted(int port)
        {
            var c = Ports.GetOrAdd(port, _ => new PortCounters());
            Interlocked.Increment(ref c.Requests);
            Interlocked.Increment(ref c.Active);
        }

        public static void HttpRequestFinished(int port)
        {
            if (Ports.TryGetValue(port, out var c)) Interlocked.Decrement(ref c.Active);
        }

        public static string Render()
        {
            var sb = new StringBuilder(4096);
            var build = BuildInfo.Version;
            sb.Append("# HELP byac_build_info server version (label matches startup log)\n");
            sb.Append("# TYPE byac_build_info gauge\n");
            sb.Append("byac_build_info{version=\"").Append(build).Append("\"} 1\n");

            using var proc = Process.GetCurrentProcess();
            double uptime;
            try { uptime = Math.Max(0, (DateTime.UtcNow - proc.StartTime.ToUniversalTime()).TotalSeconds); }
            catch { uptime = Math.Max(0, (DateTime.UtcNow - StartedAtUtc).TotalSeconds); }
            sb.Append("# HELP byac_uptime_seconds process uptime\n");
            sb.Append("# TYPE byac_uptime_seconds gauge\n");
            sb.Append("byac_uptime_seconds ").Append(Num(uptime)).Append('\n');

            try { proc.Refresh(); } catch {  }
            sb.Append("# HELP byac_process_cpu_seconds_total cumulative process CPU seconds (can exceed uptime on multicore)\n");
            sb.Append("# TYPE byac_process_cpu_seconds_total counter\n");
            sb.Append("byac_process_cpu_seconds_total ").Append(Num(SafeDouble(() => proc.TotalProcessorTime.TotalSeconds))).Append('\n');
            sb.Append("# HELP byac_process_working_set_bytes working set bytes\n");
            sb.Append("# TYPE byac_process_working_set_bytes gauge\n");
            sb.Append("byac_process_working_set_bytes ").Append(Safe(() => proc.WorkingSet64)).Append('\n');
            sb.Append("# HELP byac_process_private_bytes private bytes\n");
            sb.Append("# TYPE byac_process_private_bytes gauge\n");
            sb.Append("byac_process_private_bytes ").Append(Safe(() => proc.PrivateMemorySize64)).Append('\n');
            sb.Append("# HELP byac_process_threads thread count\n");
            sb.Append("# TYPE byac_process_threads gauge\n");
            sb.Append("byac_process_threads ").Append(Safe(() => proc.Threads.Count)).Append('\n');

            sb.Append("# HELP byac_dotnet_gc_heap_bytes GC managed heap bytes\n");
            sb.Append("# TYPE byac_dotnet_gc_heap_bytes gauge\n");
            sb.Append("byac_dotnet_gc_heap_bytes ").Append(GC.GetTotalMemory(false)).Append('\n');
            sb.Append("# HELP byac_dotnet_gc_collections_total GC collection count\n");
            sb.Append("# TYPE byac_dotnet_gc_collections_total counter\n");
            for (var gen = 0; gen <= 2; gen++)
                sb.Append("byac_dotnet_gc_collections_total{generation=\"").Append(gen).Append("\"} ")
                  .Append(GC.CollectionCount(gen)).Append('\n');

            var stats = BattleRegistry.Stats();
            sb.Append("# HELP byac_battles_active active battles\n");
            sb.Append("# TYPE byac_battles_active gauge\n");
            sb.Append("byac_battles_active ").Append(stats.Active).Append('\n');
            sb.Append("# HELP byac_battles_retained in-memory sessions (including 30-minute retention after finish)\n");
            sb.Append("# TYPE byac_battles_retained gauge\n");
            sb.Append("byac_battles_retained ").Append(stats.Retained).Append('\n');
            sb.Append("# HELP byac_battle_viewers_attached human seats currently attached to hub\n");
            sb.Append("# TYPE byac_battle_viewers_attached gauge\n");
            sb.Append("byac_battle_viewers_attached ").Append(stats.AttachedHumans).Append('\n');
            sb.Append("# HELP byac_battles_created_total battles created total (matchmaking + solo/tutorial)\n");
            sb.Append("# TYPE byac_battles_created_total counter\n");
            sb.Append("byac_battles_created_total ").Append(Interlocked.Read(ref _battlesCreated)).Append('\n');

            sb.Append("# HELP byac_battle_frames_sent_total battle frames broadcast (50ms buckets)\n");
            sb.Append("# TYPE byac_battle_frames_sent_total counter\n");
            sb.Append("byac_battle_frames_sent_total ").Append(Interlocked.Read(ref _framesSent)).Append('\n');
            sb.Append("# HELP byac_battle_events_sent_total battle events broadcast\n");
            sb.Append("# TYPE byac_battle_events_sent_total counter\n");
            sb.Append("byac_battle_events_sent_total ").Append(Interlocked.Read(ref _eventsSent)).Append('\n');
            sb.Append("# HELP byac_battle_broadcast_errors_total broadcast errors (log [BATTLE] broadcast EX)\n");
            sb.Append("# TYPE byac_battle_broadcast_errors_total counter\n");
            sb.Append("byac_battle_broadcast_errors_total ").Append(Interlocked.Read(ref _broadcastErrors)).Append('\n');
            sb.Append("# HELP byac_battle_reconnect_replays_total reconnect replays (full replay of already-broadcast frames)\n");
            sb.Append("# TYPE byac_battle_reconnect_replays_total counter\n");
            sb.Append("byac_battle_reconnect_replays_total ").Append(Interlocked.Read(ref _reconnectReplays)).Append('\n');
            sb.Append("# HELP byac_battle_reconnect_replay_events_total events replayed on reconnect\n");
            sb.Append("# TYPE byac_battle_reconnect_replay_events_total counter\n");
            sb.Append("byac_battle_reconnect_replay_events_total ").Append(Interlocked.Read(ref _reconnectReplayEvents)).Append('\n');

            var lagCount = Interlocked.Read(ref _lagCount);
            sb.Append("# HELP byac_battle_frame_send_lag_seconds frame send lag (actual send - scheduled, seconds)\n");
            sb.Append("# TYPE byac_battle_frame_send_lag_seconds histogram\n");
            long cumulative = 0;
            for (var i = 0; i < LagBuckets.Length; i++)
            {
                cumulative += Interlocked.Read(ref LagBucketCounts[i]);
                sb.Append("byac_battle_frame_send_lag_seconds_bucket{le=\"").Append(Num(LagBuckets[i])).Append("\"} ")
                  .Append(cumulative).Append('\n');
            }
            sb.Append("byac_battle_frame_send_lag_seconds_bucket{le=\"+Inf\"} ").Append(lagCount).Append('\n');
            sb.Append("byac_battle_frame_send_lag_seconds_sum ")
              .Append(Num(Interlocked.Read(ref _lagSumMicros) / 1_000_000.0)).Append('\n');
            sb.Append("byac_battle_frame_send_lag_seconds_count ").Append(lagCount).Append('\n');

            var mm = MatchMakingHub.Metrics();
            sb.Append("# HELP byac_matchmaking_windows_open open matchmaking windows\n");
            sb.Append("# TYPE byac_matchmaking_windows_open gauge\n");
            sb.Append("byac_matchmaking_windows_open ").Append(mm.OpenWindows).Append('\n');
            sb.Append("# HELP byac_matchmaking_windows_accepting matchmaking windows still accepting\n");
            sb.Append("# TYPE byac_matchmaking_windows_accepting gauge\n");
            sb.Append("byac_matchmaking_windows_accepting ").Append(mm.AcceptingWindows).Append('\n');
            sb.Append("# HELP byac_matchmaking_seats_waiting human seats waiting in matchmaking\n");
            sb.Append("# TYPE byac_matchmaking_seats_waiting gauge\n");
            sb.Append("byac_matchmaking_seats_waiting ").Append(mm.WaitingSeats).Append('\n');
            sb.Append("# HELP byac_matchmaking_entries_total total entries\n");
            sb.Append("# TYPE byac_matchmaking_entries_total counter\n");
            sb.Append("byac_matchmaking_entries_total ").Append(mm.EntryTotal).Append('\n');
            sb.Append("# HELP byac_matchmaking_settled_total total settled (full window / early-start vote)\n");
            sb.Append("# TYPE byac_matchmaking_settled_total counter\n");
            sb.Append("byac_matchmaking_settled_total ").Append(mm.SettledTotal).Append('\n');
            sb.Append("# HELP byac_matchmaking_duplicate_rejects_total duplicate entry rejects\n");
            sb.Append("# TYPE byac_matchmaking_duplicate_rejects_total counter\n");
            sb.Append("byac_matchmaking_duplicate_rejects_total ").Append(mm.DuplicateRejects).Append('\n');

            sb.Append("# HELP byac_http_requests_total cumulative requests per listener port\n");
            sb.Append("# TYPE byac_http_requests_total counter\n");
            sb.Append("# HELP byac_http_active_streams in-flight requests/streams per listener port (hub long connection counts as 1)\n");
            sb.Append("# TYPE byac_http_active_streams gauge\n");
            foreach (var kv in Ports)
            {
                sb.Append("byac_http_requests_total{port=\"").Append(kv.Key).Append("\"} ")
                  .Append(Interlocked.Read(ref kv.Value.Requests)).Append('\n');
            }
            foreach (var kv in Ports)
            {
                sb.Append("byac_http_active_streams{port=\"").Append(kv.Key).Append("\"} ")
                  .Append(Interlocked.Read(ref kv.Value.Active)).Append('\n');
            }

            return sb.ToString();
        }

        private static string Num(double v) => v.ToString("0.######", CultureInfo.InvariantCulture);

        private static long Safe(Func<long> read)
        {
            try { return read(); } catch { return 0; }
        }

        private static double SafeDouble(Func<double> read)
        {
            try { return read(); } catch { return 0; }
        }
    }
}
