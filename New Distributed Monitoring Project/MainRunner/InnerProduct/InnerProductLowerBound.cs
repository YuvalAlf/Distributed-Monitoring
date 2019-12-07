using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using MoreLinq;
using Utils.AiderTypes;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace InnerProduct
{
    public sealed partial class InnerProductFunction
    {
        public ConvexBound LowerBound(Vector initialVector, double threshold)
        {
            (var x0, var y0) = initialVector.Halve(HalfDimension);
            var constantPart = (x0 + y0) * (x0 + y0);

            double ConvexFunc(Vector vector)
            {
                (var x, var y) = vector.Halve(HalfDimension);
                var convexPart = (x - y)                      * (x - y);
                var linearPart = 2 * (vector - initialVector) * (x0 + y0).Concat(x0 + y0, HalfDimension);
                return 0.25 * ((linearPart + constantPart) - convexPart);
            }


            Either<Vector, double> ClosestPointFromVector(Vector vector, int nodeId)
            {
                if (double.IsNegativeInfinity(threshold))
                    return double.PositiveInfinity;
                var T = threshold;
                (var p0, var q0) = vector.Halve(HalfDimension);
                var m = p0 + q0;
                var n = p0 - q0;
                var k = x0 + y0;

                Vector GetXY(double t)
                {
                    var sum  = m - k + k * t;
                    var diff = n            / t;
                    var x    = (sum + diff) / 2.0;
                    var y    = (sum - diff) / 2.0;
                    return x.Concat(y, HalfDimension);
                }

                var a  = -2 * k * k;
                var b  = -2 * k * m + 3 * k * k + 4 * T;
                var c  = 0;
                var d  = n * n;
                var ts = FindRoots.Cubic(d, c, b, a).ToArray();
                if (a == 0.0)
                    ts = FindRoots.Quadratic(d, c, b).ToArray();
                return ts.Where(t => t.IsNearReal()).Select(t => t.Real)
                         .Select(GetXY).MinBy(vector.DistL2FromVector()).First();
            }

            return ConvexBoundBuilder.Create(MonitoredFunction.Function, ConvexFunc, ConvexBound.Type.LoweBound, threshold)
                                     .WithDistanceNorm(2, ClosestPointFromVector).ToConvexBound();
        }
    }
}
