using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public static class ValueTupleUtils
    {
        public static T[] ToArray<T>(this Tuple<T, T, T> @this) => new[] {@this.Item1, @this.Item2, @this.Item3};
        public static T[] ToArray<T>(this Tuple<T, T>    @this) => new[] {@this.Item1, @this.Item2};

        public static (T1, T2, T3) AddLast<T1, T2, T3>(this (T1, T2) tuple, T3 t3)
        {
            var (t1, t2) = tuple;
            return (t1, t2, t3);
        }
    }
}