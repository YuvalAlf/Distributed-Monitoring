using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.VectorType;
using Monitoring.Utils;
using Utils.AiderTypes;
using Utils.MathUtils;
using Utils.SparseTypes;

namespace Entropy
{
    public class SpecialCBEntropy
    {
        public int               Dimension         { get; }
        public MonitoredFunction MonitoredFunction { get; }

        public double MinEntropy { get; }
        public double MaxEntropy { get; }

        public SpecialCBEntropy(int dimension)
        {
            Dimension         = dimension;
            MonitoredFunction = new MonitoredFunction(ComputeEntropy, UpperBound, LowerBound, GlobalVectorType.Average /*, 1*/ );
            MinEntropy        = 0.0;
            MaxEntropy        = Math.Log(Dimension);
        }

        public static double ComputeEntropyToValue(double value)
        {
            if (value <= 0.0)
                return 0.0;
            return -value * Math.Log(value);
        }

        public static double ComputeEntropy(Vector vector)
        {
            return vector.IndexedValues.Values.Select(ComputeEntropyToValue).Sum();
        }


        public ConvexBound UpperBound(Vector initVector, double threshold)
        {
            var constantPart  = initVector.IndexedValues.Values.Sum();
            var parameters    = initVector.Map(Dimension, pi => -Math.Log(pi + 0.000000001) - 1);
            var lineHalfPlane = LineHalfPlane.Create(parameters, constantPart, threshold, Dimension);

            return lineHalfPlane.ToConvexUpperBound(MonitoredFunction.Function);
        }


        public ConvexBound LowerBound(Vector initVector, double threshold)
        {
            /*Either<Vector, double> DistanceL1(Vector point, int nodeId)
            {
                var entropy = LowerBoundEntropy(point);
                if (entropy > threshold)
                    return ClosestL1PointFromAbove(threshold, point);
                if (entropy < threshold)
                    return ClosestL1PointFromBelow(threshold, point);
                return initVector;
            }*/
            var pivots = new Dictionary<int, double>(Dimension);
            var gradients = new Dictionary<int, double>(Dimension);
            var generalPivot = 1.0 / (10.0 * Dimension);
            for (int i = 0; i < Dimension; i++)
            {
                var value = initVector[i];
                var pivot = value.AlmostEqual(0) ? generalPivot : Math.Min(generalPivot, value);
                var gradient = ComputeEntropyToValue(pivot) / pivot;
                pivots[i] = pivot;
                gradients[i] = gradient;
            }

            double LowerBoundEntropyToValue(int index, double value)
            {
                if (value <= pivots[index])
                    return gradients[index] * value;
                return ComputeEntropyToValue(value);
            }

            double LowerBoundEntropy(Vector vector) 
                => vector.IndexedValues.Sum(kv => LowerBoundEntropyToValue(kv.Key, kv.Value));

            return ConvexBoundBuilder.Create(MonitoredFunction.Function, LowerBoundEntropy, value => value >= threshold)
                                  // .WithDistanceNorm(1, DistanceL1)
                                     .ToConvexBound();
        }
    }
}
