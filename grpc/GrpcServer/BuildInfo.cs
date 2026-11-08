using System.Reflection;

namespace Byac.Services
{

    internal static class BuildInfo
    {
        public static string Version { get; } = Resolve();

        private static string Resolve()
        {
            var asm = typeof(BuildInfo).Assembly;
            var info = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (!string.IsNullOrWhiteSpace(info)) return info;
            return asm.GetName().Version?.ToString(3) ?? "0.0.0";
        }
    }
}
