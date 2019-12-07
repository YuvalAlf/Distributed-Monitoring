using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Sphere
{
    public partial class SphereFunction
    {
        public ConvexBound UpperBound(Vector initialVector, double threshold)
        {
            ClosestPointFromPoint closestPoint = (point, nodeId) =>
                                                 {
                                                     var currentSum = Compute(point);
                                                     if (currentSum <= 0.0000000001)
                                                         return threshold;
                                                     
                                                     var desiredSum = threshold;
                                                     var mulBy      = Math.Sqrt(threshold / currentSum);
                                                     var result = point * mulBy;
                                                     return result;
                                                 };

            return ConvexBoundBuilder.Create(MonitoredFunction.Function, Compute, ConvexBound.Type.UpperBound, threshold)
                                     .WithDistanceNorm(2, closestPoint)
                                     .ToConvexBound();
        }
    }
}
