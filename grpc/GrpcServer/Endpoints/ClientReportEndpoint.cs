using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Byac.Server
{

    public static class ClientReportEndpoint
    {
        private const int MaxBodyBytes = 8 * 1024;
        private const int MaxLogChars = 2048;

        private const string DefaultWatchKey = "byac-watch-2026-10";

        private static string WatchKey =>
            Environment.GetEnvironmentVariable("BYAC_WATCH_KEY") is { Length: > 0 } k ? k : DefaultWatchKey;

        public static void Map(IEndpointRouteBuilder endpoints) =>
            endpoints.MapPost("/client/battle-report", Handle).RequireRateLimiting("public");

        private static async Task Handle(HttpContext ctx)
        {
            if (!string.Equals(ctx.Request.Headers["x-byac-watch"], WatchKey, StringComparison.Ordinal))
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }
            if (ctx.Request.ContentLength is > MaxBodyBytes)
            {
                ctx.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                return;
            }

            using var ms = new MemoryStream(MaxBodyBytes + 1);
            var buf = new byte[8 * 1024];
            int got;
            while ((got = await ctx.Request.Body.ReadAsync(buf, 0, buf.Length)) > 0)
            {
                if (ms.Length + got > MaxBodyBytes)
                {
                    ctx.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
                    return;
                }
                ms.Write(buf, 0, got);
            }
            var body = Encoding.UTF8.GetString(ms.ToArray());

            try
            {
                using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(body) ? "{}" : body);
                if (doc.RootElement.ValueKind != JsonValueKind.Object)
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await ctx.Response.WriteAsync("{\"error\":\"bad_shape\"}");
                    return;
                }

                var sb = new StringBuilder(256);
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    if (sb.Length > MaxLogChars) { sb.Append(" …"); break; }
                    var val = prop.Value.ValueKind is JsonValueKind.Object or JsonValueKind.Array
                        ? prop.Value.GetRawText()
                        : prop.Value.ToString();
                    if (val.Length > 256) val = val[..253] + "…";
                    sb.Append(OneLine(prop.Name)).Append('=').Append(OneLine(val)).Append(' ');
                }
                Console.WriteLine($"[REPORT] {sb}");
                ctx.Response.StatusCode = StatusCodes.Status204NoContent;
            }
            catch (JsonException)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                await ctx.Response.WriteAsync("{\"error\":\"bad_json\"}");
            }
        }

        private static string OneLine(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            foreach (var c in s)
            {
                if (c < ' ')
                {
                    var one = new StringBuilder(s.Length + 8);
                    foreach (var ch in s)
                    {
                        switch (ch)
                        {
                            case '\r': one.Append("\\r"); break;
                            case '\n': one.Append("\\n"); break;
                            case '\t': one.Append("\\t"); break;
                            default:
                                if (ch < ' ') one.Append("\\x").Append(((int)ch).ToString("X2"));
                                else one.Append(ch);
                                break;
                        }
                    }
                    return one.ToString();
                }
            }
            return s;
        }
    }
}
