using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Byac.ServerShared.MessagePackObjects;

namespace Byac.Services.Battle
{

    public sealed record AiMirrorRound(int Round, int Level, int Gold, List<AiMirrorUnit> Units);

    public sealed record AiMirrorUnit(int UnitId, int Rank, int X, int Y, List<BattleEquipmentDataMpo> Equipment);

    public static class AiMirrorStore
    {
        private static readonly object Gate = new object();
        private static SqliteConnection? conn_;
        private static DateTime nextPruneUtc_ = DateTime.MinValue;

        private const string Schema = @"
CREATE TABLE IF NOT EXISTS mirror_round (
    battle_id  TEXT    NOT NULL,
    round      INTEGER NOT NULL,
    viewer_id  INTEGER NOT NULL,
    level      INTEGER NOT NULL,
    gold       INTEGER NOT NULL,
    units      TEXT    NOT NULL,
    PRIMARY KEY (battle_id, round, viewer_id)
);
CREATE INDEX IF NOT EXISTS idx_mirror_battle ON mirror_round(battle_id);";

        private static bool On => Environment.GetEnvironmentVariable("BYAC_AI_MIRROR") != "0";

        private static string DataDir
        {
            get
            {
                var env = Environment.GetEnvironmentVariable("BYAC_DATA_DIR");
                return string.IsNullOrEmpty(env) ? Path.Combine(Environment.CurrentDirectory, "data") : env;
            }
        }

        private static string DbPath => Path.Combine(DataDir, "aimirror.db");

        private static int KeepBattles
        {
            get
            {
                var env = Environment.GetEnvironmentVariable("BYAC_AI_MIRROR_KEEP_BATTLES");
                return int.TryParse(env, out var n) && n >= 0 ? n : 200;
            }
        }

        private static int PruneSeconds
        {
            get
            {
                var env = Environment.GetEnvironmentVariable("BYAC_AI_MIRROR_PRUNE_SEC");
                return int.TryParse(env, out var n) && n >= 0 ? n : 600;
            }
        }

        private static readonly JsonSerializerOptions JsonOpts = new();

        private static SqliteConnection Conn()
        {
            if (conn_ != null) return conn_;
            var dir = Path.GetDirectoryName(DbPath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            var conn = new SqliteConnection($"Data Source={DbPath}");
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = Schema;
                cmd.ExecuteNonQuery();
            }
            return conn_ = conn;
        }

        private static void DropConn()
        {
            lock (Gate)
            {
                try { conn_?.Dispose(); } catch { }
                conn_ = null;
            }
        }

        public static void RecordRound(string battleId, int round, IReadOnlyList<BattlePlayerDataMpo> humans)
        {
            if (!On || humans == null || humans.Count == 0 || string.IsNullOrEmpty(battleId) || round < 1) return;
            try
            {
                lock (Gate)
                {
                    var conn = Conn();
                    using var tx = conn.BeginTransaction();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = @"INSERT OR REPLACE INTO mirror_round
                            (battle_id, round, viewer_id, level, gold, units)
                            VALUES ($b,$r,$v,$lv,$g,$u)";
                        var pB = cmd.Parameters.Add("$b", SqliteType.Text);
                        var pR = cmd.Parameters.Add("$r", SqliteType.Integer);
                        var pV = cmd.Parameters.Add("$v", SqliteType.Integer);
                        var pLv = cmd.Parameters.Add("$lv", SqliteType.Integer);
                        var pG = cmd.Parameters.Add("$g", SqliteType.Integer);
                        var pU = cmd.Parameters.Add("$u", SqliteType.Text);
                        var written = 0;
                        foreach (var p in humans)
                        {
                            var units = (p.FieldUnitList ?? new List<BattleUnitDataMpo>())
                                .Select(u => new UnitDto
                                {
                                    U = u.UnitId,
                                    R = u.Rank,
                                    X = u.PositionX,
                                    Y = u.PositionY,
                                    Eq = u.EquipmentList ?? new List<BattleEquipmentDataMpo>(),
                                }).ToList();
                            pB.Value = battleId;
                            pR.Value = round;
                            pV.Value = p.ViewerId;
                            pLv.Value = p.Level;
                            pG.Value = p.Gold;
                            pU.Value = JsonSerializer.Serialize(units, JsonOpts);
                            written += cmd.ExecuteNonQuery();
                        }
                        PruneThrottled_NoLock(conn, tx);
                        tx.Commit();
                        Console.WriteLine($"[AI-MIRROR] record battle={battleId[..Math.Min(8, battleId.Length)]} round={round} rows={written}");
                    }
                }
            }
            catch (Exception ex)
            {
                DropConn();
                Console.WriteLine($"[AI-MIRROR] record FAIL battle={battleId} round={round}: {ex.GetType().Name}: {ex.Message}");
            }
        }

