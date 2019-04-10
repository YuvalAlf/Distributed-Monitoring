using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Monitoring.Utils;
using Utils.MathUtils;
using Utils.TypeUtils;

namespace Entropy
{
    public static partial class EntropyFunction
    {
        public static ConvexBound UpperBound(Vector<double> initVector, double threshold)
        {
            var constantPart = initVector.Sum();
            var parameters = initVector.Select(pi => -Math.Log(pi + 0.000000001) - 1).ToVector();
            var lineHalfPlane = LineHalfPlane.Create(parameters, constantPart, threshold);

            return lineHalfPlane.ToConvexUpperBound();
        }
    }
}
