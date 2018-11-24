using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Utils.TypeUtils
{
    public static class EnumerableUtils
    {
        public static Vector<double> ToVector(this IEnumerable<double> @this) =>
            Vector<double>.Build.SparseOfEnumerable(@this);

        public static S[] Map<T, S>(this IEnumerable<T> @this, Func<T, S> map) => @this.Select(map).ToArray();

        public static (T1[], T2[]) UnZip<T1, T2>(this IEnumerable<ValueTuple<T1, T2>> @this)
        {
            var t1 = new List<T1>();
            var t2 = new List<T2>();
            foreach (var tuple in @this)
            {
                t1.Add(tuple.Item1);
                t2.Add(tuple.Item2);
            }

            return (t1.ToArray(), t2.ToArray());
        }
        public static (T1[], T2[], T3[]) UnZip<T1, T2, T3>(this IEnumerable<ValueTuple<T1, T2, T3>> @this)
        {
            var t1 = new List<T1>();
            var t2 = new List<T2>();
            var t3 = new List<T3>();
            foreach (var tuple in @this)
            {
                t1.Add(tuple.Item1);
                t2.Add(tuple.Item2);
                t3.Add(tuple.Item3);
            }

            return (t1.ToArray(), t2.ToArray(), t3.ToArray());
        }

        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }

        public static HashSet<int> IndicesWhere<T>(this IEnumerable<T> @this, Predicate<T> predicate)
        {
            HashSet<int> set   = new HashSet<int>();
            int          index = 0;
            foreach (var item in @this)
            {
                if (predicate(item))
                    set.Add(index);
                index++;
            }

            return set;
        }
    }
}
