using MagicOnion.Server;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Byac.ServerShared.MessagePack.Formatters;
using Byac.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace Byac.Server
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddGrpc(o =>
            {
                if (Security.PublicGuardOptions.GzipResponses)
                {
                    o.ResponseCompressionAlgorithm = "gzip";
                    o.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Fastest;
                }
            });

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                var perMin = Security.PublicGuardOptions.RatePerMin;
                options.AddPolicy("public", context =>
                {

                    var ip = Security.ClientIp.Key(context);
                    if (!Security.PublicGuardOptions.IsPublicPort(context.Connection.LocalPort) || perMin <= 0)
                        return RateLimitPartition.GetNoLimiter(ip);
                    return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = perMin,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
                });
                options.OnRejected = (ctx, _) =>
                {
                    Console.WriteLine($"[GUARD] rate-limit ip=" +
                        $"{Services.LogSafe.Address(Security.ClientIp.Key(ctx.HttpContext))}" +
                        $" -> 429 {ctx.HttpContext.Request.Path}");
                    return ValueTask.CompletedTask;
                };
            });
            services.AddMagicOnion(configureOptions =>
            {
                configureOptions.SerializerOptions = MessagePackSerializer.DefaultOptions.WithResolver(
                     CompositeResolver.Create(
                     UlidMessagePackResolver.Instance,
                     StandardResolver.Instance
                 ));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (context, next) =>
            {

                var port = context.Connection.LocalPort;
                ServerMetrics.HttpRequestStarted(port);
                try
                {
                    await next();
                }
                finally
                {
                    ServerMetrics.HttpRequestFinished(port);
                }
            });
            app.Use(async (context, next) =>
            {

                var path = context.Request.Path;
                if (!path.StartsWithSegments("/battle-status")
                    && !path.StartsWithSegments("/metrics")
                    && !path.StartsWithSegments("/gm/api/metrics")
                    && !path.StartsWithSegments("/IServiceStatusService"))
                    Console.WriteLine($"[REQ] {path}");
                await next();
            });
            app.UseMiddleware<Security.PublicGuard>();
            app.UseRouting();
            app.UseRateLimiter();
            app.UseEndpoints(endpoints =>
            {

                endpoints.MapMagicOnionService().RequireRateLimiting("public");
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/linkid=2086909");
                });

                Gm.GmModule.Map(endpoints);

                BattleStatusEndpoint.Map(endpoints);

                AuthLoginEndpoint.Map(endpoints);

                ClientReportEndpoint.Map(endpoints);

                MetricsEndpoint.Map(endpoints);
            });
        }
    }
}
