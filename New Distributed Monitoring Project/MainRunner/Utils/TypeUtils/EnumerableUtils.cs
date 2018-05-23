using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Utils.TypeUtils
{
    public static class EnumerableUtils
    {
        public static Vector<double> ToVector(this IEnumerable<double> @this) => Vector<double>.Build.DenseOfEnumerable(@this);

        public static S[] Map<T, S>(this IEnumerable<T> @this, Func<T, S> map) => @this.Select(map).ToArray();

        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        public static HashSet<int> IndicesWhere<T>(this IEnumerable<T> @this, Predicate<T> predicate)
        {
            HashSet<int> set = new HashSet<int>();
            int index = 0;
            foreach (var item in @this)
            {
                if (predicate(item))
                    set.Add(index);
                index++;
            }
            return set;
        }

        public static IEnumerable<T[]> Inverse<T>(this IEnumerable<IEnumerable<T>> @this)
        {
            var enumerators = @this.Select(list => list.GetEnumerator()).ToArray();
            while (true)
            {
                var array = new T[enumerators.Length];
                for (int i = 0; i < enumerators.Length; i++)
                {
                    var enumerator = enumerators[i];
                    if (enumerator.MoveNext())
                        array[i] = enumerator.Current;
                    else
                        yield break;
                }

                yield return array;
            }
        }
    }
}
