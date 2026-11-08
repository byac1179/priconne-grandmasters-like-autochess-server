#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using Byac.Services.Battle;

namespace Byac.Services
{

    public static class LeaderboardStore
    {
        private static readonly object Gate = new object();

        private const string Schema = @"
CREATE TABLE IF NOT EXISTS battle_lineup (
    battle_id    TEXT    NOT NULL,
    viewer_id    INTEGER NOT NULL,
    unit_ids     TEXT    NOT NULL,
    ranking      INTEGER NOT NULL,
    is_human     INTEGER NOT NULL,
    round_count  INTEGER NOT NULL,
    week_key     TEXT    NOT NULL,
    month_key    TEXT    NOT NULL,
    ended_at_utc TEXT    NOT NULL,
    PRIMARY KEY (battle_id, viewer_id)
);
CREATE INDEX IF NOT EXISTS idx_lineup_week  ON battle_lineup(week_key);
CREATE INDEX IF NOT EXISTS idx_lineup_month ON battle_lineup(month_key);";

        public readonly struct LineupRow
        {
            public LineupRow(int viewerId, bool isHuman, int ranking, IEnumerable<int> units)
            {
                ViewerId = viewerId; IsHuman = isHuman; Ranking = ranking; Units = units;
            }
            public int ViewerId { get; }
            public bool IsHuman { get; }
            public int Ranking { get; }
            public IEnumerable<int> Units { get; }
        }

        private static bool On => Environment.GetEnvironmentVariable("BYAC_LEADERBOARD") != "0";

        private static string DataDir
        {
            get
            {
                var env = Environment.GetEnvironmentVariable("BYAC_DATA_DIR");
                return string.IsNullOrEmpty(env) ? Path.Combine(Environment.CurrentDirectory, "data") : env;
            }
        }

        private static string DbPath => Path.Combine(DataDir, "leaderboard.db");

        private static int KeepMonths
        {
            get
            {
                var env = Environment.GetEnvironmentVariable("BYAC_LB_KEEP_MONTHS");
                return int.TryParse(env, out var n) && n >= 0 ? n : 2;
            }
        }

        public static string WeekKey(DateTime local) =>
            $"{ISOWeek.GetYear(local):0000}-W{ISOWeek.GetWeekOfYear(local):00}";

        public static string MonthKey(DateTime local) => local.ToString("yyyy-MM", CultureInfo.InvariantCulture);

        public static void Record(string battleId, int roundCount, IEnumerable<LineupRow> rows)
        {
            if (!On || rows == null || string.IsNullOrEmpty(battleId)) return;
            var now = DateTime.Now;
            var week = WeekKey(now);
            var month = MonthKey(now);
            var endedUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
            try
            {
                lock (Gate)
                {
                    using var conn = Open();
                    using var tx = conn.BeginTransaction();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"INSERT OR IGNORE INTO battle_lineup
                            (battle_id, viewer_id, unit_ids, ranking, is_human, round_count, week_key, month_key, ended_at_utc)
                            VALUES ($b,$v,$u,$r,$h,$rc,$w,$m,$e)";
                        var pB = cmd.Parameters.Add("$b", SqliteType.Text);
                        var pV = cmd.Parameters.Add("$v", SqliteType.Integer);
                        var pU = cmd.Parameters.Add("$u", SqliteType.Text);
                        var pR = cmd.Parameters.Add("$r", SqliteType.Integer);
                        var pH = cmd.Parameters.Add("$h", SqliteType.Integer);
                        var pRc = cmd.Parameters.Add("$rc", SqliteType.Integer);
                        var pW = cmd.Parameters.Add("$w", SqliteType.Text);
                        var pM = cmd.Parameters.Add("$m", SqliteType.Text);
                        var pE = cmd.Parameters.Add("$e", SqliteType.Text);
                        int written = 0;
                        foreach (var row in rows)
                        {
                            var units = (row.Units ?? Enumerable.Empty<int>())
                                .Where(id => id > 0).Distinct().OrderBy(id => id).ToList();
                            pB.Value = battleId;
                            pV.Value = row.ViewerId;
                            pU.Value = string.Join(",", units);
                            pR.Value = row.Ranking;
                            pH.Value = row.IsHuman ? 1 : 0;
                            pRc.Value = roundCount;
                            pW.Value = week;
                            pM.Value = month;
                            pE.Value = endedUtc;
                            written += cmd.ExecuteNonQuery();
                        }
                        Console.WriteLine($"[LB] record battle={battleId} round={roundCount} rows={written} week={week} month={month}");
                    }
                    Prune_NoLock(conn, tx, month);
                    tx.Commit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LB] SAVE FAIL battle={battleId}: {ex.GetType().Name}: {ex.Message}");
            }
        }

