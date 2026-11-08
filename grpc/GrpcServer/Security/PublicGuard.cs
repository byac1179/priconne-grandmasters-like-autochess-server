using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Byac.Services;

namespace Byac.Server.Security
{

    public class PublicGuard
    {
        private readonly RequestDelegate _next;

        public PublicGuard(RequestDelegate next)
        {
            _next = next;
        }

        private static readonly ConcurrentDictionary<string, int> PerIp = new();

        public async Task InvokeAsync(HttpContext context)
        {
            if (!PublicGuardOptions.IsPublicPort(context.Connection.LocalPort)
                || PublicGuardOptions.MaxStreamsPerIp <= 0)
            {
                await _next(context);
                return;
            }

            var key = ClientIp.Key(context);
            var active = PerIp.AddOrUpdate(key, 1, (_, v) => v + 1);
            try
            {
                if (active > PublicGuardOptions.MaxStreamsPerIp)
                {
                    Console.WriteLine($"[GUARD] ip={LogSafe.Address(key)} streams={active}" +
                        $">{PublicGuardOptions.MaxStreamsPerIp} -> 429 {context.Request.Path}");
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    return;
                }
                await _next(context);
            }
            finally
            {
                if (PerIp.AddOrUpdate(key, 0, (_, v) => v - 1) == 0)
                    ((ICollection<KeyValuePair<string, int>>)PerIp)
                        .Remove(new KeyValuePair<string, int>(key, 0));
            }
        }
    }

    public static class ClientIp
    {
        public static string Key(HttpContext context)
        {
            var ip = Of(context);
            return ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4().ToString() : ip.ToString();
        }

        private static IPAddress Of(HttpContext context)
        {
            if (PublicGuardOptions.TrustProxy)
            {
                var xff = context.Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrWhiteSpace(xff))
                {
                    var parts = xff.Split(',');
                    for (var i = parts.Length - 1; i >= 0; i--)
                        if (IPAddress.TryParse(parts[i].Trim(), out var ip))
                            return ip;
                }
            }
            return context.Connection.RemoteIpAddress ?? IPAddress.None;
        }
    }

    public static class PublicGuardOptions
    {

        public static int H2cPort => ReadInt("BYAC_H2C_PORT", 18081);

        public static int TlsPort => ReadInt("BYAC_TLS_PORT", 0);

        public static bool IsPublicPort(int localPort) =>
            (H2cPort > 0 && localPort == H2cPort) || (TlsPort > 0 && localPort == TlsPort);

        public static int MaxStreamsPerIp => ReadInt("BYAC_MAX_STREAMS_PER_IP", 64);

        public static int MaxConns => ReadInt("BYAC_MAX_CONNS", 1024);

        public static int RatePerMin => ReadInt("BYAC_RATE_PER_MIN", 600);

        public static bool TrustProxy => ReadInt("BYAC_TRUST_PROXY", 1) == 1;

        public static bool GzipResponses => ReadInt("BYAC_GRPC_GZIP", 1) == 1;

        private static int ReadInt(string name, int fallback) =>
            int.TryParse(Environment.GetEnvironmentVariable(name), out var v) ? v : fallback;
    }
}
