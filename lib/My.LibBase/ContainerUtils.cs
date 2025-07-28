using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace My
{
    public static partial class ContainerUtils
    {
        public static T[] VariadicToArray<T>(params T[] items)
        {
            return items;
        }

        // return false to break the loop
        public delegate bool FnGenerator<T>(out T obj);

        // bond needs Stream, but Stream doesn't support to be converted from ReadOnlySpan, so uses ArraySegment here.
        public static IEnumerable<T> ToEnumerable<T>(this FnGenerator<T> fn)
        {
            while (true)
            {
                T obj;
                if (!fn.Invoke(out obj))
                    break;
                yield return obj;
            }
        }

        public static IAsyncEnumerator<T> EmptyAsyncEnumerable<T>() => EmptyAsyncEnumerator<T>.Instance;

        class EmptyAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            public static readonly EmptyAsyncEnumerator<T> Instance = new EmptyAsyncEnumerator<T>();
            public T Current => default!;
            public ValueTask DisposeAsync() => default;
            public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
            ValueTask IAsyncDisposable.DisposeAsync() => default;
            ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync() => new ValueTask<bool>(false);
        }
    }
}
