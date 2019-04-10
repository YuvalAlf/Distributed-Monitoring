using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Utils.MathUtils;
using Utils.TypeUtils;

namespace ClassLibrary1
{
    public static partial class SpectralGapFunction
    {
        private static ConvexBound UpperBound(Vector<double> referenceMatrix, double threshold)
        {
            return ConvexBoundBuilder.Create(_ => 0.0, value => value <= threshold)
                                     .WithDistanceNorm(2, (point, id) => 0.0)
                                     .ToConvexBound();
        }
    }
}
