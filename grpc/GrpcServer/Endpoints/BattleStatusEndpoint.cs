using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Byac.Services.Battle;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Byac.Server
{

    public static class BattleStatusEndpoint
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private const string DefaultWatchKey = "byac-watch-2026-10";

        private static string WatchKey =>
            Environment.GetEnvironmentVariable("BYAC_WATCH_KEY") is { Length: > 0 } k ? k : DefaultWatchKey;

        public static void Map(IEndpointRouteBuilder endpoints) =>
            endpoints.MapGet("/battle-status", Handle).RequireRateLimiting("public");

        private static async Task Handle(HttpContext ctx)
        {
            if (!string.Equals(ctx.Request.Headers["x-byac-watch"], WatchKey, StringComparison.Ordinal))
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            if (!int.TryParse(ctx.Request.Query["viewerId"], out var viewerId) || viewerId <= 0)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                await ctx.Response.WriteAsync("{\"error\":\"bad_viewerId\"}");
                return;
            }

            var hit = BattleRegistry.Snapshot()
                .Where(s => !s.Finished && s.HumanViewerIds.Contains(viewerId))
                .OrderByDescending(s => s.CreatedAtUtc)
                .FirstOrDefault();

            ctx.Response.Headers.CacheControl = "no-store";
            ctx.Response.ContentType = "application/json; charset=utf-8";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                inBattle = hit != null,
                battleId = hit?.BattleId.ToString(),
                phase = hit?.Phase.ToString(),
                round = hit?.Round ?? 0,
                attached = hit != null && hit.IsAttached(viewerId),
                ageSec = hit == null ? 0 : (int)Math.Max(0, (DateTime.UtcNow - hit.CreatedAtUtc).TotalSeconds),
                serverTime = DateTime.UtcNow.ToString("o"),

                report = ReportConfig.Percent,
            }, JsonOpts));
        }
    }
}
