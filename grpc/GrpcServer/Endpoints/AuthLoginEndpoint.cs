using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Byac.Server.Security;
using Byac.Services;
using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Byac.Server
{

    public static class AuthLoginEndpoint
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        private const long MaxBodyBytes = 4096;

        private static int EnvInt(string name, int fallback) =>
            int.TryParse(Environment.GetEnvironmentVariable(name), out var v) ? v : fallback;

        private static bool Enabled => EnvInt("BYAC_AUTH", 1) == 1;

        private static int RatePerWindow => EnvInt("BYAC_AUTH_RATE", 8);

        private static int MaxFailures => EnvInt("BYAC_AUTH_FAIL", 5);

        private static int LockSeconds => EnvInt("BYAC_AUTH_LOCK", 600);

        public static void Map(IEndpointRouteBuilder endpoints)
        {
            if (!Enabled)
            {
                Console.WriteLine("[AUTH] BYAC_AUTH=0: /auth/login not registered (client login dialog skips it automatically)");
                return;
            }
            endpoints.MapPost("/auth/login", Handle).RequireRateLimiting("public");
            endpoints.MapGet("/auth/login", Probe).RequireRateLimiting("public");
        }

        private static Task Probe(HttpContext ctx)
        {
            ctx.Response.StatusCode = StatusCodes.Status200OK;
            ctx.Response.Headers.CacheControl = "no-store";
            ctx.Response.ContentType = "application/json; charset=utf-8";
            return ctx.Response.WriteAsync("{\"enabled\":true,\"allow\":\"POST\"}");
        }

        private sealed class LoginBody
        {
            public string Account { get; set; } = "";
            public string Password { get; set; } = "";
        }

        private static async Task Handle(HttpContext ctx)
        {

            if (ctx.Request.ContentLength is > MaxBodyBytes)
            {
                await Reply(ctx, StatusCodes.Status400BadRequest, new { error = "bad_body" });
                return;
            }
            var cap = ctx.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (cap != null && !cap.IsReadOnly) cap.MaxRequestBodySize = MaxBodyBytes;

            LoginBody body;
            try
            {
                body = await ctx.Request.ReadFromJsonAsync<LoginBody>(JsonOpts) ?? new LoginBody();
            }
            catch (JsonException)
            {
                await Reply(ctx, StatusCodes.Status400BadRequest, new { error = "bad_body" });
                return;
            }
            catch (BadHttpRequestException)
            {

                await Reply(ctx, StatusCodes.Status400BadRequest, new { error = "bad_body" });
                return;
            }

            var ip = ClientIp.Key(ctx);
            var now = DateTime.UtcNow;
            if (Attempts.IsLocked(ip, now))
            {
                Console.WriteLine($"[Auth] LOCKED ip={LogSafe.Address(ip)} -> 423");
                await Reply(ctx, StatusCodes.Status423Locked, new { error = "locked" });
                return;
            }
            if (!Attempts.TryConsume(ip, now, RatePerWindow))
            {
                Console.WriteLine($"[Auth] RATE ip={LogSafe.Address(ip)} -> 429");
                await Reply(ctx, StatusCodes.Status429TooManyRequests, new { error = "rate_limited" });
                return;
            }

            var hasXff = !string.IsNullOrWhiteSpace(ctx.Request.Headers["X-Forwarded-For"].ToString());
            var viaProxy = PublicGuardOptions.TlsPort > 0 || (PublicGuardOptions.TrustProxy && hasXff);
            var result = AccountStore.LoginOrCreate(body.Account, body.Password, ip, viaProxy, out var rec);

            switch (result)
            {
                case AccountStore.LoginResult.Ok:
                case AccountStore.LoginResult.Created:
                    Attempts.NoteSuccess(ip);
                    ctx.Response.Headers.CacheControl = "no-store";
                    await Reply(ctx, StatusCodes.Status200OK, new
                    {
                        result = result == AccountStore.LoginResult.Created ? "created" : "ok",
                        viewerId = rec.ViewerId,
                        accessToken = rec.AuthenticationToken,
                        deviceIdentifier = Convert.ToBase64String(rec.DeviceIdentifier ?? Array.Empty<byte>()),
                    });
                    return;

                case AccountStore.LoginResult.WrongPassword:
                    var locked = Attempts.NoteFailure(ip, now, MaxFailures, LockSeconds);
                    Console.WriteLine($"[Auth] FAIL ip={LogSafe.Address(ip)} wrong password{(locked ? " (locked)" : "")}");
                    await Reply(ctx, locked ? StatusCodes.Status423Locked : StatusCodes.Status401Unauthorized,
                        new { error = locked ? "locked" : "wrong_password" });
                    return;

                case AccountStore.LoginResult.BadAccount:
                    await Reply(ctx, StatusCodes.Status400BadRequest, new { error = "bad_account" });
                    return;

                default:
                    await Reply(ctx, StatusCodes.Status400BadRequest, new { error = "bad_password" });
                    return;
            }
        }

        private static async Task Reply(HttpContext ctx, int status, object payload)
        {
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/json; charset=utf-8";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOpts));
        }

        private static class Attempts
        {
            private sealed class Cell
            {
                public long WindowStartTicks;
                public int Used;
                public int Failures;
                public long LockedUntilTicks;
            }

            private const long WindowTicks = TimeSpan.TicksPerMinute;
            private const int PruneAt = 4096;

            private static readonly ConcurrentDictionary<string, Cell> Cells = new();

            private static Cell Get(string ip, DateTime now)
            {
                var cell = Cells.GetOrAdd(ip, _ => new Cell());
                if (Cells.Count > PruneAt) Prune(now);
                return cell;
            }

            private static void Rollover(Cell cell, long nowTicks)
            {
                if (nowTicks - cell.WindowStartTicks >= WindowTicks || nowTicks < cell.WindowStartTicks)
                {
                    cell.WindowStartTicks = nowTicks;
                    cell.Used = 0;
                }
            }

            public static bool IsLocked(string ip, DateTime now) =>
                Cells.TryGetValue(ip, out var c) && c.LockedUntilTicks > now.Ticks;

            public static bool TryConsume(string ip, DateTime now, int limit)
            {
                if (limit <= 0) return true;
                var cell = Get(ip, now);
                lock (cell)
                {
                    Rollover(cell, now.Ticks);
                    if (cell.Used >= limit) return false;
                    cell.Used++;
                    return true;
                }
            }

            public static bool NoteFailure(string ip, DateTime now, int maxFailures, int lockSeconds)
            {
                var cell = Get(ip, now);
                lock (cell)
                {
                    Rollover(cell, now.Ticks);
                    cell.Failures++;
                    if (maxFailures <= 0 || lockSeconds <= 0 || cell.Failures < maxFailures) return false;
                    cell.LockedUntilTicks = now.AddSeconds(lockSeconds).Ticks;
                    cell.Failures = 0;
                    return true;
                }
            }

            public static void NoteSuccess(string ip)
            {
                if (Cells.TryGetValue(ip, out var cell))
                    lock (cell) cell.Failures = 0;
            }

            private static void Prune(DateTime now)
            {
                var t = now.Ticks;
                foreach (var (ip, cell) in Cells)
                    if (t - cell.WindowStartTicks > WindowTicks * 5 && cell.LockedUntilTicks <= t)
                        Cells.TryRemove(ip, out _);
            }
        }
    }
}
