using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.SparseTypes;

namespace InnerProduct
{
    public sealed partial class InnerProductFunction
    {
        public int Dimension { get; }
        public int HalfDimension => Dimension / 2;
        public MonitoredFunction MonitoredFunction { get; }

        public InnerProductFunction(int dimension)
        {
            Dimension = dimension;
            MonitoredFunction = new MonitoredFunction(Compute, UpperBound, LowerBound, GlobalVectorType.Sum, 2);
        }

        public double Compute(Vector vector)
        {
            var (vector1, vector2) = vector.Halve(HalfDimension);
            return vector1.InnerProduct(vector2);
        }
        
    }
}
