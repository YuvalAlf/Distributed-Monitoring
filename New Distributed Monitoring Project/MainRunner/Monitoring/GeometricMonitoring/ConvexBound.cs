using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Utils.SparseTypes;
using Utils.TypeUtils;
using norm = System.Int32;
namespace Monitoring.GeometricMonitoring
{
    public sealed class ConvexBound
    {
        public enum Type
        {
            UpperBound,
            LoweBound
        };

        public double Threshold { get; }
        public Type ConvexBoundType { get; }
        public Func<Vector, double> MonitoredFunction { get; }
        public Func<Vector, double> Compute { get; }
        private Dictionary<norm, ClosestPointFromPoint> GetClosestPointOfNorm { get; }
        public IEnumerable<int> Norms => GetClosestPointOfNorm.Keys;

        public ConvexBound(Func<Vector, double> monitoredFunction, Func<Vector, double> compute, Dictionary<int, ClosestPointFromPoint> getClosestPointOfNorm, double threshold, Type convexBoundType)
        {
            MonitoredFunction = monitoredFunction;
            Compute = compute;
            GetClosestPointOfNorm = getClosestPointOfNorm;
            Threshold = threshold;
            ConvexBoundType = convexBoundType;
        }

        public bool IsInBound(Vector vector)
        {
            var convexValue = this.Compute(vector);
            return IsInBound(convexValue);
        }
        public bool IsInBound(double convexValue)
        {
            switch (ConvexBoundType)
            {
                case Type.UpperBound:
                    return convexValue <= Threshold;
                case Type.LoweBound:
                    return convexValue >= Threshold;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public double ComputeDistance(int norm, Vector givenPoint, int node)
        {
            var multiply = IsInBound(givenPoint) ? -1 : +1;
            var closestPoint = GetClosestPointOfNorm[norm](givenPoint, node);
            if (closestPoint.IsChoice2)
                return multiply * closestPoint.GetChoice2;
            else
            {
                var residualVector = givenPoint - closestPoint.GetChoice1;
                var distance       = residualVector.Norm(norm);
                return multiply * distance;
            }
        }
    }
}
