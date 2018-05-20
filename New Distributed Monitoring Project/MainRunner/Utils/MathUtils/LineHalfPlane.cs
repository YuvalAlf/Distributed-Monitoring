using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Utils.TypeUtils;

namespace Utils.MathUtils
{
    public sealed class LineHalfPlane
    {
        public Vector<double> Parameters { get; }
        public double ConstantPart { get; }
        public double Threshold { get; }

        private LineHalfPlane(Vector<double> parameters, double constantPart, double threshold)
        {
            Parameters = parameters;
            ConstantPart = constantPart;
            Threshold = threshold;
        }

        public static LineHalfPlane Create(Vector<double> paramters, double constantPart, double threshold)
        {
            return new LineHalfPlane(paramters, constantPart, threshold);
        }

        public double Compute(Vector<double> input) => Parameters * input + ConstantPart;
        
        //TODO: Check thoroughly
        public Vector<double> ClosestPointL1(Vector<double> point)
        {
            var sigma = Parameters * point;
            var minDiff = double.MaxValue;
            int minIndex = 0;
            for (int i = 0; i < point.Count; i++)
            {
                var pi = Parameters[i];

                var diff = (Threshold - ConstantPart - sigma) / pi;
                diff = Math.Abs(diff);
                if (diff < minDiff)
                {
                    minDiff = diff;
                    minIndex = i;
                }
            }

            var closestPoint = point.Clone();
            closestPoint[minIndex] += (Threshold - ConstantPart - sigma) / Parameters[minIndex];
            return closestPoint;
        }


        //TODO: Check thoroughly
        public Vector<double> ClosestPointL2(Vector<double> point)
        {
            var sigmaParameterSquared = Parameters * Parameters;
            var sigma = Parameters * point;
            var diffVector = Parameters.Select(p => p * (ConstantPart + sigma - Threshold) / sigmaParameterSquared).ToVector();
            return point - diffVector;
        }
    }
}
