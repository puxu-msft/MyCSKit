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
            return hex.SplitInParts((s, i) => {
                if (i + 1 >= s.Length) throw new ArgumentException("invalid number of hex chars");
                int oi = i;
                while ("-: ".Contains(s[i])) i++;
                return (i - oi + 2, s.Substring(i, 2));
            }).Select(e => Convert.ToByte(e, 16)).ToArray();
        }

        public static IEnumerable<String> SplitInParts(this string s, Func<string, int, (int, string)> fn)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            for (var i = 0; i < s.Length;)
            {
                var (move, part) = fn(s, i);
                if (move <= 0)
                    throw new InvalidOperationException("Returned move must be positive");

                i += move;
                yield return part;
            }
        }
    }
}
