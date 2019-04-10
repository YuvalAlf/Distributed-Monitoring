using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.VectorType;

namespace Sphere
{
    public static partial class SphereFunction
    {
        public static double Compute(Vector<double> vector) => vector * vector;

        public static MonitoredFunction MonitoredFunction = new MonitoredFunction(Compute, UpperBound, LowerBound, GlobalVectorType.Average, 2);
    }
}
