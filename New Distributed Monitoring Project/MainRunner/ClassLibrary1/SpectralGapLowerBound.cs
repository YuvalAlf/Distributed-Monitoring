using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Utils.TypeUtils;

namespace ClassLibrary1
{
    public static partial class SpectralGapFunction
    {
        private static ConvexBound LowerBound(Vector<double> referenceMatrix, double threshold)
        {
            var (eigenvector, eigenvalue) = referenceMatrix.AsMatrix().PowerIterationMethod(Epsilon, Rnd);
            var lastDataInside = new Dictionary<int, Vector<double>>();

            double LowerBoundFunction(Vector<double> currentVector)
            {
                var currentMatrix = currentVector.AsMatrix();
                var l1HalfPlane   = eigenvector * currentMatrix * eigenvector;
                var l2ConvexBound = currentMatrix.SecondLargestEigenvalue(eigenvector, eigenvalue, Epsilon, Rnd);
                return l1HalfPlane - l2ConvexBound;
            }
            Either<Vector<double>, double> CalculateDistance(Vector<double> currentVector, int node)
            {
                var lowerBound = LowerBoundFunction(currentVector);
                if (lowerBound > threshold)
                    return DistanceFromInside(currentVector, node);
                if (lowerBound < threshold)
                    return DistanceFromOutside(currentVector, node);
                return 0;
            }
            Either<Vector<double>, double> DistanceFromInside(Vector<double> currentVector, int node)
            {
                lastDataInside[node] = currentVector.Clone();
                var value = LowerBoundFunction(currentVector);
                var delta = (value - threshold);
                return delta / Math.Sqrt(2);
            }

            Either<Vector<double>, double> DistanceFromOutside(Vector<double> currentVector, int node)
            {
                var result = (currentVector - lastDataInside[node]).L2Norm();
                return result;
            }

            return ConvexBoundBuilder.Create(LowerBoundFunction, value => value >= threshold)
                                     .WithDistanceNorm(2, CalculateDistance)
                                     .ToConvexBound();
        }
    }
}
