using System;
using System.Net;
using System.Net.Sockets;

namespace Byac.Services
{

    public static class LogSafe
    {
        public static string Header(string key, string value)
        {
            if (!key.Equals("authorization", StringComparison.OrdinalIgnoreCase)) return value;
            return Secret(value);
        }

        public static string Secret(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "<empty>";
            if (value.Length <= 12) return $"***len={value.Length}";
            return $"{value.Substring(0, 8)}…len={value.Length}";
        }

        public static string Address(IPAddress? address)
        {
            if (address == null) return "unknown";
            if (address.IsIPv4MappedToIPv6) address = address.MapToIPv4();
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                var parts = address.ToString().Split('.');
                return parts.Length == 4 ? $"{parts[0]}.{parts[1]}.x.x" : "ipv4?";
            }
            var v6 = address.ToString().Split(':');
            return v6.Length >= 2 ? $"{v6[0]}:{v6[1]}:…" : "ipv6?";
        }

        public static string Address(string? address)
        {
            if (string.IsNullOrEmpty(address)) return "unknown";
            return IPAddress.TryParse(address, out var ip) ? Address(ip) : "?";
        }
    }
}
