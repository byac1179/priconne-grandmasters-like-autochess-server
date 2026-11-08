using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Byac.ServerShared;
using Byac.ServerShared.Enums;
using Byac.Services.Battle;

namespace Byac.Services
{

    public class AccountRecord
    {
        public int ViewerId { get; set; }
        public string Name { get; set; } = "";
        public string AuthenticationToken { get; set; } = "";
        public byte[] DeviceIdentifier { get; set; } = Array.Empty<byte>();

        public string? Account { get; set; }

        public string PasswordHash { get; set; } = "";

        public string OwnerAddress { get; set; } = "";
        public int AvatarId { get; set; }
        public int BoardId { get; set; }
        public int BattlePoint { get; set; }

        public int CompletedBattleCount { get; set; }

        public int TutorialStep { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime LastLoggedInAtUtc { get; set; }
    }

    public static class AccountStore
    {

        private const int ViewerIdBase = 140_000_000;
        private const int ViewerIdSpan = 100_000_000;
        private const int FirstAllocatableViewerId = ViewerIdBase;
        private const string LoopbackPeer = "127.0.0.1";

        private const string OwnerHashPrefix = "h1:";
        private static readonly byte[] OwnerHashKey = ResolveOwnerHashKey();

        private static byte[] ResolveOwnerHashKey()
        {
            var env = Environment.GetEnvironmentVariable("BYAC_OWNER_HASH_KEY");
            if (!string.IsNullOrWhiteSpace(env))
            {
                var fromEnv = Convert.FromBase64String(env.Trim());
                if (fromEnv.Length != 32)
                    throw new InvalidOperationException($"BYAC_OWNER_HASH_KEY must be base64 of 32 bytes (got {fromEnv.Length})");
                return fromEnv;
            }

            var keyPath = Path.Combine(DataDir, "owner.key");
            if (File.Exists(keyPath))
            {
                var stored = Convert.FromBase64String(File.ReadAllText(keyPath).Trim());
                if (stored.Length != 32)
                    throw new InvalidOperationException($"invalid key file {keyPath}: expected base64 of 32 bytes");
                return stored;
            }

            var generated = new byte[32];
            RandomNumberGenerator.Fill(generated);
            Directory.CreateDirectory(DataDir);
            File.WriteAllText(keyPath, Convert.ToBase64String(generated));
            Console.WriteLine($"[AccountStore] generated owner-hash key file {keyPath}; set BYAC_OWNER_HASH_KEY to supply an explicit key");
            return generated;
        }

        private static string HashOwner(string? peer)
        {
            var mac = HMACSHA256.HashData(OwnerHashKey,
                System.Text.Encoding.UTF8.GetBytes(peer ?? ""));
            return OwnerHashPrefix + Convert.ToHexString(mac.AsSpan(0, 16)).ToLowerInvariant();
        }

        private static bool IsHashedOwner(string? owner) =>
            owner != null && owner.StartsWith(OwnerHashPrefix, StringComparison.Ordinal);

        private static string OwnerForLog(string? owner) =>
            IsHashedOwner(owner) ? "h1:…"
            : string.IsNullOrEmpty(owner) ? LoopbackPeer
            : LogSafe.Address(owner);

        public static string PeerAddressOf(string? grpcPeer)
        {
            if (string.IsNullOrEmpty(grpcPeer)) return LoopbackPeer;
            var s = grpcPeer!;
            var colon = s.IndexOf(':');
            if (colon > 0) s = s.Substring(colon + 1);
            var last = s.LastIndexOf(':');
            if (last > 0) s = s.Substring(0, last);
            return s.Length == 0 ? LoopbackPeer : s;
        }

        private static readonly object Gate = new object();

        public static Func<Func<AccountRecord>, Task<AccountRecord>>? OpRunnerCreate;

        private static List<AccountRecord>? _accounts;
        private static int _nextViewerId = FirstAllocatableViewerId;

        public static AccountRecord[] SnapshotAll()
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                return _accounts!.ToArray();
            }
        }

        private static string DataDir
        {
            get
            {
                var env = Environment.GetEnvironmentVariable("BYAC_DATA_DIR");
                return string.IsNullOrEmpty(env) ? Path.Combine(Environment.CurrentDirectory, "data") : env;
            }
        }

        private static string DataPath => Path.Combine(DataDir, "accounts.json");

        private static string DbPath => Path.Combine(DataDir, "accounts.db");

