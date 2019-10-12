using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntropyMathematics;
using Monitoring.GeometricMonitoring;
using Monitoring.Utils;
using Utils.AiderTypes;
using Utils.MathUtils;
using Utils.SparseTypes;

namespace EntropySketch
{
    public partial class EntropySketchFunction
    {
        public ConvexBound LowerBound(Vector reducedVector, double threshold)
        {
            Either<Vector, double> DistanceL1(Vector point, int node)
            {
                var functionValue = ComputeEntropySketch(point);
                if (functionValue > threshold)
                    return DecreaseEntropySketch.l1DecreaseEntropySketchTo(point, Dimension, threshold, Approximations.ApproximationEpsilon);
                if (functionValue < threshold)
                    return IncreaseEntropySketch.l1IncreaseEntropySketchTo(point, Dimension, threshold, Approximations.ApproximationEpsilon);
                return point;
            }

            return ConvexBoundBuilder.Create(MonitoredFunction.Function, ComputeEntropySketch, value => value >= threshold)
                                     .WithDistanceNorm(1, DistanceL1)
                                     .ToConvexBound();
        }

    }
}
