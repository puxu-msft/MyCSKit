using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace My
{
    public static partial class Extension
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            source.All(x =>
            {
                action.Invoke(x);
                return true;
            });
        }

        public static IEnumerable<Tuple<int, T>> ZipIndex<T>(this IEnumerable<T> source)
        {
            var i = 0;
            foreach (var item in source)
            {
                yield return Tuple.Create(i++, item);
            }
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("Batch size: " + size);

            T[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new T[size];

                bucket[count++] = item;

                if (count != size)
                    continue;

                yield return bucket.Select(x => x);

                bucket = null;
                count = 0;
            }

            // return the last bucket with all remaining elements
            if (bucket != null && count > 0)
            {
                Array.Resize(ref bucket, count);
                yield return bucket.Select(x => x);
            }
        }

        public static IEnumerable<T> RecursiveSelect<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            var stack = new Stack<IEnumerator<T>>();
            var enumerator = source.GetEnumerator();

            try {
                while (true) {
                    if (enumerator.MoveNext()) {
                        var element = enumerator.Current;
                        yield return element;

                        stack.Push(enumerator);
                        enumerator = selector(element).GetEnumerator();
                    }
                    else if (stack.Count > 0) {
                        enumerator.Dispose();
                        enumerator = stack.Pop();
                    }
                    else {
                        yield break;
                    }
                }
            }
            finally {
                enumerator.Dispose();

                // Clean up in case of an exception
                while (stack.Count > 0) {
                    enumerator = stack.Pop();
                    enumerator.Dispose();
                }
            }
        }
    }
}
