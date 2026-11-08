using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Byac.ServerShared.Hub;
using Byac.Services;
using Byac.Services.Battle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Byac.Server.Gm
{

    public static class GmModule
    {
        private static readonly JsonSerializerOptions JsonOpts = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/gm/", Guard(ServePage));
            endpoints.MapGet("/gm/api/state", Guard(HandleState));
            endpoints.MapGet("/gm/api/metrics", Guard(HandleMetrics));
            endpoints.MapPost("/gm/api/player", Guard(HandlePlayerEdit));
            endpoints.MapPost("/gm/api/unit", Guard(HandleAddUnit));
            endpoints.MapPost("/gm/api/equipment", Guard(HandleAddEquipment));
            endpoints.MapPost("/gm/api/force", Guard(HandleForce));
            endpoints.MapPost("/gm/api/bp", Guard(HandleBpEdit));
            endpoints.MapPost("/gm/api/verify-mode", Guard(HandleVerifyMode));
            endpoints.MapGet("/gm/api/report-rate", Guard(HandleReportRateRead));
            endpoints.MapPost("/gm/api/report-rate", Guard(HandleReportRate));
            endpoints.MapGet("/gm/api/glog", Guard(HandleGlog));
            endpoints.MapGet("/gm/api/leaderboard", Guard(HandleLeaderboard));

            endpoints.MapPost("/gm/api/bind-account", Guard(HandleBindAccount));
        }

        public static bool IsRequestAllowed(HttpContext ctx)
        {
            var gmPort = int.TryParse(Environment.GetEnvironmentVariable("BYAC_GM_PORT"), out var p) ? p : 9445;
            if (gmPort <= 0 || ctx.Connection.LocalPort != gmPort) return false;
            if (string.Equals(Environment.GetEnvironmentVariable("BYAC_GM_BIND"), "any",
                    StringComparison.OrdinalIgnoreCase))
                return true;
            var ip = ctx.Connection.RemoteIpAddress;
            if (ip != null && ip.IsIPv4MappedToIPv6) ip = ip.MapToIPv4();
            return ip != null && System.Net.IPAddress.IsLoopback(ip);
        }

        private static RequestDelegate Guard(RequestDelegate next) => async ctx =>
        {
            if (!IsRequestAllowed(ctx))
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
            await next(ctx);
        };

        private static string HtmlPath
        {
            get
            {
                var env = Environment.GetEnvironmentVariable("BYAC_GM_HTML");
                if (!string.IsNullOrEmpty(env)) return env;
                var src = Path.Combine(AppContext.BaseDirectory, "gm", "index.html");
                if (File.Exists(src)) return src;
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gm", "index.html");
            }
        }

        private static string GlogPath =>
            Environment.GetEnvironmentVariable("GSERVER_LOG") ?? @"glog.txt";

        private static string VerifyModePath => Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "verify-mode.json");

        private static async Task JsonAsync(HttpContext ctx, int status, object payload)
        {
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/json; charset=utf-8";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOpts));
        }

        private static async Task<(bool ok, JsonElement body, string err)> ReadBodyAsync(HttpContext ctx)
        {
            try
            {
                using var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8);
                var text = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(text)) return (true, default, "");
                using var doc = JsonDocument.Parse(text);
                return (true, doc.RootElement.Clone(), "");
            }
            catch (Exception ex)
            {
                return (false, default, "bad_json: " + ex.Message);
            }
        }

        private static string Err(string message) =>
            JsonSerializer.Serialize(new { ok = false, error = message }, JsonOpts);

        private static void Log(string msg) => Console.WriteLine($"[GM] {msg}");

        private static (BattleSession? session, string err) ResolveSession(JsonElement body)
        {
            if (body.ValueKind == JsonValueKind.Object &&
                body.TryGetProperty("battleId", out var bid) &&
                bid.ValueKind == JsonValueKind.String)
            {
                if (!Ulid.TryParse(bid.GetString(), out var id))
                    return (null, "bad_battleId");
                var found = BattleRegistry.Snapshot().FirstOrDefault(s => s.BattleId == id);
                return found == null ? (null, "session_not_found") : (found, "");
            }
            var all = BattleRegistry.Snapshot();
            if (all.Length == 1) return (all[0], "");
            if (all.Length == 0) return (null, "no_session");
            return (null, "multiple_sessions_specify_battleId");
        }

        private static Task HandleState(HttpContext ctx)
        {
            var sessions = BattleRegistry.Snapshot().Select(s =>
            {
                double? prepRemain = null;
                if (s.PreparationDeadlineUtc.HasValue)
                    prepRemain = Math.Round((s.PreparationDeadlineUtc.Value - DateTime.UtcNow).TotalSeconds, 1);

                var players = s.Players.Select(p => new
                {
                    viewerId = p.ViewerId,
                    name = p.Name,
                    teamId = p.TeamId,
                    index = p.Index,
                    level = p.Level,
                    life = p.Life,
                    maxLife = p.MaxLife,
                    alive = p.Alive,
                    gold = p.Gold,
                    totalGold = p.TotalGold,
                    rankUpToken = p.RankUpToken,
                    partyPower = p.PartyPower,
                    wins = p.TotalWinsCount,
                    loses = p.TotalLoseCount,
                    isShopLock = p.IsShopLock,
                    shop = (p.ShopUnitList ?? new List<Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>())
                        .Select(UnitView).ToList(),
                    deck = (p.DeckUnitList ?? new List<Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>())
                        .Select(UnitView).ToList(),
                    field = (p.FieldUnitList ?? new List<Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo>())
                        .Select(UnitView).ToList(),
                    equipmentCount = p.EquipmentList?.Count ?? 0
                }).ToList();

                return new
                {
                    battleId = s.BattleId.ToString(),
                    phase = s.Phase.ToString(),
                    round = s.Round,
                    finished = s.Finished,
                    battleRunning = s.BattleRunning,
                    hasHumanAttached = s.HasHumanAttached,
                    humanViewerId = s.HumanViewerId,

                    humans = s.HumanViewerIds.Select(vid => new
                    {
                        viewerId = vid,
                        attached = s.IsAttached(vid),
                        disconnectedAgoSec = s.DisconnectAt(vid) is { } d ? (int)(DateTime.UtcNow - d).TotalSeconds : (int?)null,
                        aiDriven = s.IsDrivenByServer(vid),
                        life = s.FindPlayer(vid).Life
                    }).ToList(),
                    sequenceStarted = s.SequenceStarted,
                    preparationOpen = s.PreparationOpen,
                    prepareDeadlineUtc = s.PreparationDeadlineUtc?.ToString("o"),
                    prepareRemainSec = prepRemain,
                    players
                };
            }).ToList();

            var vm = new
            {
                prepareSecondsOverride = VerifyMode.PrepareSecondsOverride,
                damageMultiplier = VerifyMode.DamageMultiplier,
                skipBattleAnimation = VerifyMode.SkipBattleAnimation,
                instantReady = VerifyMode.InstantReady,
                maxRounds = VerifyMode.MaxRounds,
                isActive = VerifyMode.IsActive
            };

            var accounts = AccountStore.SnapshotAll().Select(a => new
            {
                viewerId = a.ViewerId,
                name = a.Name,
                battlePoint = a.BattlePoint,
                tutorialStep = a.TutorialStep,
                avatarId = a.AvatarId,
                boardId = a.BoardId,
                lastLoggedInAt = a.LastLoggedInAtUtc.ToString("o")
            }).ToList();

            var mm = MatchMakingHub.Metrics();
            var proc = System.Diagnostics.Process.GetCurrentProcess();
            var uptimeSec = Math.Max(0, (long)(DateTime.UtcNow - proc.StartTime.ToUniversalTime()).TotalSeconds);

            return JsonAsync(ctx, 200, new
            {
                serverVersion = BuildInfo.Version,
                serverTime = DateTime.UtcNow.ToString("o"),
                gmPort = Environment.GetEnvironmentVariable("BYAC_GM_PORT") ?? "9445",
                verifyMode = vm,
                matchmaking = new
                {
                    openWindows = mm.OpenWindows,
                    acceptingWindows = mm.AcceptingWindows,
                    waitingSeats = mm.WaitingSeats,
                    entryTotal = mm.EntryTotal,
                    settledTotal = mm.SettledTotal,
                    duplicateRejects = mm.DuplicateRejects
                },
                activeBattles = BattleRegistry.ActiveCount,
                process = new
                {
                    uptimeSec,
                    cpuSec = Math.Round(proc.TotalProcessorTime.TotalSeconds, 1),
                    workingSetMb = proc.WorkingSet64 / (1024 * 1024),
                    threads = proc.Threads.Count,
                    gcHeapMb = GC.GetTotalMemory(false) / (1024 * 1024)
                },
                sessions,
                accounts
            });
        }

        private static object UnitView(Byac.ServerShared.MessagePackObjects.BattleUnitDataMpo u) => new
        {
            key = u.UniqueKey,
            unitId = u.UnitId,
            rank = u.Rank,
            state = u.State.ToString(),
            posX = u.PositionX,
            posY = u.PositionY,
            index = u.Index,
            isPurchased = u.IsPurchased,
            isToken = u.IsToken,
            isNpc = u.IsNpc,
            equipmentCount = u.EquipmentList?.Count ?? 0
        };

        private static Task HandleMetrics(HttpContext ctx)
        {
            var mm = MatchMakingHub.Metrics();
            var proc = System.Diagnostics.Process.GetCurrentProcess();
            var uptimeSec = Math.Max(0, (long)(DateTime.UtcNow - proc.StartTime.ToUniversalTime()).TotalSeconds);
            var sessions = BattleRegistry.Snapshot().Select(s => new
            {
                battleId = s.BattleId.ToString(),
                phase = s.Phase.ToString(),
                round = s.Round,
                finished = s.Finished,
                sequenceStarted = s.SequenceStarted,
                humans = s.HumanViewerIds.Select(vid => new
                {
                    viewerId = vid,
                    attached = s.IsAttached(vid),
                    aiDriven = s.IsDrivenByServer(vid),
                    life = s.FindPlayer(vid).Life
                }).ToList(),
                playerIds = s.Players.Select(p => p.ViewerId).ToList()
            }).ToList();

            return JsonAsync(ctx, 200, new
            {
                serverTime = DateTime.UtcNow.ToString("o"),
                matchmaking = new
                {
                    openWindows = mm.OpenWindows,
                    acceptingWindows = mm.AcceptingWindows,
                    waitingSeats = mm.WaitingSeats,
                    entryTotal = mm.EntryTotal,
                    settledTotal = mm.SettledTotal,
                    duplicateRejects = mm.DuplicateRejects
                },
                activeBattles = BattleRegistry.ActiveCount,
                process = new
                {
                    uptimeSec,
                    cpuSec = Math.Round(proc.TotalProcessorTime.TotalSeconds, 1),
                    workingSetMb = proc.WorkingSet64 / (1024 * 1024),
                    threads = proc.Threads.Count,
                    gcHeapMb = GC.GetTotalMemory(false) / (1024 * 1024)
                },
                sessions
            });
        }

        private static async Task HandlePlayerEdit(HttpContext ctx)
        {
            var (ok, body, err) = await ReadBodyAsync(ctx);
            if (!ok) { await JsonAsync(ctx, 400, new { ok = false, error = err }); return; }
            if (body.ValueKind != JsonValueKind.Object ||
                !body.TryGetProperty("viewerId", out var vid) || !vid.TryGetInt32(out var viewerId))
            { await JsonAsync(ctx, 400, new { ok = false, error = "viewerId required" }); return; }

            var (session, serr) = ResolveSession(body);
            if (session == null) { await JsonAsync(ctx, 404, new { ok = false, error = serr }); return; }

            int? life = body.TryGetProperty("life", out var l) && l.ValueKind != JsonValueKind.Null ? l.GetInt32() : null;
            int? gold = body.TryGetProperty("gold", out var g) && g.ValueKind != JsonValueKind.Null ? g.GetInt32() : null;
            int? token = body.TryGetProperty("rankUpToken", out var t) && t.ValueKind != JsonValueKind.Null ? t.GetInt32() : null;
            if (life == null && gold == null && token == null)
            { await JsonAsync(ctx, 400, new { ok = false, error = "life/gold/rankUpToken required" }); return; }

            var applied = session.EditPlayerInfoOp(viewerId, life, gold, token);
            if (applied) session.BroadcastUpdatePlayers();
            Log($"player edit viewer={viewerId} life={life?.ToString() ?? "-"} gold={gold?.ToString() ?? "-"} token={token?.ToString() ?? "-"} -> {applied}");
            await JsonAsync(ctx, applied ? 200 : 404, new { ok = applied, error = applied ? null : "player_not_found" });
        }

        private static async Task HandleAddUnit(HttpContext ctx)
        {
            var (ok, body, err) = await ReadBodyAsync(ctx);
            if (!ok) { await JsonAsync(ctx, 400, new { ok = false, error = err }); return; }
            if (body.ValueKind != JsonValueKind.Object ||
                !body.TryGetProperty("viewerId", out var vid) || !vid.TryGetInt32(out var viewerId) ||
                !body.TryGetProperty("unitId", out var uid) || !uid.TryGetInt32(out var unitId))
            { await JsonAsync(ctx, 400, new { ok = false, error = "viewerId+unitId required" }); return; }
            var rank = body.TryGetProperty("rank", out var r) && r.TryGetInt32(out var rv) ? rv : 1;

            var (session, serr) = ResolveSession(body);
            if (session == null) { await JsonAsync(ctx, 404, new { ok = false, error = serr }); return; }

            var res = session.DebugAddUnitOp(viewerId, unitId, rank);
            if (res.Ok) session.BroadcastUpdatePlayers();
            Log($"add unit viewer={viewerId} unitId={unitId} rank={rank} -> ok={res.Ok} {res.Reason} key={res.UniqueKey}");
            await JsonAsync(ctx, res.Ok ? 200 : 400, new { ok = res.Ok, error = res.Ok ? null : res.Reason, uniqueKey = res.Ok ? res.UniqueKey : 0 });
        }

        private static async Task HandleAddEquipment(HttpContext ctx)
        {
            var (ok, body, err) = await ReadBodyAsync(ctx);
            if (!ok) { await JsonAsync(ctx, 400, new { ok = false, error = err }); return; }
            if (body.ValueKind != JsonValueKind.Object ||
                !body.TryGetProperty("viewerId", out var vid) || !vid.TryGetInt32(out var viewerId) ||
                !body.TryGetProperty("equipmentId", out var eid) || !eid.TryGetInt32(out var equipmentId))
            { await JsonAsync(ctx, 400, new { ok = false, error = "viewerId+equipmentId required" }); return; }

            var (session, serr) = ResolveSession(body);
            if (session == null) { await JsonAsync(ctx, 404, new { ok = false, error = serr }); return; }

            var res = session.DebugAddEquipmentOp(viewerId, equipmentId);
            if (res.Ok) session.BroadcastUpdatePlayers();
            Log($"add equipment viewer={viewerId} equipId={equipmentId} -> ok={res.Ok} {res.Reason} key={res.UniqueKey}");
            await JsonAsync(ctx, res.Ok ? 200 : 400, new { ok = res.Ok, error = res.Ok ? null : res.Reason, uniqueKey = res.Ok ? res.UniqueKey : 0 });
        }

        private static async Task HandleForce(HttpContext ctx)
        {
            var (ok, body, err) = await ReadBodyAsync(ctx);
            if (!ok) { await JsonAsync(ctx, 400, new { ok = false, error = err }); return; }
            var action = body.TryGetProperty("action", out var a) ? a.GetString() : null;
            var (session, serr) = ResolveSession(body);
            if (session == null) { await JsonAsync(ctx, 404, new { ok = false, error = serr }); return; }

            var caller = body.TryGetProperty("callerViewerId", out var c) && c.TryGetInt32(out var cv)
                ? cv : session.HumanViewerId;

            switch (action)
            {
                case "complete":
                    session.ForceCompleteWaitForCommandOp(caller);
                    break;
                case "surrender":
                    session.ForceSurrenderPlayersOp(caller);
                    break;
                default:
                    await JsonAsync(ctx, 400, new { ok = false, error = "action must be complete|surrender" });
                    return;
            }
            Log($"force action={action} caller={caller}");
            await JsonAsync(ctx, 200, new { ok = true });
        }

        private static async Task HandleBpEdit(HttpContext ctx)
        {
            var (ok, body, err) = await ReadBodyAsync(ctx);
            if (!ok) { await JsonAsync(ctx, 400, new { ok = false, error = err }); return; }
            if (body.ValueKind != JsonValueKind.Object ||
                !body.TryGetProperty("viewerId", out var vid) || !vid.TryGetInt32(out var viewerId) ||
                !body.TryGetProperty("battlePoint", out var bp) || !bp.TryGetInt32(out var target))
            { await JsonAsync(ctx, 400, new { ok = false, error = "viewerId+battlePoint required" }); return; }

            var current = AccountStore.BattlePointOf(viewerId);
            var delta = target - current;
            var final = AccountStore.AddBattlePoint(viewerId, delta);
            Log($"bp edit viewer={viewerId} {current} -> {final}");
            await JsonAsync(ctx, 200, new { ok = true, battlePoint = final, delta });
        }

        private static async Task HandleVerifyMode(HttpContext ctx)
        {
            var (ok, body, err) = await ReadBodyAsync(ctx);
            if (!ok) { await JsonAsync(ctx, 400, new { ok = false, error = err }); return; }

            if (body.ValueKind == JsonValueKind.Object &&
                body.TryGetProperty("reset", out var rst) && rst.ValueKind == JsonValueKind.True)
            {
                if (File.Exists(VerifyModePath)) File.Delete(VerifyModePath);
                VerifyMode.Load();
                Log("verify-mode reset (file deleted)");
                await JsonAsync(ctx, 200, new { ok = true, verifyMode = new { prepareSecondsOverride = VerifyMode.PrepareSecondsOverride, damageMultiplier = VerifyMode.DamageMultiplier, skipBattleAnimation = VerifyMode.SkipBattleAnimation, instantReady = VerifyMode.InstantReady, maxRounds = VerifyMode.MaxRounds, isActive = VerifyMode.IsActive } });
                return;
            }

            int IntOf(string name, int def) =>
                body.ValueKind == JsonValueKind.Object && body.TryGetProperty(name, out var e) && e.TryGetInt32(out var v) ? v : def;
            double DblOf(string name, double def) =>
                body.ValueKind == JsonValueKind.Object && body.TryGetProperty(name, out var e) && e.TryGetDouble(out var v) ? v : def;
            bool BoolOf(string name, bool def) =>
                body.ValueKind == JsonValueKind.Object && body.TryGetProperty(name, out var e) && e.ValueKind is JsonValueKind.True or JsonValueKind.False ? e.GetBoolean() : def;
            var purpose = body.ValueKind == JsonValueKind.Object && body.TryGetProperty("purpose", out var pp) ? pp.GetString() : null;

            var payload = new Dictionary<string, object>
            {
                ["prepareSecondsOverride"] = IntOf("prepareSecondsOverride", VerifyMode.PrepareSecondsOverride),
                ["damageMultiplier"] = DblOf("damageMultiplier", VerifyMode.DamageMultiplier),
                ["skipBattleAnimation"] = BoolOf("skipBattleAnimation", VerifyMode.SkipBattleAnimation),
                ["instantReady"] = BoolOf("instantReady", VerifyMode.InstantReady),
                ["maxRounds"] = IntOf("maxRounds", VerifyMode.MaxRounds),
                ["_note"] = "verify-mode temp config — delete after acceptance (SKILL §6.2)",
                ["_changed_at"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                ["_changed_by"] = "gm-tool",
                ["_purpose"] = string.IsNullOrWhiteSpace(purpose) ? "GM tool edit" : purpose
            };
            File.WriteAllText(VerifyModePath, JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
            VerifyMode.Load();
            Log($"verify-mode written: prep={payload["prepareSecondsOverride"]} dmg={payload["damageMultiplier"]} instantReady={payload["instantReady"]} maxRounds={payload["maxRounds"]}");
            await JsonAsync(ctx, 200, new { ok = true, verifyMode = new { prepareSecondsOverride = VerifyMode.PrepareSecondsOverride, damageMultiplier = VerifyMode.DamageMultiplier, skipBattleAnimation = VerifyMode.SkipBattleAnimation, instantReady = VerifyMode.InstantReady, maxRounds = VerifyMode.MaxRounds, isActive = VerifyMode.IsActive } });
        }

        private static async Task HandleReportRateRead(HttpContext ctx) =>
            await JsonAsync(ctx, 200, new { ok = true, reportRate = ReportConfig.Percent, source = ReportConfig.Source });

        private static async Task HandleReportRate(HttpContext ctx)
        {
            var (ok, body, err) = await ReadBodyAsync(ctx);
            if (!ok) { await JsonAsync(ctx, 400, new { ok = false, error = err }); return; }

            var before = ReportConfig.Percent;
            var reset = body.ValueKind == JsonValueKind.Object &&
                body.TryGetProperty("reset", out var r) && r.ValueKind == JsonValueKind.True;

            int? percent = body.ValueKind == JsonValueKind.Object &&
                body.TryGetProperty("percent", out var p) && p.TryGetInt32(out var pv) ? pv : null;

            var now = reset ? ReportConfig.Reset() : percent.HasValue ? ReportConfig.Set(percent.Value) : ReportConfig.Percent;
            Log($"report-rate {(reset ? "reset" : percent.HasValue ? $"set {percent}%" : "read")} : {before}% → {now}% ({ReportConfig.Source})");
            await JsonAsync(ctx, 200, new { ok = true, reportRate = now, source = ReportConfig.Source, previous = before });
        }

        private static async Task HandleGlog(HttpContext ctx)
        {
            var n = 200;
            if (int.TryParse(ctx.Request.Query["n"], out var qn) && qn > 0) n = Math.Min(qn, 2000);

            try
            {
                if (!File.Exists(GlogPath))
                { await JsonAsync(ctx, 200, new { ok = true, lines = Array.Empty<string>(), note = "glog not found" }); return; }

                byte[] tail;
                using (var fs = new FileStream(GlogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var len = fs.Length;
                    var take = (int)Math.Min(len, 512 * 1024);
                    fs.Seek(-take, SeekOrigin.End);
                    tail = new byte[take];
                    var read = 0;
                    while (read < take)
                    {
                        var r = fs.Read(tail, read, take - read);
                        if (r <= 0) break;
                        read += r;
                    }
                    if (read < take) Array.Resize(ref tail, read);
                }

                var text = Encoding.UTF8.GetString(tail);
                var nl = text.IndexOf('\n');
                if (nl >= 0) text = text.Substring(nl + 1);
                var lines = text.Replace("\r\n", "\n").Split('\n');
                var tailLines = lines.Skip(Math.Max(0, lines.Length - n)).ToArray();
                await JsonAsync(ctx, 200, new { ok = true, lines = tailLines });
            }
            catch (Exception ex)
            {
                await JsonAsync(ctx, 500, new { ok = false, error = ex.Message });
            }
        }

        private static async Task HandleLeaderboard(HttpContext ctx)
        {
            var q = ctx.Request.Query;
            var period = q["period"].ToString().ToLowerInvariant();
            if (period != "week" && period != "month") period = "both";
            var bucket = q["bucket"].ToString().Trim();
            var flag = q["includeAi"].ToString().ToLowerInvariant();
            var includeAi = flag == "1" || flag == "true";
            var limit = 30;
            if (int.TryParse(q["limit"], out var ql) && ql > 0) limit = Math.Min(ql, 200);
            var minPicks = 1;
            if (int.TryParse(q["minPicks"], out var qm) && qm >= 0) minPicks = qm;

            try
            {
                var payload = new Dictionary<string, object>
                {
                    ["ok"] = true,
                    ["includeAi"] = includeAi,
                    ["limit"] = limit,
                    ["minPicks"] = minPicks,
                };
                if (period != "month")
                    payload["week"] = LeaderboardStore.Query("week",
                        period == "week" && bucket.Length > 0 ? bucket : null, includeAi, limit, minPicks);
                if (period != "week")
                    payload["month"] = LeaderboardStore.Query("month",
                        period == "month" && bucket.Length > 0 ? bucket : null, includeAi, limit, minPicks);
                await JsonAsync(ctx, 200, payload);
            }
            catch (Exception ex)
            {
                await JsonAsync(ctx, 500, new { ok = false, error = ex.Message });
            }
        }

        private static async Task HandleBindAccount(HttpContext ctx)
        {
            var (ok, body, err) = await ReadBodyAsync(ctx);
            if (!ok) { await JsonAsync(ctx, 400, new { ok = false, error = err }); return; }
            if (body.ValueKind != JsonValueKind.Object ||
                !body.TryGetProperty("viewerId", out var vid) || !vid.TryGetInt32(out var viewerId) ||
                !body.TryGetProperty("account", out var acct) || acct.ValueKind != JsonValueKind.String ||
                !body.TryGetProperty("password", out var pw) || pw.ValueKind != JsonValueKind.String)
            { await JsonAsync(ctx, 400, new { ok = false, error = "viewerId+account+password required" }); return; }

            var error = AccountStore.BindAccount(viewerId, acct.GetString(), pw.GetString());
            if (error != null) { await JsonAsync(ctx, 400, new { ok = false, error }); return; }
            Log($"bind-account viewer={viewerId}");
            await JsonAsync(ctx, 200, new { ok = true, viewerId, account = AccountStore.GetOrDefault(viewerId).Account });
        }

        private static async Task ServePage(HttpContext ctx)
        {
            try
            {
                var path = HtmlPath;
                if (!File.Exists(path))
                {
                    ctx.Response.StatusCode = 404;
                    await ctx.Response.WriteAsync($"index.html not found at {path}");
                    return;
                }
                ctx.Response.ContentType = "text/html; charset=utf-8";
                await ctx.Response.SendFileAsync(path);
            }
            catch (Exception ex)
            {
                ctx.Response.StatusCode = 500;
                await ctx.Response.WriteAsync(ex.Message);
            }
        }
    }
}