        public static List<(string BattleId, int ViewerId)> PickDonors(int count)
        {
            var result = new List<(string, int)>();
            if (!On || count <= 0) return result;
            try
            {
                lock (Gate)
                {
                    var conn = Conn();
                    using var cmd = conn.CreateCommand();

                    cmd.CommandText = @"SELECT battle_id, viewer_id FROM mirror_round
                        GROUP BY battle_id, viewer_id
                        HAVING COUNT(DISTINCT round) > 1
                        ORDER BY RANDOM() LIMIT $n";
                    cmd.Parameters.Add("$n", SqliteType.Integer).Value = count;
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                        result.Add((reader.GetString(0), reader.GetInt64(1) > int.MaxValue ? int.MaxValue : (int)reader.GetInt64(1)));
                }
            }
            catch (Exception ex)
            {
                DropConn();
                Console.WriteLine($"[AI-MIRROR] pick FAIL: {ex.GetType().Name}: {ex.Message}");
            }
            return result;
        }

        public static List<AiMirrorRound> LoadTimeline(string battleId, int viewerId)
        {
            var timeline = new List<AiMirrorRound>();
            if (!On || string.IsNullOrEmpty(battleId)) return timeline;
            try
            {
                lock (Gate)
                {
                    var conn = Conn();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = @"SELECT round, level, gold, units FROM mirror_round
                        WHERE battle_id=$b AND viewer_id=$v ORDER BY round";
                    cmd.Parameters.Add("$b", SqliteType.Text).Value = battleId;
                    cmd.Parameters.Add("$v", SqliteType.Integer).Value = viewerId;
                    using var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var units = JsonSerializer.Deserialize<List<UnitDto>>(reader.GetString(3), JsonOpts) ?? new List<UnitDto>();
                        timeline.Add(new AiMirrorRound(
                            Convert.ToInt32(reader.GetInt64(0), CultureInfo.InvariantCulture),
                            Convert.ToInt32(reader.GetInt64(1), CultureInfo.InvariantCulture),
                            Convert.ToInt32(reader.GetInt64(2), CultureInfo.InvariantCulture),
                            units.Select(d => new AiMirrorUnit(d.U, d.R, d.X, d.Y, d.Eq ?? new List<BattleEquipmentDataMpo>())).ToList()));
                    }
                }
            }
            catch (Exception ex)
            {
                DropConn();
                Console.WriteLine($"[AI-MIRROR] load FAIL battle={battleId} viewer={viewerId}: {ex.GetType().Name}: {ex.Message}");
            }
            return timeline;
        }

        private static void PruneThrottled_NoLock(SqliteConnection conn, SqliteTransaction tx)
        {
            var now = DateTime.UtcNow;
            if (now < nextPruneUtc_) return;
            nextPruneUtc_ = now.AddSeconds(PruneSeconds);
            Prune_NoLock(conn, tx);
        }

        private static void Prune_NoLock(SqliteConnection conn, SqliteTransaction tx)
        {
            var keep = KeepBattles;
            if (keep <= 0) return;
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"DELETE FROM mirror_round WHERE battle_id IN (
                SELECT battle_id FROM (
                    SELECT battle_id, MAX(rowid) AS last FROM mirror_round
                    GROUP BY battle_id ORDER BY last DESC LIMIT -1 OFFSET $k))";
            cmd.Parameters.Add("$k", SqliteType.Integer).Value = keep;
            cmd.ExecuteNonQuery();
        }

        private sealed class UnitDto
        {
            public int U { get; set; }
            public int R { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public List<BattleEquipmentDataMpo>? Eq { get; set; }
        }
    }
}
