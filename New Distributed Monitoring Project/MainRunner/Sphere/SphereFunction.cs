using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.SparseTypes;

namespace Sphere
{
    public partial class SphereFunction
    {
        public int Dimension { get; }
        public MonitoredFunction MonitoredFunction { get; }

        public SphereFunction(int dimension)
        {
            Dimension = dimension;
            MonitoredFunction = new MonitoredFunction(Compute, UpperBound, LowerBound, GlobalVectorType.Average, 2);
        }

        public double Compute(Vector vector) => vector * vector;
    }
}