        private static void Prune_NoLock(SqliteConnection conn, SqliteTransaction tx, string currentMonth)
        {
            var keep = KeepMonths;
            if (keep <= 0) return;
            if (!DateTime.TryParseExact(currentMonth + "-01", "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var first)) return;
            var oldest = MonthKey(first.AddMonths(-(keep - 1)));
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = "DELETE FROM battle_lineup WHERE month_key < $oldest";
            cmd.Parameters.Add("$oldest", SqliteType.Text).Value = oldest;
            var deleted = cmd.ExecuteNonQuery();
            if (deleted > 0) Console.WriteLine($"[LB] prune months<{oldest} rows={deleted}");
        }

        public static Board Query(string period, string? bucket, bool includeAi, int limit, int minPicks)
        {
            var now = DateTime.Now;
            var board = new Board { Period = period, IncludeAi = includeAi, Disabled = !On };
            if (!On) return board;
            board.Bucket = string.IsNullOrWhiteSpace(bucket)
                ? (period == "month" ? MonthKey(now) : WeekKey(now))
                : bucket.Trim();
            var column = period == "month" ? "month_key" : "week_key";
            var picks = new Dictionary<int, int>();
            var wins = new Dictionary<int, int>();
            var champPicks = new Dictionary<int, int>();
            try
            {
                lock (Gate)
                {
                    using var conn = Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = $@"SELECT unit_ids, ranking, is_human, battle_id FROM battle_lineup
                                         WHERE {column} = $k";
                    cmd.Parameters.Add("$k", SqliteType.Text).Value = board.Bucket;
                    var seenBattles = new HashSet<string>();
                    var humanBattles = new HashSet<string>();
                    using var rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        var battle = rd.GetString(3);
                        var human = rd.GetInt32(2) != 0;
                        if (!includeAi && !human) continue;
                        board.Rows++;
                        seenBattles.Add(battle);
                        if (human) humanBattles.Add(battle);
                        var champion = rd.GetInt32(1) == 1;
                        if (champion) board.ChampionRows++;
                        foreach (var idText in rd.GetString(0).Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (!int.TryParse(idText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)) continue;
                            picks[id] = picks.GetValueOrDefault(id) + 1;
                            if (!champion) continue;
                            wins[id] = wins.GetValueOrDefault(id) + 1;
                            champPicks[id] = champPicks.GetValueOrDefault(id) + 1;
                        }
                    }
                    board.Games = seenBattles.Count;
                    board.HumanGames = humanBattles.Count;
                }
            }
            catch (Exception ex)
            {
                board.Error = $"{ex.GetType().Name}: {ex.Message}";
                return board;
            }
            board.WinRate = BuildRate(picks, wins, minPicks, limit);
            board.ChampionFrequency = BuildFrequency(champPicks, board.ChampionRows, limit);
            return board;
        }

        private static List<Entry> BuildRate(Dictionary<int, int> picks, Dictionary<int, int> wins,
            int minPicks, int limit)
        {
            var list = new List<Entry>();
            foreach (var kv in picks)
            {
                if (kv.Value < Math.Max(0, minPicks)) continue;
                var w = wins.GetValueOrDefault(kv.Key);
                list.Add(EntryOf(kv.Key, kv.Value, w, kv.Value > 0 ? (double)w / kv.Value : 0d));
            }
            return list
                .OrderByDescending(e => e.WinRate).ThenByDescending(e => e.Picks).ThenBy(e => e.UnitId)
                .Take(Math.Max(1, limit)).ToList();
        }

        private static List<Entry> BuildFrequency(Dictionary<int, int> champPicks, int championRows, int limit)
        {
            var list = new List<Entry>();
            foreach (var kv in champPicks)
                list.Add(EntryOf(kv.Key, kv.Value, kv.Value,
                    championRows > 0 ? (double)kv.Value / championRows : 0d));
            return list
                .OrderByDescending(e => e.Picks).ThenBy(e => e.UnitId)
                .Take(Math.Max(1, limit)).ToList();
        }

        private static Entry EntryOf(int unitId, int picks, int wins, double ratio) => new Entry
        {
            UnitId = unitId,
            Name = NameOf(unitId),
            Picks = picks,
            Wins = wins,
            WinRate = Math.Round(ratio, 4),
        };

        public sealed class Entry
        {
            public int UnitId { get; set; }
            public string Name { get; set; } = "";

            public int Picks { get; set; }

            public int Wins { get; set; }

            public double WinRate { get; set; }
        }

        public sealed class Board
        {
            public string Period { get; set; } = "";
            public string Bucket { get; set; } = "";
            public bool IncludeAi { get; set; }
            public bool Disabled { get; set; }
            public string? Error { get; set; }
            public int Games { get; set; }
            public int HumanGames { get; set; }
            public int Rows { get; set; }

            public int ChampionRows { get; set; }
            public List<Entry> WinRate { get; set; } = new();
            public List<Entry> ChampionFrequency { get; set; } = new();
        }

        private static SqliteConnection Open()
        {
            var dir = Path.GetDirectoryName(DbPath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            var conn = new SqliteConnection($"Data Source={DbPath}");
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = Schema;
                cmd.ExecuteNonQuery();
            }
            return conn;
        }

        private static string NameOf(int unitId)
        {
            try { return BattleMaster.GetUnit(unitId)?.Name ?? ""; }
            catch (Exception) { return ""; }
        }
    }
}
