using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;

namespace Entropy
{
    public static partial class EntropyFunction
    {
        public static ConvexBound LowerBound(Vector<double> initVector, double threshold)
        {
            Vector<double> DistanceL1(Vector<double> point)
            {
                var entropy = ComputeEntropy(point);
                if (entropy > threshold)
                    return EntropyMath.ClosestL1PointFromAbove(threshold, point);
                if (entropy < threshold)
                    return EntropyMath.ClosestL1PointFromBelow(threshold, point);
                return initVector;
            }

            return ConvexBoundBuilder.Create(ComputeEntropy, value => value >= threshold)
                                     .WithDistanceNorm(1, DistanceL1).ToConvexBound();
        }
    }
}
