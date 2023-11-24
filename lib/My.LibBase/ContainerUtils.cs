using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace My
{
    public static partial class Extension
    {
        public static T[] VariadicToArray<T>(params T[] items) {
            return items;
        }
    }
}
