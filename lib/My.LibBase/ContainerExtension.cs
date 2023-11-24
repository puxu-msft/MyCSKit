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

        public static IEnumerable<KeyValuePair<int, T>> ZipIndex<T>(this IEnumerable<T> source)
        {
            var i = 0;
            foreach (var item in source)
            {
                yield return new KeyValuePair<int, T>(i++, item);
            }
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
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
    }
}
