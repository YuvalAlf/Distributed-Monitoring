using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Monitoring.Utils;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Entropy
{
    public partial class EntropyFunction
    {
        public ConvexBound UpperBound(Vector initVector, double threshold)
        {
            var constantPart = initVector.IndexedValues.Values.Sum();
            var parameters = initVector.Map(Dimension, pi => -Math.Log(pi + 0.000000001) - 1);
            var lineHalfPlane = LineHalfPlane.Create(parameters, constantPart, threshold, Dimension);
            
            return lineHalfPlane.ToConvexUpperBound(MonitoredFunction.Function, threshold);
        }
    }
}
