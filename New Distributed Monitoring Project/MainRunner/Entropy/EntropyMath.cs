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
        // Entropy > Threshold
        // Decrease entropy to reach threshold
        public Vector ClosestL1PointFromAbove(double desiredEntropy, Vector point)
        {
            Func<Vector, double> entropyFunction = LowerBoundEntropy;

            return EntropyMathematics.DecreaseEntropy.l1DecreaseEntropy(point, Dimension, desiredEntropy, XLine.Value, Approximations.ApproximationEpsilon);

        }

        // Entropy < Threshold
        // Increase entropy to reach threshold
        public Vector ClosestL1PointFromBelow(double desiredEntropy, Vector point)
        {
            if (desiredEntropy >= MaxEntropy)
                return Enumerable.Repeat(1.0 / Dimension, Dimension).ToVector();
            
            return EntropyMathematics.IncreaseEntropy.l1IncreaseEntropyTo(point, Dimension, desiredEntropy, XLine.Value, Approximations.ApproximationEpsilon);
        }
    }
}