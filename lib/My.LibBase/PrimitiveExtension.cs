using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public static partial class Extension
    {
        public static bool IsAlmostEqual(this double a, double b)
        {
            return Math.Abs(a - b) < 0.00001;
        }

        public static UInt64 ReverseEndianness(this UInt64 x)
        {
            // https://learn.microsoft.com/en-us/dotnet/standard/frameworks#preprocessor-symbols
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version
            #if (NET5_0_OR_GREATER)
                return System.Buffers.Binary.BinaryPrimitives.ReverseEndianness(x);
            #elif (NET48_OR_GREATER || NETSTANDARD1_0_OR_GREATER)
                // digit separator "_" is supported since C# 7.0
                return 0
                    | ((x & 0x0000_0000_0000_00fful) << 56)
                    | ((x & 0x0000_0000_0000_ff00ul) << 40)
                    | ((x & 0x0000_0000_00ff_0000ul) << 24)
                    | ((x & 0x0000_0000_ff00_0000ul) << 8)
                    | ((x & 0x0000_00ff_0000_0000ul) >> 8)
                    | ((x & 0x0000_ff00_0000_0000ul) >> 24)
                    | ((x & 0x00ff_0000_0000_0000ul) >> 40)
                    | ((x & 0xff00_0000_0000_0000ul) >> 56);
            #else
                return 0
                    | ((x & 0x00000000000000fful) << 56)
                    | ((x & 0x000000000000ff00ul) << 40)
                    | ((x & 0x0000000000ff0000ul) << 24)
                    | ((x & 0x00000000ff000000ul) << 8)
                    | ((x & 0x000000ff00000000ul) >> 8)
                    | ((x & 0x0000ff0000000000ul) >> 24)
                    | ((x & 0x00ff000000000000ul) >> 40)
                    | ((x & 0xff00000000000000ul) >> 56);
            #endif
        }
    }
}
