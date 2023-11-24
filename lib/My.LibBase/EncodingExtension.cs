using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public static partial class Extension
    {
        public static string? ToHex(this ArraySegment<byte> bytes)
        {
            // if (bytes.Array == null) return null;
            #if (NET5_0_OR_GREATER)
                // BitConverter.ToString returns likes "00-FF"
                // return BitConverter.ToString(bytes.Array, bytes.Offset, bytes.Count);
                return Convert.ToHexString(bytes.Array, bytes.Offset, bytes.Count);
            #else
                return string.Concat(bytes.Select(b => b.ToString("X2")));
            #endif
        }

        public static byte[] FromHex(this string hex)
        {
            // use Convert.FromHexString if dotnet core >= 5
            // var b = Convert.FromHexString(hash);
            return hex.SplitInParts(2).Select(e => Convert.ToByte(e, 16)).ToArray();
        }

        public static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }
}
