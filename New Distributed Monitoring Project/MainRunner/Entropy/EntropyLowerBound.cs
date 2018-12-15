using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Entropy
{
    public partial class EntropyFunction
    {
        public ConvexBound LowerBound(Vector initVector, double threshold)
        {
            Either<Vector, double> DistanceL1(Vector point, int nodeId)
            {
                var entropy = LowerBoundEntropy(point);
                if (entropy > threshold)
                    return ClosestL1PointFromAbove(threshold, point);
                if (entropy < threshold)
                    return ClosestL1PointFromBelow(threshold, point);
                return initVector;
            }

            return ConvexBoundBuilder.Create(LowerBoundEntropy, value => value >= threshold)
                                     .WithDistanceNorm(1, DistanceL1).ToConvexBound();
        }


        public double LowerBoundEntropy(Vector vector)
        {
            double xLine = 1.0    / (10.0 * Dimension);
            double yLine = -xLine * Math.Log(xLine);
            double m     = yLine  / xLine;
            double ComputeEntropyToValue(double value)
            {
                if (value > xLine)
                    return -value * Math.Log(value);
                return m * value;
            }

            return vector.IndexedValues.Values.Select(ComputeEntropyToValue).Sum();
        }
    }
}
