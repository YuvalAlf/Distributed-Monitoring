using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Monitoring.Utils;
using Utils.MathUtils;
using Utils.TypeUtils;

namespace Sphere
{
    public static partial class SphereFunction
    {
        public static ConvexBound LowerBound(Vector<double> initialVector, double threshold)
        {
            if (threshold <= 0)
            {
                ClosestPointFromPoint closestPoint = pt => Enumerable.Repeat((double)int.MaxValue, pt.Count).ToVector();
                return ConvexBoundBuilder.Create(_ => double.NegativeInfinity, _ => true)
                                         .WithDistanceNorm(1, closestPoint)
                                         .WithDistanceNorm(2, closestPoint)
                                         .ToConvexBound();
            }

            var currentNorm = Compute(initialVector);
            var newPointNorm = threshold;
            var mulBy = Math.Sqrt(newPointNorm / currentNorm);
            var point = initialVector.Select(x => x * mulBy).ToVector();

            var constantPart  = -currentNorm;
            var parameters    = 2 * initialVector;
            var lineHalfPlane = LineHalfPlane.Create(parameters, constantPart, threshold);

            return lineHalfPlane.ToConvexLowerBound();
        }
    }
}
