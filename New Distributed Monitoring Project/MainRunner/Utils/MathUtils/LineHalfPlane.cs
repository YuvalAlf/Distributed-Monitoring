using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Utils.AiderTypes;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Utils.MathUtils
{
    public sealed class LineHalfPlane
    {
        public int Dimension { get; }
        public Vector Parameters   { get; }
        public double ConstantPart { get; }
        public double Threshold    { get; }

        private LineHalfPlane(Vector parameters, double constantPart, double threshold, int dimension)
        {
            Parameters   = parameters;
            ConstantPart = constantPart;
            Threshold    = threshold;
            Dimension = dimension;
        }

        public static LineHalfPlane Create(Vector paramters, double constantPart, double threshold, int dimension)
            => new LineHalfPlane(paramters, constantPart, threshold, dimension);

        public double Compute(Vector input) => Parameters * input + ConstantPart;

        public Either<Vector, double> ClosestPointL1(Vector point, int nodeId)
        {
            var sigma    = Parameters * point;
            var minDiff  = double.MaxValue;
            int minIndex = 0;
            for (int i = 0; i < Dimension; i++)
            {
                var pi = Parameters[i];

                var diff = (Threshold - ConstantPart - sigma) / pi;
                diff = Math.Abs(diff);
                if (diff < minDiff)
                {
                    minDiff  = diff;
                    minIndex = i;
                }
            }

            var closestPoint = point.Clone();
            closestPoint[minIndex] += (Threshold - ConstantPart - sigma) / Parameters[minIndex];
            return closestPoint;
        }

        public Either<Vector, double> ClosestPointL2(Vector point, int nodeId)
        {
            var sigmaParameterSquared = Parameters * Parameters;
            var sigma                 = Parameters * point;
            var diffVector = Parameters.Map(Dimension, p => p * (ConstantPart + sigma - Threshold) / sigmaParameterSquared);
            return point - diffVector;
        }
    }
}
