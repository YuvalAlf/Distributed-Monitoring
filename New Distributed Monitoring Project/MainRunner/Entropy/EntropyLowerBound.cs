using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;

namespace Entropy
{
    public static partial class EntropyFunction
    {
        public static double LowerBoundConvexBoundEntropy(Vector<double> probVector)
        {
            double xLine = 1.0 / (10.0 * probVector.Count);
            double yLine = Entropy(xLine);
            double m = yLine / xLine;
            double ComputeEntropyToValue(double value)
            {
                if (value > xLine)
                    return Entropy(value);
                return m * value;
            }

            return probVector.Select(ComputeEntropyToValue).Sum();
        }

        public static ConvexBound LowerBound(Vector<double> initVector, double threshold)
        {
            Vector<double> DistanceL1(Vector<double> point)
            {
                var entropy = LowerBoundConvexBoundEntropy(point);
                if (entropy > threshold)
                    return EntropyMath.ClosestL1PointFromAbove(threshold, point);
                if (entropy < threshold)
                    return EntropyMath.ClosestL1PointFromBelow(threshold, point);
                return initVector;
            }

            return ConvexBoundBuilder.Create(LowerBoundConvexBoundEntropy, value => value >= threshold).WithDistanceNorm(1, DistanceL1).ToConvexBound();
        }
    }
}
