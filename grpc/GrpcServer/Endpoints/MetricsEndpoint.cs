using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Byac.Services;
using System;
using System.Threading.Tasks;

namespace Byac.Server
{

    public static class MetricsEndpoint
    {
        public static int Port =>
            int.TryParse(Environment.GetEnvironmentVariable("BYAC_METRICS_PORT"), out var p) ? p : 9446;

        public static void Map(IEndpointRouteBuilder endpoints) =>
            endpoints.MapGet("/metrics", Handle);

        private static Task Handle(HttpContext ctx)
        {
            var port = Port;
            if (port <= 0 || ctx.Connection.LocalPort != port)
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                return Task.CompletedTask;
            }

            ctx.Response.Headers.CacheControl = "no-store";
            ctx.Response.ContentType = "text/plain; version=0.0.4; charset=utf-8";
            return ctx.Response.WriteAsync(ServerMetrics.Render());
        }
    }
}
