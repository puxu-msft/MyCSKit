using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace My;

public static partial class Extension
{
    public static ArraySegment<byte>? ToArraySegment(this IEnumerable<JToken> jarray)
    {
        if (jarray == null) return null;
        var bytes = jarray
            .Select(token => token.Value<sbyte>())
            .Select(sb => unchecked((byte)(sb)))
            .ToArray();

        var padded = new byte[bytes.Length + 8];
        Array.Copy(bytes, 0, padded, 4, bytes.Length);
        return new ArraySegment<byte>(padded, 4, bytes.Length);
    }

    public static bool CanAsJsonInt(this Type type, bool acceptString = false)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        if (false
            || type == typeof(long)
            || type == typeof(ulong)
            || type == typeof(int)
            || type == typeof(uint)
            || type == typeof(short)
            || type == typeof(ushort)
            || type == typeof(byte)
            || type == typeof(sbyte)
            || type == typeof(System.Numerics.BigInteger)
            || false)
            return true;
        if (acceptString && (false
            || type == typeof(string)
            || type == typeof(char)
            || false))
            return true;
        return false;
    }
}
