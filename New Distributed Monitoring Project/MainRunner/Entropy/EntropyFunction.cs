using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.SparseTypes;

namespace Entropy
{
    public partial class EntropyFunction
    {
        public int Dimension { get; }
        public MonitoredFunction MonitoredFunction { get; }

        public double MinEntropy { get; }
        public double MaxEntropy { get; }

        public EntropyFunction(int dimension)
        {
            Dimension = dimension;
            MonitoredFunction = new MonitoredFunction(ComputeEntropy, UpperBound, LowerBound, GlobalVectorType.Average, 1);
            MinEntropy = 0.0;
            MaxEntropy = Math.Log(Dimension);
        }

        public double ComputeEntropy(Vector vector)
        {
            double ComputeEntropyToValue(double value)
            {
                if (value <= 0.0)
                    return 0.0;
                return -value * Math.Log(value);
            }

            return vector.IndexedValues.Values.Select(ComputeEntropyToValue).Sum();
        }
    }
}
