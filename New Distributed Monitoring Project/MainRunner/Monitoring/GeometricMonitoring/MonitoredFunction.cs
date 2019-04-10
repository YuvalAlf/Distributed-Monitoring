using System;
using threshold = System.Double;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring.VectorType;

namespace Monitoring.GeometricMonitoring
{
    public sealed class MonitoredFunction
    {
        public Func<Vector<double>, double> Function { get; }
        public Func<Vector<double>, threshold, ConvexBound> UpperBound { get; }
        public Func<Vector<double>, threshold, ConvexBound> LowerBound { get; }
        public GlobalVectorType GlobalVectorType { get; }
        public int[] Norms { get; }

        public MonitoredFunction(
            Func<Vector<double>, double> function,
            Func<Vector<double>, threshold, ConvexBound> upperBound,
            Func<Vector<double>, threshold, ConvexBound> lowerBound,
            GlobalVectorType globalVectorType,
            params int[] norms)
        {
            Function = function;
            UpperBound = upperBound;
            LowerBound = lowerBound;
            GlobalVectorType = globalVectorType;
            Norms = norms;
        }
    }
}
