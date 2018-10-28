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
            var (eigenvector, eigenvalue) = referenceMatrix.AsMatrix().PowerIterationMethod(Epsilon, Rnd);

            double UpperBoundFunction(Vector<double> currentVector)
            {
                var currentMatrix = currentVector.AsMatrix();
                var l1HalfPlane = eigenvector * currentMatrix * eigenvector;
                var l2ConvexBound = currentMatrix.PowerIterationMethod2(Epsilon, eigenvector, Rnd).Eigenvalue;
                return l1HalfPlane - l2ConvexBound;
            }
            Either<Vector<double>, double> CalculateDistance(Vector<double> currentVector)
            {
                var spectralGap = SpectralGapFunction.Compute(currentVector);
                if (spectralGap < threshold)
                    return DistanceFromInside(currentVector);
                if (spectralGap > threshold)
                    return DistanceFromOutside(currentVector);
                return 0;
            }
            Either<Vector<double>, double> DistanceFromInside(Vector<double> currentVector)
            {
                var value = UpperBoundFunction(currentVector);
                var delta = (threshold - value);
                return Math.Sqrt(2) * delta;
            }

            Either<Vector<double>, double> DistanceFromOutside(Vector<double> currentVector)
            {
                return 0.0;
            }

            return ConvexBoundBuilder.Create(UpperBoundFunction, value => value <= threshold)
                                     .WithDistanceNorm(2, CalculateDistance)
                                     .ToConvexBound();
        }

    }
}
