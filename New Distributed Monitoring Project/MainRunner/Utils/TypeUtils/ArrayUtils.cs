using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Utils.SparseTypes;

namespace Utils.TypeUtils
{
    public static class ArrayUtils
    {
        public static T[] Copy<T>(this T[] @this) => @this.Clone() as T[];

        public static T[] ShuffleInPlace<T>(this T[] @this, Random rnd)
        {
            for (int i = 1; i < @this.Length; i++)
            {
                var other = rnd.Next(i + 1);
                var temp  = @this[i];
                @this[i]     = @this[other];
                @this[other] = temp;
            }

            return @this;
        }

        public static Vector ToVector(this double[] @this)
        {
            var vector = new Vector();
            for (int i = 0; i < @this.Length; i++)
                vector[i] = @this[i];
            return vector;
        }

        public static T[] Init<T>(int arraySize, Func<int, T> createFunc) =>
            Enumerable.Range(0, arraySize).Select(createFunc).ToArray();
    }
}
