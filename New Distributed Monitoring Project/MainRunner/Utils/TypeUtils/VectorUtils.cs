using System;
using System.Diagnostics;
using System.Linq;
using Accord.Math;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Utils.TypeUtils
{
    public static class VectorUtils
    {
        public static Matrix<double> AsMatrix(this Vector<double> @this)
        {
            var size = (int)Math.Sqrt(@this.Count);
            var matrix = Matrix<double>.Build.Sparse(size, size, (r, c) => @this[r * size + c]);
            return matrix;
        }

        public static Vector<double> SumVector(this Vector<double>[] @this)
        {
            var vecLength = @this[0].Count;
            var sumArray  = new double[vecLength];

            foreach (var vector in @this)
                for (int i = 0; i < vecLength; i++)
                    sumArray[i] += vector[i];

            return sumArray.ToVector();
        }

        public static int CountNonZero(this Vector<double> @this) => @this.Count(num => !num.AlmostEqual(0.0));


        public static Vector<double> AverageVector(this Vector<double>[] @this)
        {
            var sumVector = @this.SumVector();

            for (int i = 0; i < sumVector.Count; i++)
                sumVector[i] /= @this.Length;

            return sumVector;
        }

        public static (Vector<double> firstHalf, Vector<double> secondHalf) Halve(this Vector<double> @this)
        {
            var length     = @this.Count;
            var halfLength = length / 2;
            Debug.Assert(length % 2 == 0);
            Vector<double> firstHalf  = @this.Take(halfLength).ToVector();
            Vector<double> secondHalf = @this.Skip(halfLength).ToVector();
            return (firstHalf, secondHalf);
        }

        public static Vector<double> VConcat(this Vector<double> @this, Vector<double> other)
            => @this.Concat(other).ToVector();

        public static double DistanceL2(Vector<double> v1, Vector<double> v2) => (v1 - v2).L2Norm();

        public static Func<Vector<double>, double> DistL2FromVector(this Vector<double> @this) =>
            otherVec => DistanceL2(@this, otherVec);


        public static void AddInPlace(this Vector<double> @this, Vector<double> other)
        {
            for (int i = 0; i < @this.Count; i++)
                @this[i] += other[i];
        }

        public static Vector<double> CreateVector(int vectorCount, Func<int, double> generator) 
            => Vector<double>.Build.DenseOfEnumerable(Enumerable.Range(0, vectorCount).Select(generator));
    }
}
