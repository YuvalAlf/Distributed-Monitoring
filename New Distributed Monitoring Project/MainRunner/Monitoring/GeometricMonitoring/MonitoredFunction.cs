using System;
using threshold = System.Double;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.SparseTypes;

namespace Monitoring.GeometricMonitoring
{
    public sealed class MonitoredFunction
    {
        public Func<Vector, double> Function { get; }
        public Func<Vector, threshold, ConvexBound> UpperBound { get; }
        public Func<Vector, threshold, ConvexBound> LowerBound { get; }
        public GlobalVectorType GlobalVectorType { get; }
        public int[] Norms { get; }

        public MonitoredFunction(
            Func<Vector, double> function,
            Func<Vector, threshold, ConvexBound> upperBound,
            Func<Vector, threshold, ConvexBound> lowerBound,
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
