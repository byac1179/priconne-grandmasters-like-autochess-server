using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Byac.Services
{

    internal static class AccountDataCrypto
    {
        private const string Magic = "BYAC-ENC-V1\n";
        private const int NonceSize = 12;
        private const int TagSize = 16;
        private const string EnvKey = "BYAC_ENC_KEY";
        private const string EnvDataDir = "BYAC_DATA_DIR";

        private static readonly byte[] Key = ResolveKey();

        private static byte[] ResolveKey()
        {
            var env = Environment.GetEnvironmentVariable(EnvKey);
            if (!string.IsNullOrWhiteSpace(env))
            {
                var fromEnv = Convert.FromBase64String(env.Trim());
                if (fromEnv.Length != 32)
                    throw new InvalidOperationException($"{EnvKey} must be base64 of 32 bytes (got {fromEnv.Length})");
                return fromEnv;
            }

            var dataDir = Environment.GetEnvironmentVariable(EnvDataDir);
            if (string.IsNullOrEmpty(dataDir)) dataDir = Path.Combine(Environment.CurrentDirectory, "data");
            var keyPath = Path.Combine(dataDir, "enc.key");
            if (File.Exists(keyPath))
            {
                var stored = Convert.FromBase64String(File.ReadAllText(keyPath).Trim());
                if (stored.Length != 32)
                    throw new InvalidOperationException($"invalid key file {keyPath}: expected base64 of 32 bytes");
                return stored;
            }

            var generated = new byte[32];
            RandomNumberGenerator.Fill(generated);
            Directory.CreateDirectory(dataDir);
            File.WriteAllText(keyPath, Convert.ToBase64String(generated));
            Console.WriteLine($"[AccountDataCrypto] generated key file {keyPath}; set {EnvKey} to supply an explicit key");
            return generated;
        }

        public static string Seal(string plainText)
        {
            var plain = Encoding.UTF8.GetBytes(plainText);
            var nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce);
            var cipher = new byte[plain.Length];
            var tag = new byte[TagSize];
            try
            {
                using var aes = new AesGcm(Key, TagSize);
                aes.Encrypt(nonce, plain, cipher, tag);
            }
            catch (PlatformNotSupportedException)
            {
                return plainText;
            }
            var blob = new byte[NonceSize + TagSize + cipher.Length];
            Buffer.BlockCopy(nonce, 0, blob, 0, NonceSize);
            Buffer.BlockCopy(tag, 0, blob, NonceSize, TagSize);
            Buffer.BlockCopy(cipher, 0, blob, NonceSize + TagSize, cipher.Length);
            return Magic + Convert.ToBase64String(blob);
        }

        public static bool TryUnseal(string storedText, out string plainText, out string error)
        {
            plainText = storedText;
            error = "";
            if (!storedText.StartsWith(Magic, StringComparison.Ordinal)) return true;
            try
            {
                var blob = Convert.FromBase64String(storedText.Substring(Magic.Length).Trim());
                if (blob.Length < NonceSize + TagSize)
                {
                    plainText = "";
                    error = $"blob too short ({blob.Length})";
                    return false;
                }
                var nonce = new byte[NonceSize];
                var tag = new byte[TagSize];
                var cipher = new byte[blob.Length - NonceSize - TagSize];
                Buffer.BlockCopy(blob, 0, nonce, 0, NonceSize);
                Buffer.BlockCopy(blob, NonceSize, tag, 0, TagSize);
                Buffer.BlockCopy(blob, NonceSize + TagSize, cipher, 0, cipher.Length);
                var plain = new byte[cipher.Length];
                using (var aes = new AesGcm(Key, TagSize))
                    aes.Decrypt(nonce, cipher, tag, plain);
                plainText = Encoding.UTF8.GetString(plain);
                return true;
            }
            catch (Exception ex)
            {
                plainText = "";
                error = $"{ex.GetType().Name}: {ex.Message}";
                return false;
            }
        }
    }
}
