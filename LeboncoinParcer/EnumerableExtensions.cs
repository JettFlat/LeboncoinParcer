using System.Collections.Generic;
using System.Linq;

namespace LeboncoinParser
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> enumerable, int size)
        {
            var array = enumerable.ToArray();
            for (var i = 0; i < (float)array.Count() / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}