using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Byac.Services;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace Byac.Server
{
    public class Program
    {

        internal static X509Certificate2 LoadServerCertificate(string certPath, string keyPath)
        {
            var cert = X509Certificate2.CreateFromPemFile(certPath, keyPath);
            if (!OperatingSystem.IsWindows()) return cert;
            return new X509Certificate2(cert.Export(X509ContentType.Pfx));
        }

        internal static class RuntimeTlsCertificate
        {
            private static readonly object Lock = new();
            private static X509Certificate2? _cert;
            private static string _certPath = "", _keyPath = "";
            private static DateTime _certStamp, _keyStamp;
            private static DateTime _lastWarnUtc;

            public static X509Certificate2? Get(string certPath, string keyPath)
            {
                lock (Lock)
                {
                    try
                    {
                        var cm = File.GetLastWriteTimeUtc(certPath);
                        var km = File.GetLastWriteTimeUtc(keyPath);
                        if (_cert != null && certPath == _certPath && keyPath == _keyPath
                            && cm == _certStamp && km == _keyStamp)
                            return _cert;
                        var cert = LoadServerCertificate(certPath, keyPath);
                        if (!cert.HasPrivateKey)
                            throw new InvalidOperationException("certificate has no private key (or key does not match cert)");
                        _cert = cert;
                        _certPath = certPath; _keyPath = keyPath;
                        _certStamp = cm; _keyStamp = km;
                        Console.WriteLine($"[TLS] cert loaded subject={cert.Subject} " +
                            $"thumbprint={cert.Thumbprint} notAfter={cert.GetExpirationDateString()}");
                        return _cert;
                    }
                    catch (Exception ex)
                    {
                        _cert = null;
                        if ((DateTime.UtcNow - _lastWarnUtc).TotalSeconds >= 60)
                        {
                            _lastWarnUtc = DateTime.UtcNow;
                            Console.WriteLine($"[SECURITY] TLS certificate unavailable cert='{certPath}' key='{keyPath}': {ex.Message}" +
                                "-- TLS handshakes will keep failing; the next handshake picks up the files once in place.");
                        }
                        return null;
                    }
                }
            }
        }

        public static void Main(string[] args)
        {            try
            {
                var logPath = Environment.GetEnvironmentVariable("GSERVER_LOG") ?? @"glog.txt";

                if (!string.IsNullOrEmpty(logPath) && logPath != "-")
                {
                    var sw = new RotatingTextWriter(logPath);
                    Console.SetOut(sw);
                    Console.SetError(sw);
                }
                Console.WriteLine($"=== Byac.Server v{BuildInfo.Version} starting {DateTime.Now:yyyy-MM-dd HH:mm:ss} pid={Environment.ProcessId} ===");
            }
            catch (Exception ex)
            {
                File.AppendAllText(@"glog.txt", "log init failed: " + ex.Message + Environment.NewLine);
            }

            Byac.Services.Battle.VerifyMode.Load();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel(options =>
                        {
                            var port = int.Parse(Environment.GetEnvironmentVariable("GSERVER_PORT") ?? "9444");
                            var httpsBind = Environment.GetEnvironmentVariable("BYAC_HTTPS_BIND");
                            var httpsAddr = string.Equals(httpsBind, "loopback", StringComparison.OrdinalIgnoreCase)
                                ? IPAddress.Loopback : IPAddress.Any;

                            if (port > 0)
                            {
                                var workDir = Environment.GetEnvironmentVariable("GSERVER_WORK") ?? AppContext.BaseDirectory;
                                var pfxPath = Environment.GetEnvironmentVariable("GSERVER_PFX") ?? Path.Combine(workDir, "server.pfx");
                                X509Certificate2 cert;
                                if (File.Exists(pfxPath))
                                {
                                    cert = new X509Certificate2(pfxPath);
                                }
                                else
                                {
                                    var certPath = Environment.GetEnvironmentVariable("GSERVER_CERT") ?? Path.Combine(workDir, "server_chain.pem");
                                    var keyPath = Environment.GetEnvironmentVariable("GSERVER_KEY") ?? Path.Combine(workDir, "server_key.pem");
                                    cert = LoadServerCertificate(certPath, keyPath);
                                }

                                options.Listen(httpsAddr, port, listenOptions =>
                                {
                                    listenOptions.Protocols = HttpProtocols.Http2;
                                    listenOptions.UseHttps(cert);
                                });
                            }

                            options.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(5);
                            options.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromSeconds(15);
                            var maxConns = Security.PublicGuardOptions.MaxConns;
                            if (maxConns > 0) options.Limits.MaxConcurrentConnections = maxConns;

                            var h2cPort = Security.PublicGuardOptions.H2cPort;
                            if (h2cPort > 0)
                            {
                                options.Listen(IPAddress.Any, h2cPort, listenOptions =>
                                {
                                    listenOptions.Protocols = HttpProtocols.Http2;
                                });
                            }

                            var tlsPort = Security.PublicGuardOptions.TlsPort;
                            if (tlsPort > 0)
                            {
                                if (tlsPort == h2cPort)
                                    throw new InvalidOperationException(
                                        $"BYAC_TLS_PORT and BYAC_H2C_PORT are both {tlsPort}: one port cannot serve both plaintext and TLS." +
                                        "Change the port, or set BYAC_H2C_PORT=0 to disable plaintext.");

                                var tlsCertPath = Environment.GetEnvironmentVariable("BYAC_TLS_CERT") ?? "";
                                var tlsKeyPath = Environment.GetEnvironmentVariable("BYAC_TLS_KEY") ?? "";
                                var tlsBind = Environment.GetEnvironmentVariable("BYAC_TLS_BIND");
                                var tlsAddr = string.Equals(tlsBind, "loopback", StringComparison.OrdinalIgnoreCase)
                                    ? IPAddress.Loopback : IPAddress.Any;
                                options.Listen(tlsAddr, tlsPort, listenOptions =>
                                {

                                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                                    listenOptions.UseHttps(o => o.ServerCertificateSelector = (_, _) =>
                                        RuntimeTlsCertificate.Get(tlsCertPath, tlsKeyPath));
                                });
                                Console.WriteLine($"[TLS] {tlsAddr}:{tlsPort} runtime-loaded certificate cert='{tlsCertPath}' key='{tlsKeyPath}' (auto-applies when files appear/renew)");
                            }

                            if (h2cPort <= 0 && tlsPort <= 0 && httpsAddr.Equals(IPAddress.Loopback))
                                throw new InvalidOperationException(
                                    "no external listener: BYAC_H2C_PORT=0 and BYAC_TLS_PORT=0," +
                                    "and 9444 is bound to loopback (BYAC_HTTPS_BIND=loopback)." +
                                    "Set BYAC_TLS_PORT to the certificate port (e.g. 18081), or explicitly enable BYAC_H2C_PORT.");

                            var gmPort = int.Parse(Environment.GetEnvironmentVariable("BYAC_GM_PORT") ?? "9445");
                            if (gmPort > 0)
                            {

                                var gmBind = Environment.GetEnvironmentVariable("BYAC_GM_BIND");
                                var gmAddr = string.Equals(gmBind, "any", StringComparison.OrdinalIgnoreCase)
                                    ? IPAddress.Any : IPAddress.Loopback;
                                options.Listen(gmAddr, gmPort, listenOptions =>
                                {
                                    listenOptions.Protocols = HttpProtocols.Http1;
                                });
                            }

                            var metricsPort = MetricsEndpoint.Port;
                            if (metricsPort > 0)
                            {
                                if (metricsPort == gmPort)
                                    throw new InvalidOperationException(
                                        $"BYAC_METRICS_PORT and BYAC_GM_PORT are both {metricsPort}: pick a different port.");
                                var metricsBind = Environment.GetEnvironmentVariable("BYAC_METRICS_BIND");
                                var metricsAddr = string.Equals(metricsBind, "any", StringComparison.OrdinalIgnoreCase)
                                    ? IPAddress.Any : IPAddress.Loopback;
                                options.Listen(metricsAddr, metricsPort, listenOptions =>
                                {
                                    listenOptions.Protocols = HttpProtocols.Http1;
                                });
                            }

                            options.ConfigureEndpointDefaults(endpointOptions =>
                            {
                                endpointOptions.Protocols = HttpProtocols.Http2;
                            });
                        })
                        .UseStartup<Startup>();
                });
    }
}
