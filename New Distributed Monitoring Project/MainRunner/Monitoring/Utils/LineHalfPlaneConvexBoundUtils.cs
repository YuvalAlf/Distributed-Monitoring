using System;
using Monitoring.GeometricMonitoring;
using Utils.MathUtils;

namespace Monitoring.Utils
{
    public static class LineHalfPlaneConvexBoundUtils
    {
        public static ConvexBound ToConvexLowerBound(this LineHalfPlane @this)
        {
            return @this.ToConvexBound((a, b) => a >= b);
        }
        public static ConvexBound ToConvexUpperBound(this LineHalfPlane @this)
        {
            return @this.ToConvexBound((a, b) => a <= b);
        }
        private static ConvexBound ToConvexBound(this LineHalfPlane @this, Func<double, double, bool> isInBoundCheck)
        {
            return ConvexBoundBuilder.Create(@this.Compute, @this.Compute, val => isInBoundCheck(val, @this.Threshold))
                .WithDistanceNorm(1, @this.ClosestPointL1)
                .WithDistanceNorm(2, @this.ClosestPointL2)
                .ToConvexBound();
        }
    }
}
