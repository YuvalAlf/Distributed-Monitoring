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
        public static double MaxEntropy(int vecLength) => - Math.Log(1.0 / vecLength);

        public static double ComputeEntropy(Vector<double> vector)
        {
            double xLine = 1.0 / (10.0 * vector.Count);
            double yLine = -xLine * Math.Log(xLine);
            double m     = yLine / xLine;
            double ComputeEntropyToValue(double value)
            {
                if (value > xLine)
                    return -value * Math.Log(value);
                return m * value;
            }

            return vector.Select(ComputeEntropyToValue).Sum();
        }
    }
}
