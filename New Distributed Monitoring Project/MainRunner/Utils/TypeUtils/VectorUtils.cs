﻿using System;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Utils.TypeUtils
{
    public static class VectorUtils
    {
        public static Vector<double> AverageVector(this Vector<double>[] @this)
        {
            var vecLength = @this[0].Count;
            var sumArray = new double[vecLength];
            foreach (var vector in @this)
                for (int i = 0; i < vecLength; i++)
                    sumArray[i] += vector[i];

            for (int i = 0; i < vecLength; i++)
                sumArray[i] /= @this.Length;

            return sumArray.ToVector();
        }

        public static (Vector<double> firstHalf, Vector<double> secondHalf) Halve(this Vector<double> @this)
        {
            var length = @this.Count;
            var halfLength = length / 2;
            Debug.Assert(length % 2 == 0);
            Vector<double> firstHalf = @this.Take(halfLength).ToVector();
            Vector<double> secondHalf = @this.Skip(halfLength).ToVector();
            return (firstHalf, secondHalf);
        }

        public static Vector<double> VConcat(this Vector<double> @this, Vector<double> other) 
            => @this.Concat(other).ToVector();

        public static double DistanceL2(Vector<double> v1, Vector<double> v2) => (v1 - v2).L2Norm();
        public static Func<Vector<double>, double> DistL2FromVector(this Vector<double> @this) => otherVec => DistanceL2(@this, otherVec);


        public static void AddInPlace(this Vector<double> @this, Vector<double> other)
        {
            for (int i = 0; i < @this.Count; i++)
                @this[i] += other[i];
        }

    }
}
