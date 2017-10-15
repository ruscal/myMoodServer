using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
    public static class LinqHelper
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            if (chunksize < 1) throw new ArgumentOutOfRangeException("chunksize", "Chunk size must be greater than or equal to 1");

            while (source.Any())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in source)
            {
                action(item, i);
                i++;
            }
        }

        public static IEnumerable<T> DefaultIfEmpty<T>(this IEnumerable<T> source, IEnumerable<T> defaultSequence)
        {
            return source.Any() ? source : defaultSequence;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T itemToFind)
        {
            var i = 0;
            foreach (var item in source)
            {
                if (object.ReferenceEquals(item, itemToFind) || object.Equals(item, itemToFind)) return i;
                i++;
            }
            return -1;
        }
    }
}
