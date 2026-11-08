using System;
using System.IO;
using System.Text.Json;

namespace Byac.Services.Battle
{

    public static class VerifyMode
    {
        private static readonly string ConfigPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "verify-mode.json");

        public static int PrepareSecondsOverride { get; private set; } = 0;

        public static float DamageMultiplier { get; private set; } = 1.0f;

        public static bool SkipBattleAnimation { get; private set; } = false;

        public static bool InstantReady { get; private set; } = false;

        public static int MaxRounds { get; private set; } = 0;

        public static bool IsActive =>
            PrepareSecondsOverride > 0 ||
            Math.Abs(DamageMultiplier - 1.0f) > 0.001f ||
            SkipBattleAnimation ||
            InstantReady ||
            MaxRounds > 0;

        public static void Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                {
                    PrepareSecondsOverride = 0;
                    DamageMultiplier = 1.0f;
                    SkipBattleAnimation = false;
                    InstantReady = false;
                    MaxRounds = 0;
                    return;
                }

                var json = File.ReadAllText(ConfigPath);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                PrepareSecondsOverride = root.TryGetProperty("prepareSecondsOverride", out var ps)
                    ? ps.GetInt32() : 0;
                DamageMultiplier = root.TryGetProperty("damageMultiplier", out var dm)
                    ? (float)dm.GetDouble() : 1.0f;
                SkipBattleAnimation = root.TryGetProperty("skipBattleAnimation", out var sb)
                    && sb.GetBoolean();
                InstantReady = root.TryGetProperty("instantReady", out var ir)
                    && ir.GetBoolean();
                MaxRounds = root.TryGetProperty("maxRounds", out var mr)
                    ? mr.GetInt32() : 0;

                var purpose = root.TryGetProperty("_purpose", out var p) ? p.GetString() : "";
                Console.WriteLine($"[VERIFY-MODE] loaded: prepOverride={PrepareSecondsOverride}s " +
                    $"dmgMul={DamageMultiplier} skipAnim={SkipBattleAnimation} instantReady={InstantReady} maxRounds={MaxRounds} purpose={purpose}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[VERIFY-MODE] load failed: {ex.Message} — using defaults");
                PrepareSecondsOverride = 0;
                DamageMultiplier = 1.0f;
                SkipBattleAnimation = false;
                InstantReady = false;
                MaxRounds = 0;
            }
        }

        public static int ApplyPrepareSeconds(int original)
        {
            if (PrepareSecondsOverride > 0)
            {
                Console.WriteLine($"[VERIFY-MODE] prepareSeconds: {original} -> {PrepareSecondsOverride}");
                return PrepareSecondsOverride;
            }
            return original;
        }

        public static int ApplyDamage(int original)
        {
            if (Math.Abs(DamageMultiplier - 1.0f) > 0.001f)
            {
                return (int)(original * DamageMultiplier);
            }
            return original;
        }
    }
}
