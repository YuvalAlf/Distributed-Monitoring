using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitoring.GeometricMonitoring;
using Monitoring.Utils;
using Utils.MathUtils;
using Utils.SparseTypes;

namespace EntropySketch
{
    public partial class EntropySketchFunction
    {
        private ConvexBound LowerBound(Vector reducedVector, double threshold)
        {
            return ConvexBoundBuilder.Create(MonitoredFunction.Function, ComputeEntropySketch, value => value >= threshold)
                                     .ToConvexBound();
        }
    }
}