        public static int ViewerIdFromHeaders(Grpc.Core.Metadata? headers)
        {
            int headerViewerId = 0;
            if (headers != null)
            {
                string auth = null;
                foreach (var h in headers)
                {
                    if (h.Key == "x-app-viewer-id")
                    {
                        if (int.TryParse(h.Value, out var vid) && vid > 0) headerViewerId = vid;
                    }
                    else if (auth == null && h.Key.Equals("authorization", StringComparison.OrdinalIgnoreCase))
                    {
                        auth = h.Value;
                    }
                }
                var token = BearerOf(auth);
                if (token != null)
                {
                    lock (Gate)
                    {
                        EnsureLoaded_NoLock();
                        foreach (var a in _accounts!)
                        {

                            if (a.AuthenticationToken == token)
                            {
                                ReportIdentityDivergence(a.ViewerId, headerViewerId);
                                return a.ViewerId;
                            }
                        }
                    }
                }
            }
            return headerViewerId > 0 ? headerViewerId : UserProfile.ViewerId;
        }

        private static string BearerOf(string auth)
        {
            if (string.IsNullOrEmpty(auth)) return null;
            var parts = auth.Split(new[] { ' ' }, 2);
            if (parts.Length != 2 || !parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase)) return null;
            return string.IsNullOrEmpty(parts[1]) ? null : parts[1];
        }

        private static int _divergenceReported;

        private static void ReportIdentityDivergence(int tokenOwner, int claimedByHeader)
        {
            if (claimedByHeader <= 0 || claimedByHeader == tokenOwner) return;
            if (Interlocked.Exchange(ref _divergenceReported, 1) != 0) return;
            Console.WriteLine($"[ACC-ID] client header says {claimedByHeader} but the Bearer token " +
                $"belongs to {tokenOwner}; token wins (client does not adopt a reassigned ViewerId)");
        }

        public static AccountRecord GetOrDefault(int viewerId)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var rec = Find_NoLock(viewerId);
                return rec ?? Find_NoLock(UserProfile.ViewerId)!;
            }
        }

        public static bool TryAuthenticate(int viewerId, string authenticationToken, out AccountRecord record)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var rec = Find_NoLock(viewerId);
                if (rec != null && rec.AuthenticationToken == authenticationToken)
                {
                    record = rec;
                    return true;
                }
            }
            record = null!;
            return false;
        }

        public static string NormalizeAccount(string raw)
        {
            var s = raw?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(s) || s.Length < 3 || s.Length > 24) return null;
            foreach (var c in s)
            {
                var ok = (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                         c == '.' || c == '_' || c == '-';
                if (!ok) return null;
            }
            return s;
        }

        public static bool PasswordShapeOk(string password) =>
            !string.IsNullOrEmpty(password) && password.Length >= 6 && password.Length <= 128;

        public static LoginResult LoginOrCreate(string rawAccount, string password,
                                                string peer, bool viaProxy, out AccountRecord record)
        {
            record = null!;
            var account = NormalizeAccount(rawAccount);
            if (account == null) return LoginResult.BadAccount;
            if (!PasswordShapeOk(password)) return LoginResult.BadPassword;

            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var hit = _accounts!.FirstOrDefault(a => a.Account == account);
                if (hit != null)
                {
                    if (hit.PasswordHash.Length == 0)
                    {

                        Console.WriteLine($"[Auth] viewer={hit.ViewerId} account={account} has no password hash -> rejected");
                        return LoginResult.WrongPassword;
                    }
                    if (!PasswordHasher.Verify(password, hit.PasswordHash)) return LoginResult.WrongPassword;
                    hit.LastLoggedInAtUtc = DateTime.UtcNow;
                    Save_NoLock();
                    record = hit;
                    Console.WriteLine($"[Auth] LOGIN ok viewer={hit.ViewerId} account={account} " +
                        $"peer={LogSafe.Address(peer)}");
                    return LoginResult.Ok;
                }

                var rec = CreateCore("", null, peer, viaProxy);
                rec.Account = account;
                rec.PasswordHash = PasswordHasher.Hash(password);
                Save_NoLock();
                record = rec;
                Console.WriteLine($"[Auth] SIGNUP ok viewer={rec.ViewerId} account={account} " +
                    $"peer={LogSafe.Address(peer)}");
                return LoginResult.Created;
            }
        }

        public static string BindAccount(int viewerId, string rawAccount, string password)
        {
            var account = NormalizeAccount(rawAccount);
            if (account == null) return "bad_account";
            if (!PasswordShapeOk(password)) return "bad_password";
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var rec = Find_NoLock(viewerId);
                if (rec == null) return "no_such_viewer";
                var taken = _accounts!.FirstOrDefault(a => a.Account == account && a.ViewerId != viewerId);
                if (taken != null) return "account_taken";
                rec.Account = account;
                rec.PasswordHash = PasswordHasher.Hash(password);
                Save_NoLock();
                Console.WriteLine($"[Auth] BIND viewer={viewerId} account={account}");
                return null;
            }
        }

        public enum LoginResult
        {

            Ok,

            Created,

            BadAccount,

            BadPassword,

            WrongPassword,
        }

        public static AccountRecord AcceptReturningClient(int viewerId, string authenticationToken, string peer = LoopbackPeer)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();

                if (!string.IsNullOrEmpty(authenticationToken))
                {
                    var byToken = _accounts!.FirstOrDefault(a => a.AuthenticationToken == authenticationToken);
                    if (byToken != null)
                    {
                        if (byToken.ViewerId != viewerId)
                            Console.WriteLine($"[AccountStore] viewer={viewerId} credentials actually owned by {byToken.ViewerId} -> " +
                                $"adopting {byToken.ViewerId} as server truth (C2b token wins)");
                        Adopt(byToken, peer);
                        Save_NoLock();
                        return byToken;
                    }
                }

                var rec = Find_NoLock(viewerId);
                if (rec == null)
                {
                    rec = new AccountRecord
                    {
                        ViewerId = viewerId,
                        Name = NewDefaultName(),
                        AuthenticationToken = authenticationToken,
                        DeviceIdentifier = NewDevice(),
                        OwnerAddress = HashOwner(peer),
                        AvatarId = UserProfile.AvatarId,
                        BoardId = UserProfile.BoardId,
                        BattlePoint = 0,
                        TutorialStep = (int)TutorialStep.END,
                        CreatedAtUtc = DateTime.UtcNow,
                        LastLoggedInAtUtc = DateTime.UtcNow,
                    };
                    _accounts!.Add(rec);
                    if (_nextViewerId <= viewerId) _nextViewerId = viewerId + 1;
                    Console.WriteLine($"[AccountStore] accepted unknown viewerId={viewerId} (client-token re-seed) owner={LogSafe.Address(peer)}");
                }
                else if (string.IsNullOrEmpty(authenticationToken) || rec.AuthenticationToken == authenticationToken)
                {
                    rec.AuthenticationToken = authenticationToken;
                    Console.WriteLine($"[AccountStore] refreshed token viewerId={viewerId}");
                }
                else
                {

                    var adopted = new AccountRecord
                    {
                        AuthenticationToken = authenticationToken,
                        DeviceIdentifier = NewDevice(),
                        OwnerAddress = HashOwner(peer),
                        AvatarId = UserProfile.AvatarId,
                        BoardId = UserProfile.BoardId,
                        BattlePoint = rec.BattlePoint,
                        TutorialStep = (int)TutorialStep.END,
                        CreatedAtUtc = DateTime.UtcNow,
                        LastLoggedInAtUtc = DateTime.UtcNow,
                    };
                    adopted.ViewerId = AllocateViewerId(authenticationToken);
                    adopted.Name = NewDefaultName();
                    _accounts!.Add(adopted);
                    Console.WriteLine($"[AccountStore] !! viewerId collision {viewerId} already owned by source {OwnerForLog(rec.OwnerAddress)}," +
                        $"caller={LogSafe.Address(peer)} -> new account {adopted.ViewerId} (original untouched)");
                    rec = adopted;
                }
                Save_NoLock();
                return rec;
            }
        }

        private static string NewDefaultName()
        {
            for (var attempt = 0; attempt < 8; attempt++)
            {
                var n = "Player" + Random.Shared.Next(1000, 10000);
                if (_accounts!.All(a => a.Name != n)) return n;
            }
            return "Player" + Random.Shared.Next(1000, 10000) + "-" + Random.Shared.Next(100, 1000);
        }

        public static AccountRecord Create(string userName, int? previousViewerId, string peer = LoopbackPeer,
                                           bool viaProxy = false)
        {
            if (OpRunnerCreate != null)
                return CreateAsync(userName, previousViewerId, peer, viaProxy).GetAwaiter().GetResult();
            return CreateCore(userName, previousViewerId, peer, viaProxy);
        }

        public static async Task<AccountRecord> CreateAsync(string userName, int? previousViewerId,
                                                            string peer = LoopbackPeer, bool viaProxy = false)
        {
            if (OpRunnerCreate != null)
                return await OpRunnerCreate(() => CreateCore(userName, previousViewerId, peer, viaProxy)).ConfigureAwait(false);
            return CreateCore(userName, previousViewerId, peer, viaProxy);
        }

        private static AccountRecord CreateCore(string userName, int? previousViewerId, string peer, bool viaProxy)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();

                if (previousViewerId.HasValue)
                {
                    var byPrev = Find_NoLock(previousViewerId.Value);
                    if (byPrev != null && SameOrigin(byPrev, peer, viaProxy))
                    {
                        if (viaProxy && !OwnerMatches(byPrev, peer))
                            Console.WriteLine($"[AccountStore] create prev={previousViewerId} source record is " +
                                $"{OwnerForLog(byPrev.OwnerAddress)}, request peer={LogSafe.Address(peer)} mismatch," +
                                "but request came via proxy -> still claim (proxy IP is not device identity, see SameOrigin)");
                        Console.WriteLine($"[AccountStore] create hit existing viewerId={byPrev.ViewerId} (prev={previousViewerId})");
                        Adopt(byPrev, peer);
                        Save_NoLock();
                        return byPrev;
                    }
                    if (byPrev != null)
                        Console.WriteLine($"[AccountStore] create prev={previousViewerId} belongs to source {OwnerForLog(byPrev.OwnerAddress)}," +
                            $"request source {LogSafe.Address(peer)} -> no claim, create new account (C2b collision guard)");
                }

                var name = userName?.Trim();
                if (!string.IsNullOrEmpty(name))
                {
                    var byName = _accounts!.FirstOrDefault(a => a.Name == name);
                    if (byName != null && SameOrigin(byName, peer, viaProxy))
                    {
                        Console.WriteLine($"[AccountStore] create hit existing name={name} viewerId={byName.ViewerId}");
                        Adopt(byName, peer);
                        Save_NoLock();
                        return byName;
                    }
                    if (byName != null)
                        Console.WriteLine($"[AccountStore] duplicate name={name} already held by source {OwnerForLog(byName.OwnerAddress)} account " +
                            $"{byName.ViewerId}, request source {LogSafe.Address(peer)} -> separate accounts (C2b duplicate-name handling)");
                }

                var rec = new AccountRecord
                {
                    AuthenticationToken = NewToken(),
                    DeviceIdentifier = NewDevice(),
                    OwnerAddress = HashOwner(peer),
                    AvatarId = UserProfile.AvatarId,
                    BoardId = UserProfile.BoardId,
                    BattlePoint = 0,

                    TutorialStep = (int)TutorialStep.END,
                    CreatedAtUtc = DateTime.UtcNow,
                    LastLoggedInAtUtc = DateTime.UtcNow,
                };
                rec.ViewerId = AllocateViewerId(rec.AuthenticationToken);

                rec.Name = NewDefaultName();
                _accounts!.Add(rec);
                Save_NoLock();
                Console.WriteLine($"[AccountStore] created account viewerId={rec.ViewerId} name={rec.Name} owner={OwnerForLog(rec.OwnerAddress)} (persisted {DbPath} via AccountDb; player data has been DB-only since 0.2.12)");
                return rec;
            }
        }

        private static bool SameOrigin(AccountRecord rec, string peer, bool viaProxy)
            => viaProxy || OwnerMatches(rec, peer);

        private static bool OwnerMatches(AccountRecord rec, string peer)
        {
            var stored = rec.OwnerAddress;
            if (string.IsNullOrEmpty(stored)) return peer == LoopbackPeer;
            if (IsHashedOwner(stored)) return stored == HashOwner(peer);
            return stored == peer;
        }

        private static void Adopt(AccountRecord rec, string peer)
        {
            if (string.IsNullOrEmpty(rec.OwnerAddress))
            {
                rec.OwnerAddress = HashOwner(peer);
                Save_NoLock();
            }
        }

        private static int AllocateViewerId(string token)
        {
            var h = Fnv1a(token);
            for (var tries = 0; tries < 32; tries++)
            {
                var id = ViewerIdBase + (int)(h % ViewerIdSpan);
                if (Find_NoLock(id) == null) return id;
                h = h * 31u + 7u;
            }
            return _nextViewerId++;
        }

        private static uint Fnv1a(string s)
        {
            var h = 2166136261u;
            foreach (var c in s)
            {
                h ^= c;
                h *= 16777619u;
            }
            return h;
        }

        public static bool TryEditName(int viewerId, string newName, out string normalized)
        {
            normalized = newName?.Trim() ?? "";
            if (normalized.Length == 0)
            {
                Console.WriteLine($"[AccountStore] edit name REJECT (blank): {newName ?? "null"}");
                return false;
            }
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var rec = GetOrDefault_NoLock(viewerId);
                rec.Name = normalized;
                Save_NoLock();
            }
            Console.WriteLine($"[AccountStore] viewer={viewerId} name -> {normalized}");
            return true;
        }

        public static bool TryEditAvatar(int viewerId, int avatarId)
        {
            var legal = BattleMaster.AvatarIds;
            if (Array.IndexOf(legal, avatarId) < 0)
            {
                Console.WriteLine($"[AccountStore] edit avatar REJECT {avatarId} (not in master avatar_data)");
                return false;
            }
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var rec = GetOrDefault_NoLock(viewerId);
                rec.AvatarId = avatarId;
                Save_NoLock();
            }
            Console.WriteLine($"[AccountStore] viewer={viewerId} avatarId -> {avatarId}");
            return true;
        }

        public static bool TryEditBoard(int viewerId, int boardId)
        {
            if (boardId < 1 || boardId > 5)
            {
                Console.WriteLine($"[AccountStore] edit board REJECT {boardId} (master board_data BoardId 1..5)");
                return false;
            }
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var rec = GetOrDefault_NoLock(viewerId);
                rec.BoardId = boardId;
                Save_NoLock();
            }
            Console.WriteLine($"[AccountStore] viewer={viewerId} boardId -> {boardId}");
            return true;
        }

        public static int AddBattlePoint(int viewerId, int delta)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var rec = GetOrDefault_NoLock(viewerId);
                rec.BattlePoint += delta;
                Save_NoLock();
                if (delta > 0)
                {
                    try
                    {
                        AccountDb.AddDailyBp(DbPath, AccountDb.TodayDateKey(), viewerId, delta);
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine($"[AccountStore] bp_daily write FAIL viewer={viewerId}: {ex.Message}");
                    }
                }
                Console.WriteLine($"[AccountStore] viewer={viewerId} BP {rec.BattlePoint - delta} -> {rec.BattlePoint} ({(delta >= 0 ? "+" : "")}{delta})");
                return rec.BattlePoint;
            }
        }

        public static readonly int[] BpByRank = { 50, 40, 35, 30, 25, 20, 10, 5 };

        public static int BpGain(int currentBp, int ranking)
        {
            var idx = ranking >= 1 && ranking <= BpByRank.Length ? ranking - 1 : BpByRank.Length - 1;
            var baseScore = BpByRank[idx];
            var cur = Math.Max(0, currentBp);
            return Math.Max(1, (int)Math.Round(baseScore * 1000.0 / (1000.0 + cur)));
        }

        public static AccountRecord[] TopByBattlePoint(int limit)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                return _accounts!
                    .OrderByDescending(a => a.BattlePoint)
                    .ThenBy(a => a.ViewerId)
                    .Take(limit)
                    .ToArray();
            }
        }

        public static int BattlePointRankOf(int viewerId)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var i = _accounts!
                    .OrderByDescending(a => a.BattlePoint)
                    .ThenBy(a => a.ViewerId)
                    .ToList()
                    .FindIndex(a => a.ViewerId == viewerId);
                return i < 0 ? 0 : i + 1;
            }
        }

        public static void AddCompletedBattles(IEnumerable<int> viewerIds)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var n = 0;
                foreach (var vid in viewerIds.Distinct())
                {
                    var rec = Find_NoLock(vid);
                    if (rec == null) continue;
                    rec.CompletedBattleCount++;
                    n++;
                }
                if (n > 0) Save_NoLock();
            }
        }

        public static List<(int DateKey, int ViewerId, int Gain)> LoadDailyBp(int fromDateKey)
        {
            try
            {
                return AccountDb.LoadDailyBp(DbPath, fromDateKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountStore] bp_daily read FAIL: {ex.Message}");
                return new List<(int, int, int)>();
            }
        }

        internal static (int Hp, int NextDeckId, string DecksJson, string ResultsJson)? LoadBossState(int viewerId)
        {
            lock (Gate)
            {
                try
                {
                    return AccountDb.LoadBossState(DbPath, viewerId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AccountStore] boss_state read FAIL viewer={viewerId}: {ex.Message}");
                    return null;
                }
            }
        }

        internal static void SaveBossState(int viewerId, int hp, int nextDeckId, string decksJson, string resultsJson)
        {
            lock (Gate)
            {
                try
                {
                    AccountDb.UpsertBossState(DbPath, viewerId, hp, nextDeckId, decksJson, resultsJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AccountStore] boss_state write FAIL viewer={viewerId}: {ex.Message}");
                }
            }
        }

        public static DateTime TouchLogin(int viewerId)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var rec = GetOrDefault_NoLock(viewerId);
                rec.LastLoggedInAtUtc = DateTime.Now;
                Save_NoLock();
                return rec.LastLoggedInAtUtc;
            }
        }

        public static int TutorialStepOf(int viewerId)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                return GetOrDefault_NoLock(viewerId).TutorialStep;
            }
        }

        public static bool TryAdvanceTutorialStep(int viewerId, out int newStep)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                var rec = GetOrDefault_NoLock(viewerId);
                var current = rec.TutorialStep;
                var inRange = (current >= 0 && current <= 15) || current == 100;
                if (!inRange)
                {
                    newStep = current;
                    Console.WriteLine($"[AccountStore] tutorial advance REJECT viewer={viewerId} current={current} (not in 0..15/100)");
                    return false;
                }
                newStep = current < 15 ? current + 1 : 100;
                if (newStep != current)
                {
                    rec.TutorialStep = newStep;
                    Save_NoLock();
                    Console.WriteLine($"[AccountStore] viewer={viewerId} TutorialStep {current} -> {newStep}");
                }
                return true;
            }
        }

        public static int BattlePointOf(int viewerId)
        {
            lock (Gate)
            {
                EnsureLoaded_NoLock();
                return GetOrDefault_NoLock(viewerId).BattlePoint;
            }
        }

        private static AccountRecord? Find_NoLock(int viewerId)
        {
            return _accounts!.FirstOrDefault(a => a.ViewerId == viewerId);
        }

        private static AccountRecord GetOrDefault_NoLock(int viewerId)
        {
            return Find_NoLock(viewerId) ?? Find_NoLock(UserProfile.ViewerId)!;
        }

        private static void EnsureLoaded_NoLock()
        {
            if (_accounts != null) return;

            _accounts = LoadAccounts();

            var bpReset = AccountDb.ResetBattlePointsOnce(DbPath);
            if (bpReset == AccountDb.BpResetState.Applied || bpReset == AccountDb.BpResetState.NoDatabase)
            {
                var nonzero = _accounts.Count(a => a.BattlePoint != 0);
                if (nonzero > 0)
                {
                    foreach (var a in _accounts) a.BattlePoint = 0;
                    Console.WriteLine($"[AccountStore] BP reset migration: {nonzero} accounts -> 0 ({bpReset})");
                }
            }
            Console.WriteLine($"[AccountStore] BP reset guard: {bpReset}");

            if (Find_NoLock(UserProfile.ViewerId) == null)
            {
                _accounts.Add(new AccountRecord
                {
                    ViewerId = UserProfile.ViewerId,
                    Name = UserProfile.Name,
                    AuthenticationToken = "VWl2dCtLWEJucXgyaFhhc2Rhc2Rhc2RhZGFzZGFzZGFqa2lvM200OTIzODRtMjM5cG12MjNpb3BybTIzb3BbNHYyNDIzNGxXTzQ2L3BDdFF4UT09",
                    DeviceIdentifier = new byte[] { 0x01, 0x7f, 0xea, 0x11, 0x31, 0x44, 0x55, 0x10, 0x3a, 0x77, 0xb1, 0x07, 0x28, 0x17, 0x45, 0x40 },
                    AvatarId = UserProfile.AvatarId,
                    BoardId = UserProfile.BoardId,
                    BattlePoint = 0,

                    TutorialStep = 2,
                    CreatedAtUtc = DateTime.UtcNow,
                    LastLoggedInAtUtc = DateTime.UtcNow,
                });
            }

            var ownerMigrated = 0;
            foreach (var rec in _accounts!)
            {
                if (!string.IsNullOrEmpty(rec.OwnerAddress) && !IsHashedOwner(rec.OwnerAddress))
                {
                    rec.OwnerAddress = HashOwner(rec.OwnerAddress);
                    ownerMigrated++;
                }
            }
            if (ownerMigrated > 0)
                Console.WriteLine($"[AccountStore] migrated {ownerMigrated} OwnerAddress -> HMAC");

            var maxUsed = _accounts.Max(a => a.ViewerId);
            var floor = Math.Max(FirstAllocatableViewerId, maxUsed + 1);
            if (_nextViewerId < floor) _nextViewerId = floor;

            Save_NoLock();

            if (bpReset == AccountDb.BpResetState.NoDatabase) AccountDb.MarkBpResetApplied(DbPath);
        }

        private static List<AccountRecord> LoadAccounts()
        {
            string? unreadableBackup = null;
            DateTime? brokenDbWrite = null;
            try
            {
                var rows = AccountDb.LoadAll(DbPath);
                if (rows.Count > 0)
                {
                    Console.WriteLine($"[AccountStore] loaded {rows.Count} accounts from {DbPath}");
                    return rows;
                }
            }
            catch (Exception ex)
            {
                brokenDbWrite = TryLastWriteUtc(DbPath);
                var badPath = DbPath + ".bad-" + DateTime.Now.ToString("yyyyMMddHHmmss");
                try
                {
                    AccountDb.ReleaseFileHandles();
                    File.Move(DbPath, badPath);
                    unreadableBackup = badPath;
                    Console.WriteLine($"[AccountStore] DB UNREADABLE {DbPath}: {ex.Message} -> backed up to {badPath}");
                }
                catch (Exception mv)
                {
                    Console.WriteLine($"[AccountStore] DB UNREADABLE {DbPath}: {ex.Message}; backup also failed ({mv.Message})," +
                        "starting with an empty DB, but do NOT restart before verifying");
                }
            }

            var legacy = ReadLegacyJson();
            if (legacy != null && legacy.Count > 0)
            {

                if (brokenDbWrite.HasValue && JsonIsOlderThan(brokenDbWrite.Value))
                {
                    Console.WriteLine($"[AccountStore] !! DB unreadable ({(unreadableBackup != null ? "backed up to " + unreadableBackup : "backup failed; original file still in place")})," +
                        $"and {DataPath} is older (leftover snapshot from before SQLite) -> not imported, starting empty. Restore accounts manually from a backup if needed.");
                    return new List<AccountRecord>();
                }
                Console.WriteLine($"[AccountStore] imported {legacy.Count} accounts from {DataPath} -> {DbPath}");
                return legacy;
            }
            Console.WriteLine($"[AccountStore] no {DbPath}; seeding default account {UserProfile.ViewerId}");
            return new List<AccountRecord>();
        }

        private static DateTime? TryLastWriteUtc(string path)
        {
            try { return File.GetLastWriteTimeUtc(path); }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountStore] failed to read timestamp of {Path.GetFileName(path)}: {ex.Message}");
                return null;
            }
        }

        private static bool JsonIsOlderThan(DateTime dbWriteUtc)
        {
            var jsonWrite = TryLastWriteUtc(DataPath);
            if (jsonWrite == null) return false;
            return jsonWrite.Value < dbWriteUtc;
        }

        private static List<AccountRecord>? ReadLegacyJson()
        {
            var storedText = AndroidFileBridge.ReadText(DataPath);
            if (storedText == null) return null;
            if (!AccountDataCrypto.TryUnseal(storedText, out var jsonText, out var cryptoError))
            {
                Console.WriteLine($"[AccountStore] DECRYPT FAIL {DataPath}: {cryptoError}");
                BackupUnreadable(storedText);
                return null;
            }
            return ParseAccountsManual(jsonText);
        }

        private static void BackupUnreadable(string text)
        {
            var badPath = DataPath + ".bad-" + DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                AndroidFileBridge.WriteText(badPath, text);
                Console.WriteLine($"[AccountStore] unreadable data backed up to {badPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountStore] backup unreadable data FAIL: {ex.Message}");
            }
        }

        private static void Save_NoLock()
        {

            Console.WriteLine($"[ACC-SAVE-ENTER {DbPath}]");
            try
            {
                AccountDb.UpsertAll(DbPath, _accounts!);
                Console.WriteLine($"[ACC-SAVE-OK n={_accounts!.Count}]");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountStore] SAVE FAIL {DbPath}: {ex}");
            }
        }

        private static List<AccountRecord> ParseAccountsManual(string text)
        {
            var list = new List<AccountRecord>();
            var incomplete = 0;
            var ghost = 0;
            try
            {
                int i = 0;
                var n = text.Length;
                while (i < n)
                {
                    while (i < n && text[i] != '{') i++;
                    if (i >= n) break;
                    i++;
                    var rec = new AccountRecord();
                    var closed = false;
                    while (i < n)
                    {
                        SkipBlank(text, ref i);
                        if (i >= n) break;
                        if (text[i] == ',') { i++; continue; }
                        if (text[i] == '}') { i++; closed = true; break; }
                        if (text[i] != '"') break;
                        var key = ReadJsonString(text, ref i);
                        SkipBlank(text, ref i);
                        if (i < n && text[i] == ':') i++;
                        SkipBlank(text, ref i);
                        ApplyAccountField(rec, key, text, ref i);
                    }

                    if (closed)
                    {
                        if (rec.ViewerId > 0) list.Add(rec);
                        else ghost++;
                    }
                    else incomplete++;
                    if (i < n && text[i] == '{') i++;
                }
                if (incomplete > 0 || ghost > 0)
                    Console.WriteLine($"[AccountStore] PARSE {DataPath}: accepted {list.Count}," +
                        $"skipped {incomplete} incomplete + {ghost} without ViewerId (import incomplete; verify counts)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountStore] PARSE FAIL {DataPath}: {ex}");
                return new List<AccountRecord>();
            }
            return list;
        }

        private static void ApplyAccountField(AccountRecord rec, string key, string s, ref int i)
        {
            switch (key)
            {
                case "ViewerId": rec.ViewerId = ReadJsonInt(s, ref i); break;
                case "AvatarId": rec.AvatarId = ReadJsonInt(s, ref i); break;
                case "BoardId": rec.BoardId = ReadJsonInt(s, ref i); break;
                case "BattlePoint": rec.BattlePoint = ReadJsonInt(s, ref i); break;
                case "TutorialStep": rec.TutorialStep = ReadJsonInt(s, ref i); break;
                case "Name": rec.Name = ReadJsonString(s, ref i); break;
                case "AuthenticationToken": rec.AuthenticationToken = ReadJsonString(s, ref i); break;
                case "DeviceIdentifier":
                    var b64 = ReadJsonString(s, ref i);
                    rec.DeviceIdentifier = b64.Length == 0 ? Array.Empty<byte>() : Convert.FromBase64String(b64);
                    break;
                case "OwnerAddress": rec.OwnerAddress = ReadJsonString(s, ref i); break;
                case "CreatedAtUtc": rec.CreatedAtUtc = ReadJsonUtc(s, ref i); break;
                case "LastLoggedInAtUtc": rec.LastLoggedInAtUtc = ReadJsonUtc(s, ref i); break;
                default: SkipJsonValue(s, ref i); break;
            }
        }

        private static void SkipBlank(string s, ref int i)
        {
            while (i < s.Length && (s[i] == ' ' || s[i] == '\t' || s[i] == '\n' || s[i] == '\r')) i++;
        }

        private static string ReadJsonString(string s, ref int i)
        {
            if (i >= s.Length || s[i] != '"')
            {
                SkipJsonValue(s, ref i);
                return "";
            }
            i++;
            var sb = new System.Text.StringBuilder();
            while (i < s.Length)
            {
                var c = s[i++];
                if (c == '"') break;
                if (c != '\\' || i >= s.Length) { sb.Append(c); continue; }
                var e = s[i++];
                switch (e)
                {
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 't': sb.Append('\t'); break;
                    case 'u':

                        if (i + 4 <= s.Length) { sb.Append((char)ReadHex4(s, i)); i += 4; }
                        break;
                    default: sb.Append(e); break;
                }
            }
            return sb.ToString();
        }

        private static int ReadHex4(string s, int at)
        {
            var v = 0;
            for (int k = 0; k < 4; k++)
            {
                var c = s[at + k];
                var d = c <= '9' ? c - '0' : (c | 0x20) - 'a' + 10;
                v = v * 16 + d;
            }
            return v;
        }

        private static int ReadJsonInt(string s, ref int i)
        {
            SkipBlank(s, ref i);
            var neg = i < s.Length && s[i] == '-';
            if (neg || (i < s.Length && s[i] == '+')) i++;
            long v = 0;
            while (i < s.Length && s[i] >= '0' && s[i] <= '9') { v = v * 10 + (s[i] - '0'); i++; }
            return (int)(neg ? -v : v);
        }

        private static void SkipJsonValue(string s, ref int i)
        {
            SkipBlank(s, ref i);
            if (i < s.Length && s[i] == '"') { ReadJsonString(s, ref i); return; }
            while (i < s.Length && s[i] != ',' && s[i] != '}' && s[i] != ']'
                   && s[i] != ' ' && s[i] != '\t' && s[i] != '\n' && s[i] != '\r') i++;
        }

        private static DateTime ReadJsonUtc(string s, ref int i)
        {
            var t = ReadJsonString(s, ref i);
            if (t.Length < 20) return default;
            var dt = new DateTime(Digits(t, 0, 4), Digits(t, 5, 2), Digits(t, 8, 2),
                Digits(t, 11, 2), Digits(t, 14, 2), Digits(t, 17, 2), DateTimeKind.Unspecified);
            long ticks = 0;
            int p = 19;
            if (p < t.Length && t[p] == '.')
            {
                p++;
                var digits = 0;
                while (p < t.Length && t[p] >= '0' && t[p] <= '9' && digits < 7)
                {
                    ticks = ticks * 10 + (t[p] - '0');
                    p++;
                    digits++;
                }
                while (digits < 7) { ticks *= 10; digits++; }
            }
            if (p < t.Length && (t[p] == '+' || t[p] == '-'))
            {
                var sign = t[p] == '-' ? 1L : -1L;
                p++;
                ticks += sign * (Digits(t, p, 2) * 60L + Digits(t, p + 3, 2)) * TimeSpan.TicksPerMinute;
            }
            return new DateTime(dt.Ticks + ticks, DateTimeKind.Utc);
        }

        private static int Digits(string t, int at, int len)
        {
            var v = 0;
            for (int k = 0; k < len && at + k < t.Length; k++)
            {
                var c = t[at + k];
                if (c < '0' || c > '9') break;
                v = v * 10 + (c - '0');
            }
            return v;
        }

        private static string NewToken()
        {
            Span<byte> buf = stackalloc byte[40];
            RandomNumberGenerator.Fill(buf);
            return Convert.ToBase64String(buf);
        }

        private static byte[] NewDevice()
        {
            var dev = new byte[16];
            dev[0] = 0x01;
            RandomNumberGenerator.Fill(dev.AsSpan(1));
            return dev;
        }
    }
}
