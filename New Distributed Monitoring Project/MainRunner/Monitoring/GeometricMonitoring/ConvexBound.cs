using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using norm = System.Int32;
namespace Monitoring.GeometricMonitoring
{
    public delegate Vector<double> ClosestPointFromPoint(Vector<double> givenPoint);
    public sealed class ConvexBound
    {
        public Func<Vector<double>, double> Compute { get; }
        public Predicate<double> IsInBound { get; }
        private Dictionary<norm, ClosestPointFromPoint> GetClosestPointOfNorm { get; }

        public ConvexBound(Func<Vector<double>, double> compute, Predicate<double> isInBound, Dictionary<int, ClosestPointFromPoint> getClosestPointOfNorm)
        {
            Compute = compute;
            IsInBound = isInBound;
            GetClosestPointOfNorm = getClosestPointOfNorm;
        }

        public (double distance, Vector<double> residualVector) ComputeDistance(int norm, Vector<double> givenPoint)
        {
            var multiply = IsInBound(this.Compute(givenPoint)) ? -1 : +1;
            var closestPoint = GetClosestPointOfNorm[norm](givenPoint);
            var residualVector = givenPoint - closestPoint;
            var distance = residualVector.Norm(norm);
            return (multiply * distance, residualVector);
        }
    }
}
