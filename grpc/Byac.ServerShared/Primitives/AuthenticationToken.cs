using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MessagePack;
using Byac.ServerShared.MessagePack.Formatters;

namespace Byac.ServerShared.Primitives
{

    [MessagePackObject]
    public readonly struct AuthenticationToken : IEquatable<AuthenticationToken>
    {
        [Key(0)]
        public readonly string _value;
        public AuthenticationToken(string value)
        {
            this._value = value;
        }

        bool IEquatable<AuthenticationToken>.Equals(AuthenticationToken other)
        {
            return string.Equals(_value, other._value, StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            return obj is AuthenticationToken other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value == null ? 0 : StringComparer.Ordinal.GetHashCode(_value);
        }
    }
}
