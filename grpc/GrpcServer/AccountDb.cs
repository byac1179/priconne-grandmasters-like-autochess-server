using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Byac.Services
{

    public static class AccountDb
    {
        private const string Schema = @"
CREATE TABLE IF NOT EXISTS accounts (
    viewer_id             INTEGER PRIMARY KEY,
    name                  TEXT    NOT NULL,
    token                 TEXT    NOT NULL,
    device_id             TEXT    NOT NULL,
    owner_address         TEXT    NOT NULL,
    avatar_id             INTEGER NOT NULL,
    board_id              INTEGER NOT NULL,
    battle_point          INTEGER NOT NULL,
    tutorial_step         INTEGER NOT NULL,
    created_at_utc        TEXT    NOT NULL,
    last_login_at_utc     TEXT    NOT NULL,
    completed_battle_count INTEGER NOT NULL DEFAULT 0,
    account               TEXT,
    password_hash         TEXT    NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS meta (
    key   TEXT PRIMARY KEY,
    value TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS bp_daily (
    date_key  INTEGER NOT NULL,
    viewer_id INTEGER NOT NULL,
    gain      INTEGER NOT NULL,
    PRIMARY KEY (date_key, viewer_id)
);

CREATE TABLE IF NOT EXISTS boss_state (
    viewer_id    INTEGER PRIMARY KEY,
    hp           INTEGER NOT NULL,
    next_deck_id INTEGER NOT NULL,
    decks_json   TEXT    NOT NULL,
    results_json TEXT    NOT NULL,
    updated_at_utc TEXT  NOT NULL
);";

        private const string BpResetMetaKey = "bp_reset_v1";

        public static int DateKeyOf(DateTime local) => local.Year * 10000 + local.Month * 100 + local.Day;

        public static int TodayDateKey() => DateKeyOf(DateTime.Now);

        private static SqliteConnection Open(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            var conn = new SqliteConnection($"Data Source={path}");

            try
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = Schema;
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "ALTER TABLE accounts ADD COLUMN completed_battle_count INTEGER NOT NULL DEFAULT 0";
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SqliteException ex) when (ex.SqliteErrorCode == 1)
                    {

                    }

                    AddColumn(cmd, "account", "TEXT");
                    AddColumn(cmd, "password_hash", "TEXT NOT NULL DEFAULT ''");

                    cmd.CommandText = "CREATE UNIQUE INDEX IF NOT EXISTS accounts_account_uidx " +
                                      "ON accounts (account) WHERE account IS NOT NULL";
                    cmd.ExecuteNonQuery();
                }
                return conn;
            }
            catch
            {
                conn.Dispose();
                throw;
            }
        }

        private static void AddColumn(SqliteCommand cmd, string name, string declaration)
        {
            cmd.CommandText = $"ALTER TABLE accounts ADD COLUMN {name} {declaration}";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 1)
            {

            }
        }

        public static void ReleaseFileHandles() => SqliteConnection.ClearAllPools();

        public enum BpResetState
        {

            Applied,

            AlreadyApplied,

            NoDatabase,

            Failed,
        }

        public static void MarkBpResetApplied(string path)
        {
            try
            {
                using var conn = Open(path);
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO meta (key, value) VALUES ($k, '1') " +
                                  "ON CONFLICT(key) DO UPDATE SET value = excluded.value";
                cmd.Parameters.AddWithValue("$k", BpResetMetaKey);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountDb] BP reset mark FAIL: {ex.Message}");
            }
        }

        public static BpResetState ResetBattlePointsOnce(string path)
        {
            if (!File.Exists(path)) return BpResetState.NoDatabase;
            try
            {

                using (var conn = Open(path))
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT value FROM meta WHERE key = $k";
                    cmd.Parameters.AddWithValue("$k", BpResetMetaKey);
                    if (cmd.ExecuteScalar() != null) return BpResetState.AlreadyApplied;
                }

                var bak = path + ".bak-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                File.Copy(path, bak);

                using (var conn = Open(path))
                using (var tx = conn.BeginTransaction())
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = tx;
                        cmd.CommandText = "UPDATE accounts SET battle_point = 0";
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = "INSERT INTO meta (key, value) VALUES ($k, '1') " +
                                          "ON CONFLICT(key) DO UPDATE SET value = excluded.value";
                        cmd.Parameters.AddWithValue("$k", BpResetMetaKey);
                        cmd.ExecuteNonQuery();
                    }
                    tx.Commit();
                }
                Console.WriteLine($"[AccountDb] BP reset migration applied (all accounts -> 0), backup: {bak}");
                return BpResetState.Applied;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountDb] BP reset migration FAIL (not reset; retry on next start): {ex.Message}");
                return BpResetState.Failed;
            }
        }

        public static void AddDailyBp(string path, int dateKey, int viewerId, int gain)
        {
            if (viewerId == 0 || gain <= 0) return;
            var cutoff = DateKeyOf(DateTime.Now.AddDays(-7));
            using var conn = Open(path);
            using var tx = conn.BeginTransaction();
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"INSERT INTO bp_daily (date_key, viewer_id, gain) VALUES ($d, $v, $g)
                                    ON CONFLICT(date_key, viewer_id) DO UPDATE SET gain = gain + excluded.gain";
                cmd.Parameters.AddWithValue("$d", dateKey);
                cmd.Parameters.AddWithValue("$v", viewerId);
                cmd.Parameters.AddWithValue("$g", gain);
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM bp_daily WHERE date_key < $c";
                cmd.Parameters.AddWithValue("$c", cutoff);
                cmd.ExecuteNonQuery();
            }
            tx.Commit();
        }

        public static List<(int DateKey, int ViewerId, int Gain)> LoadDailyBp(string path, int fromDateKey)
        {
            var rows = new List<(int, int, int)>();
            if (!File.Exists(path)) return rows;
            using var conn = Open(path);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT date_key, viewer_id, gain FROM bp_daily
                                WHERE date_key >= $from
                                ORDER BY date_key DESC, gain DESC, viewer_id ASC";
            cmd.Parameters.AddWithValue("$from", fromDateKey);
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
                rows.Add((rd.GetInt32(0), rd.GetInt32(1), rd.GetInt32(2)));
            return rows;
        }

        public static (int Hp, int NextDeckId, string DecksJson, string ResultsJson)? LoadBossState(string path, int viewerId)
        {
            if (viewerId == 0 || !File.Exists(path)) return null;
            using var conn = Open(path);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT hp, next_deck_id, decks_json, results_json FROM boss_state WHERE viewer_id = $v";
            cmd.Parameters.AddWithValue("$v", viewerId);
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return (rd.GetInt32(0), rd.GetInt32(1),
                    rd.IsDBNull(2) ? "[]" : rd.GetString(2),
                    rd.IsDBNull(3) ? "{}" : rd.GetString(3));
        }

        public static void UpsertBossState(string path, int viewerId, int hp, int nextDeckId, string decksJson, string resultsJson)
        {
            if (viewerId == 0) return;
            using var conn = Open(path);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO boss_state (viewer_id, hp, next_deck_id, decks_json, results_json, updated_at_utc)
                                VALUES ($v, $hp, $nd, $dj, $rj, $ts)
                                ON CONFLICT(viewer_id) DO UPDATE SET
                                    hp = excluded.hp,
                                    next_deck_id = excluded.next_deck_id,
                                    decks_json = excluded.decks_json,
                                    results_json = excluded.results_json,
                                    updated_at_utc = excluded.updated_at_utc";
            cmd.Parameters.AddWithValue("$v", viewerId);
            cmd.Parameters.AddWithValue("$hp", hp);
            cmd.Parameters.AddWithValue("$nd", nextDeckId);
            cmd.Parameters.AddWithValue("$dj", decksJson ?? "[]");
            cmd.Parameters.AddWithValue("$rj", resultsJson ?? "{}");
            cmd.Parameters.AddWithValue("$ts", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
            cmd.ExecuteNonQuery();
        }

        public static List<AccountRecord> LoadAll(string path)
        {
            var list = new List<AccountRecord>();
            if (!File.Exists(path)) return list;

            using var conn = Open(path);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT viewer_id, name, token, device_id, owner_address, avatar_id,
                                       board_id, battle_point, tutorial_step, created_at_utc, last_login_at_utc,
                                       completed_battle_count, account, password_hash
                                FROM accounts ORDER BY viewer_id";
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new AccountRecord
                {
                    ViewerId = rd.GetInt32(0),
                    Name = rd.GetString(1),
                    AuthenticationToken = Unseal(rd.GetString(2)),
                    DeviceIdentifier = FromSealedBase64(rd.GetString(3)),
                    OwnerAddress = rd.GetString(4),
                    AvatarId = rd.GetInt32(5),
                    BoardId = rd.GetInt32(6),
                    BattlePoint = rd.GetInt32(7),
                    TutorialStep = rd.GetInt32(8),
                    CreatedAtUtc = ParseUtc(rd.GetString(9)),
                    LastLoggedInAtUtc = ParseUtc(rd.GetString(10)),
                    CompletedBattleCount = rd.GetInt32(11),

                    Account = rd.IsDBNull(12) ? null : rd.GetString(12),
                    PasswordHash = rd.IsDBNull(13) ? "" : rd.GetString(13),
                });
            }
            return list;
        }

        public static void UpsertAll(string path, IReadOnlyList<AccountRecord> rows)
        {
            using var conn = Open(path);
            using var tx = conn.BeginTransaction();
            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"INSERT INTO accounts (viewer_id, name, token, device_id, owner_address,
                        avatar_id, board_id, battle_point, tutorial_step, created_at_utc, last_login_at_utc,
                        completed_battle_count, account, password_hash)
                    VALUES ($id,$name,$token,$dev,$owner,$avatar,$board,$bp,$tut,$created,$login,$cbc,$acct,$pwh)
                    ON CONFLICT(viewer_id) DO UPDATE SET
                        name=excluded.name, token=excluded.token, device_id=excluded.device_id,
                        owner_address=excluded.owner_address, avatar_id=excluded.avatar_id,
                        board_id=excluded.board_id, battle_point=excluded.battle_point,
                        tutorial_step=excluded.tutorial_step, created_at_utc=excluded.created_at_utc,
                        last_login_at_utc=excluded.last_login_at_utc,
                        completed_battle_count=excluded.completed_battle_count,
                        account=excluded.account, password_hash=excluded.password_hash";
                var pId = cmd.Parameters.Add("$id", SqliteType.Integer);
                var pName = cmd.Parameters.Add("$name", SqliteType.Text);
                var pToken = cmd.Parameters.Add("$token", SqliteType.Text);
                var pDev = cmd.Parameters.Add("$dev", SqliteType.Text);
                var pOwner = cmd.Parameters.Add("$owner", SqliteType.Text);
                var pAvatar = cmd.Parameters.Add("$avatar", SqliteType.Integer);
                var pBoard = cmd.Parameters.Add("$board", SqliteType.Integer);
                var pBp = cmd.Parameters.Add("$bp", SqliteType.Integer);
                var pTut = cmd.Parameters.Add("$tut", SqliteType.Integer);
                var pCreated = cmd.Parameters.Add("$created", SqliteType.Text);
                var pLogin = cmd.Parameters.Add("$login", SqliteType.Text);
                var pCbc = cmd.Parameters.Add("$cbc", SqliteType.Integer);
                var pAcct = cmd.Parameters.Add("$acct", SqliteType.Text);
                var pPwh = cmd.Parameters.Add("$pwh", SqliteType.Text);
                foreach (var a in rows)
                {
                    pId.Value = a.ViewerId;
                    pName.Value = a.Name ?? "";
                    pToken.Value = AccountDataCrypto.Seal(a.AuthenticationToken ?? "");
                    pDev.Value = AccountDataCrypto.Seal(Convert.ToBase64String(a.DeviceIdentifier ?? Array.Empty<byte>()));
                    pOwner.Value = a.OwnerAddress ?? "";
                    pAvatar.Value = a.AvatarId;
                    pBoard.Value = a.BoardId;
                    pBp.Value = a.BattlePoint;
                    pTut.Value = a.TutorialStep;
                    pCreated.Value = (a.CreatedAtUtc == default ? DateTime.UtcNow : a.CreatedAtUtc).ToString("O");
                    pLogin.Value = (a.LastLoggedInAtUtc == default ? DateTime.UtcNow : a.LastLoggedInAtUtc).ToString("O");
                    pCbc.Value = a.CompletedBattleCount;

                    pAcct.Value = (object)a.Account ?? DBNull.Value;
                    pPwh.Value = a.PasswordHash ?? "";
                    cmd.ExecuteNonQuery();
                }
            }
            tx.Commit();
        }

        private static string Unseal(string stored)
        {
            if (!AccountDataCrypto.TryUnseal(stored, out var plain, out var error))
            {

                Console.WriteLine($"[AccountDb] unseal FAIL: {error}");
                return "";
            }
            return plain;
        }

        private static byte[] FromSealedBase64(string stored)
        {
            var s = Unseal(stored);
            if (s.Length == 0) return Array.Empty<byte>();
            try { return Convert.FromBase64String(s); }
            catch (FormatException) { return Array.Empty<byte>(); }
        }

        private static DateTime ParseUtc(string text) =>
            DateTime.TryParseExact(text, "O", CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out var dt) ? dt.ToUniversalTime() : DateTime.MinValue;
    }
}
