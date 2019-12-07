using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Utils.AiderTypes;
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

            return ConvexBoundBuilder.Create(MonitoredFunction.Function, LowerBoundEntropy, ConvexBound.Type.LoweBound, threshold)
                                     .WithDistanceNorm(1, DistanceL1).ToConvexBound();
        }


        private Lazy<double> XLine => new Lazy<double>(() => 1.0 / (10.0 * Dimension));
        private Lazy<double> YLine => new Lazy<double>(() => -XLine.Value * Math.Log(XLine.Value));
        private Lazy<double> M => new Lazy<double>(() => YLine.Value / XLine.Value);

        double LowerBoundComputeEntropyToValue(double value)
        {
            if (value > XLine.Value)
                return -value * Math.Log(value);
            return M.Value * value;
        }


        public double LowerBoundEntropy(Vector vector)
        {
            return vector.IndexedValues.Values.Select(LowerBoundComputeEntropyToValue).Sum();
        }
    }
}
