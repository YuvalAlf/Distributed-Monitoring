using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitoring.GeometricMonitoring;
using Monitoring.Utils;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace EntropySketch
{
    public partial class EntropySketchFunction
    {
        public ConvexBound UpperBound(Vector reducedVector, double threshold)
        {
            var exps = reducedVector.Enumerate(Dimension).Select(Math.Exp).ToVector();
            var sum = exps.Sum();
            Vector paramters = exps.Enumerate(Dimension).Select(exp => - exp / sum).ToVector();
            double constantPart = ComputeEntropySketch(reducedVector) - paramters * reducedVector;
            return LineHalfPlane.Create(paramters, constantPart, threshold, Dimension)
                                .ToConvexUpperBound(MonitoredFunction.Function, threshold);
        }
    }
}
