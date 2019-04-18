using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Entropy
{
    public sealed partial class EntropyFunction
    {
        public static double Approximation => 0.0000001;

        // Entropy > Threshold
        // Decrease entropy to reach threshold
        public Vector ClosestL1PointFromAbove(double desiredEntropy, Vector point)
        {
            Func<Vector, double> entropyFunction = LowerBoundEntropy;
            Predicate<double> l1DistanceOk = l1 => entropyFunction(DecreaseEntropy(l1, point.Clone())) >= desiredEntropy;
            var minL1Distance = 0.0;
            var maxL1Distance = 2.0;
            var distanceL1 = BinarySearch.FindWhere(minL1Distance, maxL1Distance, l1DistanceOk, Approximation);
            return DecreaseEntropy(distanceL1, point.Clone());
        }
        private Vector DecreaseEntropy(double l1Distance, Vector vec)
        {
            var minIndex = vec.MinimumIndex();
            var maxIndex = vec.MaximumIndex();
            if (minIndex == maxIndex)
            {
                minIndex = 0;
                maxIndex = 1;
            }
            vec[minIndex] -= l1Distance / 2.0;
            vec[maxIndex] += l1Distance / 2.0;
            return vec;
        }

        // Entropy < Threshold
        // Increase entropy to reach threshold
        public Vector ClosestL1PointFromBelow(double desiredEntropy, Vector point)
        {
            if (desiredEntropy >= MaxEntropy)
                return Enumerable.Repeat(1.0 / Dimension, Dimension).ToVector();
            
            return EntropyMathematics.Entropy.l1IncreaseEntropyTo(point, Dimension, desiredEntropy, LowerBoundComputeEntropyToValue, Approximation);
        }
    }
}