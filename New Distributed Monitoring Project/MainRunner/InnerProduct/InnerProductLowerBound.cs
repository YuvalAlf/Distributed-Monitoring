using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using MoreLinq;
using Utils.MathUtils;
using Utils.TypeUtils;

namespace InnerProduct
{
    public static partial class InnerProductFunction
    {
        public static ConvexBound LowerBound(Vector<double> initialVector, double threshold)
        {
            (var x0, var y0) = initialVector.Halve();
            var constantPart = (x0 + y0) * (x0 + y0);
            double ConvexFunc(Vector<double> vector)
            {
                (var x, var y) = vector.Halve();
                var convexPart = (x - y) * (x - y);
                var linearPart = 2 * (vector - initialVector) * (x0 + y0).VConcat(x0 + y0);
                return 0.25 * ((linearPart + constantPart) - convexPart);
            }


            Either<Vector<double>, double> ClosestPointFromVector(Vector<double> vector)
            {
                if (double.IsNegativeInfinity(threshold))
                    return Enumerable.Repeat(double.NegativeInfinity, vector.Count).ToVector();
                var T = threshold;
                (var p0, var q0) = vector.Halve();
                var m = p0 + q0;
                var n = p0 - q0;
                var k = x0 + y0;

                Vector<double> GetXY(double t)
                {
                    var sum = m - k + k * t;
                    var diff = n / t;
                    var x = (sum + diff) / 2.0;
                    var y = (sum - diff) / 2.0;
                    return x.VConcat(y);
                }

                var a = -2 * k * k;
                var b = -2 * k * m + 3 * k * k + 4 * T;
                var c = 0;
                var d = n * n;
                var ts = FindRoots.Cubic(d, c, b, a).ToArray();
                if (a == 0.0)
                    ts = FindRoots.Quadratic(d, c, b).ToArray();
                return ts.Where(t => t.IsNearReal()).Select(t => t.Real)
                    .Select(GetXY).MinBy(vector.DistL2FromVector()).First();
            }

            return ConvexBoundBuilder.Create(ConvexFunc, val => val >= threshold).WithDistanceNorm(2, ClosestPointFromVector).ToConvexBound();
        }
    }
}
