using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.VectorType;

namespace Entropy
{
    public static partial class EntropyFunction
    {
        public static MonitoredFunction MonitoredFunction => new MonitoredFunction(ComputeEntropy, UpperBound, LowerBound, GlobalVectorType.Average, 1);

        public static double MinEntropy => 0.0;
        public static double MaxEntropy(int vecLength) => vecLength * Entropy(1.0 / vecLength);

        public static double Entropy(double value)
        {
            if (value < -0.0000001)
                return double.NegativeInfinity;
            if (value <= 0.0)
                return 0.0;
            return -value * Math.Log(value);
        }

        public static double ComputeEntropy(Vector<double> vector)
        {
            return vector.Select(Entropy).Sum();
        }
    }
}
