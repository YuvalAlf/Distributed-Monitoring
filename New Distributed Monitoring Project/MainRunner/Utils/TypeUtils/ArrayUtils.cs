using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Utils.TypeUtils
{
    public static class ArrayUtils
    {
        public static T[] Copy<T>(this T[] @this) => @this.Clone() as T[];

        public static T[] ShuffleInPlace<T>(this T[] @this, Random rnd)
        {
            for (int i = 2; i < @this.Length; i++)
            {
                var other = rnd.Next(i + 1);
                var temp  = @this[i];
                @this[i]     = @this[other];
                @this[other] = temp;
            }

            return @this;
        }

        public static T ChooseRandomly<T>(this T[] @this, Random rnd) => @this[rnd.Next(@this.Length)];

        public static Vector<double> ToVector(this double[] @this) => Vector<double>.Build.DenseOfArray(@this);

        public static T[] Init<T>(int arraySize, Func<int, T> createFunc) =>
            Enumerable.Range(0, arraySize).Select(createFunc).ToArray();
    }
}
