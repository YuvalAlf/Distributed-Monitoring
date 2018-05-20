using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.VectorType;

namespace InnerProduct
{
    public static partial class InnerProductFunction
    {
        public static double Compute(Vector<double> vector)
        {
            Debug.Assert(vector.Count % 2 == 0);
            var halfLength = vector.Count / 2;
            var sum = 0.0;
            for (int i = 0; i < halfLength; i++)
                sum += vector[i] * vector[i + halfLength];
            return sum;
        }
        public static MonitoredFunction MonitoredFunction = new MonitoredFunction(Compute, UpperBound, LowerBound, GlobalVectorType.Sum, 2);
    }
}
