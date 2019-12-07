using System;
using Monitoring.GeometricMonitoring;
using Utils.MathUtils;
using Utils.SparseTypes;

namespace Monitoring.Utils
{
    public static class LineHalfPlaneConvexBoundUtils
    {
        public static ConvexBound ToConvexLowerBound(this LineHalfPlane @this, Func<Vector, double> monitoredFunction, double threshold)
        {
            return @this.ToConvexBound(monitoredFunction, ConvexBound.Type.LoweBound, threshold);
        }
        public static ConvexBound ToConvexUpperBound(this LineHalfPlane @this, Func<Vector, double> monitoredFunction, double threshold)
        {
            return @this.ToConvexBound(monitoredFunction, ConvexBound.Type.UpperBound, threshold);
        }
        private static ConvexBound ToConvexBound(this LineHalfPlane @this, Func<Vector, double> monitoredFunction, ConvexBound.Type type, double threshold)
        {
            return ConvexBoundBuilder.Create(monitoredFunction, @this.Compute, type, threshold)
                .WithDistanceNorm(1, @this.ClosestPointL1)
                .WithDistanceNorm(2, @this.ClosestPointL2)
                .ToConvexBound();
        }
    }
}
