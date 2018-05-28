using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Utils.TypeUtils
{
    public static class ArrayUtils
    {
        public static T[] ToArray<T>(this Tuple<T, T, T> @this) => new[] { @this.Item1, @this.Item2, @this.Item3 };
        public static T[] ToArray<T>(this Tuple<T, T> @this) => new[] { @this.Item1, @this.Item2 };

        public static T[] Copy<T>(this T[] @this) => @this.Clone() as T[];
        public static T[] Shuffle<T>(this T[] @this, Random rnd) => @this.OrderBy(_ => rnd.Next()).ToArray();

        public static T ChooseRandomly<T>(this T[] @this, Random rnd) => @this[rnd.Next(@this.Length)];

        public static Vector<double> ToVector(this double[] @this) => Vector<double>.Build.DenseOfArray(@this);

        public static T[] Init<T>(int arraySize, Func<int, T> createFunc)
        {
            return Enumerable.Range(0, arraySize).Select(createFunc).ToArray();
        }
    }
}
