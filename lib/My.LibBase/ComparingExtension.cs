using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public static partial class ComparingExtension
    {
        public static bool PrimitiveEquals<T>(this T a, T b)
        {
            return a.Equals(b);
        }

        public static bool NullableEquals<T>(this T? a, T? b)
            where T : struct
        {
            if (a.HasValue && b.HasValue)
                return a.Value.Equals(b.Value);
            return !a.HasValue && !b.HasValue;
        }

        public static bool NullableEquals<T>(this T? a, T? b)
            where T : class
        {
            if (object.ReferenceEquals(a, b))
                return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;
            return a.Equals(b);
        }

        public static bool NullablePreEquals<T>(this T? a, T? b)
            where T : struct
        {
            if (a.HasValue && b.HasValue)
                return true;
            return !a.HasValue && !b.HasValue;
        }

        public static bool NullablePreEquals<T>(this T? a, T? b)
            where T : class
        {
            if (object.ReferenceEquals(a, b))
                return true;
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;
            return true;
        }

        public static bool SequenceEquals(this ReadOnlySpan<byte> a, ReadOnlySpan<byte> b)
        {
            return a.SequenceEqual(b);
        }

        public static bool SequenceEquals(this IEnumerable<byte>? a, IEnumerable<byte>? b)
        {
            if (a == null || b == null)
                return a == b;
            return a.SequenceEqual(b);
        }

        public static bool ContainerEquals<T>(this IEnumerable<T> a, IEnumerable<T> b, Func<T, T, bool> comparer)
        {
            return Enumerable.Zip(a, b, (ae, be) => comparer(ae, be)).All(e => e);
        }

        // public static bool ContainerEquals<T>(this IEnumerable<T> a, IEnumerable<T> b)
        // {
        //     return ContainerEquals(a, b, (ae, be) => ValueEquals(ae, be));
        // }

        // public static bool ContainerEquals<T>(this ArraySegment<T> lhs, ArraySegment<T> rhs)
        // {
        //     if (lhs.Array == null && rhs.Array == null) return true;
        //     if (lhs.Array == null || rhs.Array == null) return false;
        //     if (lhs.Count != rhs.Count) return false;
        //     for (int i = 0; i < lhs.Count; ++i)
        //     {
        //         if (!Equals(lhs.Array[lhs.Offset + i], rhs.Array[rhs.Offset + i]))
        //             return false;
        //     }
        //     return true;
        // }
    }
}
