using System;
using System.IO;

namespace Byac.ServerShared
{

    public static class AndroidFileBridge
    {

        public static Func<string, byte[]?>? ReadBytesOverride;
        public static Func<string, string?>? ReadTextOverride;
        public static Action<string, string>? WriteTextOverride;

        public static byte[]? ReadBytes(string path)
        {
            if (ReadBytesOverride != null) return ReadBytesOverride(path);
            return File.Exists(path) ? File.ReadAllBytes(path) : null;
        }

        public static string? ReadText(string path)
        {
            if (ReadTextOverride != null) return ReadTextOverride(path);
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }

        public static void WriteText(string path, string contents)
        {
            if (WriteTextOverride != null)
            {
                WriteTextOverride(path, contents);
                return;
            }
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, contents);
            File.Move(tmp, path, true);
        }
    }
}
