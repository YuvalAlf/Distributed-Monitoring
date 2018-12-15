using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using MoreLinq;
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
            var InitialSum = x0.Add(y0);
            var constantPart = InitialSum.InnerProduct(InitialSum);
            double ConvexFunc(Vector vector)
            {
                (var x, var y) = vector.Halve(HalfDimension);
                var diff = x.Subtruct(y);
                var convexPart = diff.InnerProduct(diff);
                var linearPart = 2 * (vector.Subtruct(initialVector).InnerProduct(InitialSum.Concat(InitialSum, HalfDimension)));
                return 0.25 * ((linearPart + constantPart) - convexPart);
            }


            Either<Vector, double> ClosestPointFromVector(Vector vector, int nodeId)
            {
                if (double.IsNegativeInfinity(threshold))
                    return double.PositiveInfinity;
                var T = threshold;
                (var p0, var q0) = vector.Halve(HalfDimension);
                var m = p0.Add(q0);
                var n = p0.Subtruct(q0);
                var k = InitialSum;

                Vector GetXY(double t)
                {
                    var sum = m.Subtruct(k).Add(k.MulBy(t));
                    var diff = n.Divide(t);
                    var x = sum.Add(diff).Divide(2.0);
                    var y = sum.Subtruct(diff).Divide(2.0);
                    return x.Concat(y, HalfDimension);
                }

                var a = -2 * k.InnerProduct(k);
                var b = -2 * k.InnerProduct(m) + 3 * k.InnerProduct(k) + 4 * T;
                var c = 0;
                var d = n.InnerProduct(n);
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
