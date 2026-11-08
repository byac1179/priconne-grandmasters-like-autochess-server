using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Byac.ServerShared.Hub;

namespace Byac.Services.Battle
{
    public class FrameDumpProxy : DispatchProxy
    {
        internal IBattleHubReceiver? Inner;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod == null) return null;
            try
            {
                FrameDump.Record(targetMethod.Name, args);
            }
            catch
            {
            }
            return targetMethod.Invoke(Inner, args);
        }
    }

    public static class FrameDump
    {
        private static readonly object Sync = new object();
        private static string? _path;
        private static bool _initialized;

        public static bool Enabled
        {
            get
            {
                EnsureInit();
                return _path != null;
            }
        }

        private static void EnsureInit()
        {
            if (_initialized) return;
            lock (Sync)
            {
                if (_initialized) return;
                var dir = Environment.GetEnvironmentVariable("BYAC_FRAME_DUMP_DIR");
                if (!string.IsNullOrWhiteSpace(dir))
                {
                    try
                    {
                        Directory.CreateDirectory(dir);
                        _path = Path.Combine(dir,
                            $"frames-{DateTime.Now:yyyyMMdd-HHmmss}-{Environment.ProcessId}.jsonl");
                    }
                    catch
                    {
                        _path = null;
                    }
                }
                _initialized = true;
            }
        }

        public static IBattleHubReceiver Wrap(IBattleHubReceiver inner)
        {
            var proxy = DispatchProxy.Create<IBattleHubReceiver, FrameDumpProxy>();
            ((FrameDumpProxy)(object)proxy).Inner = inner;
            return proxy;
        }

        public static void Record(string method, object?[]? args)
        {
            if (!Enabled || _path == null) return;
            var sb = new StringBuilder(256);
            sb.Append("{\"ts\":\"").Append(DateTime.Now.ToString("O"))
              .Append("\",\"method\":").Append(JsonSerializer.Serialize(method))
              .Append(",\"args\":[");
            if (args != null)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(SafeJson(args[i]));
                }
            }
            sb.Append("]}");
            var line = sb.ToString();
            lock (Sync)
            {
                try
                {
                    File.AppendAllText(_path, line + "\n", Encoding.UTF8);
                }
                catch
                {
                }
            }
        }

        private static string SafeJson(object? value)
        {
            if (value == null) return "null";
            try
            {

                if (value is Array arr && value.GetType().IsArray)
                {
                    var sb2 = new StringBuilder(256);
                    sb2.Append('[');
                    for (var i = 0; i < arr.Length; i++)
                    {
                        if (i > 0) sb2.Append(',');
                        var el = arr.GetValue(i);
                        sb2.Append(el == null ? "null" : SafeJson(el));
                    }
                    sb2.Append(']');
                    return sb2.ToString();
                }
                return JsonSerializer.Serialize(value, value.GetType());
            }
            catch
            {
                try
                {
                    return JsonSerializer.Serialize(value.ToString());
                }
                catch
                {
                    return "\"<unserializable>\"";
                }
            }
        }
    }
}
