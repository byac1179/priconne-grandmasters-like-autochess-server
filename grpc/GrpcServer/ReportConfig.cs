using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Byac.Server
{

    public static class ReportConfig
    {
        public const int FallbackPercent = 10;

        private static int _percent = -1;
        private static string _source = "unset";

        private static string OverridePath =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report-rate.json");

        public static int Percent
        {
            get
            {
                if (_percent < 0) Load();
                return _percent;
            }
        }

        public static string Source
        {
            get
            {
                if (_percent < 0) Load();
                return _source;
            }
        }

        public static void Load()
        {
            var v = EnvDefault(out var src);
            try
            {
                if (File.Exists(OverridePath))
                {
                    using var doc = JsonDocument.Parse(File.ReadAllText(OverridePath));
                    if (doc.RootElement.TryGetProperty("percent", out var p) && p.TryGetInt32(out var pv))
                    {
                        v = pv;
                        src = "file";
                    }
                }
            }
            catch (Exception e)
            {

                Console.WriteLine($"[REPORT] rate file ignored: {e.GetType().Name}: {e.Message}");
            }
            _percent = Clamp(v);
            _source = src;
        }

        public static int Set(int percent)
        {
            var v = Clamp(percent);
            try
            {
                File.WriteAllText(OverridePath, JsonSerializer.Serialize(
                    new Dictionary<string, object>
                    {
                        ["percent"] = v,
                        ["_note"] = "client battle-report sampling knob — delete this file to fall back to BYAC_REPORT_RATE",
                        ["_changed_at"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        ["_changed_by"] = "gm-tool",
                    }));
                _source = "file";
            }
            catch (Exception e)
            {
                Console.WriteLine($"[REPORT] rate file write failed (in-memory only): {e.GetType().Name}: {e.Message}");
                _source = "memory";
            }
            _percent = v;
            return v;
        }

        public static int Reset()
        {
            try { if (File.Exists(OverridePath)) File.Delete(OverridePath); }
            catch (Exception e)
            {
                Console.WriteLine($"[REPORT] rate file delete failed: {e.GetType().Name}: {e.Message}");
            }
            _percent = Clamp(EnvDefault(out _source));
            return _percent;
        }

        private static int EnvDefault(out string source)
        {
            if (int.TryParse(Environment.GetEnvironmentVariable("BYAC_REPORT_RATE"), out var v))
            {
                source = "env";
                return v;
            }
            source = "default";
            return FallbackPercent;
        }

        private static int Clamp(int v) => v < 0 ? 0 : (v > 100 ? 100 : v);
    }
}
