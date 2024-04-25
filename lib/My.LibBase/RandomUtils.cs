using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace My
{
    public static partial class RandomUtils
    {
        public static Random s_Random = new Random();

        public static byte[] GenByteArray(int size)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            var bytes = new byte[size];
            new Random().NextBytes(bytes);
            return bytes;
        }
    }
}
