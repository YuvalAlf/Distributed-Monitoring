using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Utils.TypeUtils;

namespace Sphere
{
    public static partial class SphereFunction
    {
        public static ConvexBound UpperBound(Vector<double> initialVector, double threshold)
        {
            ClosestPointFromPoint closestPoint = (point, nodeId) =>
                                                 {
                                                     var currentSum = Compute(point);
                                                     if (currentSum <= 0.0000000001)
                                                         return new[] {Math.Sqrt(threshold)}
                                                               .Concat(Enumerable.Repeat(0.0, initialVector.Count - 1))
                                                               .ToVector();
                                                     var desiredSum = threshold;
                                                     var mulBy      = Math.Sqrt(threshold / currentSum);
                                                     var result = point * mulBy;
                                                     return result;
                                                 };

            return ConvexBoundBuilder.Create(Compute, value => value <= threshold)
                                     .WithDistanceNorm(2, closestPoint)
                                     .ToConvexBound();
        }
    }
}
