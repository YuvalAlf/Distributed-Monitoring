using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Utils.AiderTypes;
using Utils.SparseTypes;
using Utils.TypeUtils;
using norm = System.Int32;
namespace Monitoring.GeometricMonitoring
{
    public delegate Either<Vector, double> ClosestPointFromPoint(Vector givenPoint, int node);
    public sealed class ConvexBound
    {
        public Func<Vector, double> MonitoredFunction { get; }
        public Func<Vector, double> Compute { get; }
        public Predicate<double> IsInBound { get; }
        private Dictionary<norm, ClosestPointFromPoint> GetClosestPointOfNorm { get; }
        public IEnumerable<int> Norms => GetClosestPointOfNorm.Keys;

        public ConvexBound(Func<Vector, double> monitoredFunction, Func<Vector, double> compute, Predicate<double> isInBound, Dictionary<int, ClosestPointFromPoint> getClosestPointOfNorm)
        {
            MonitoredFunction = monitoredFunction;
            Compute = compute;
            IsInBound = isInBound;
            GetClosestPointOfNorm = getClosestPointOfNorm;
        }
        
        public double ComputeDistance(int norm, Vector givenPoint, int node)
        {
            var multiply = IsInBound(this.Compute(givenPoint)) ? -1 : +1;
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
