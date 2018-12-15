using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Utils.SparseTypes;
using norm = System.Int32;

namespace Monitoring.GeometricMonitoring
{
    public sealed class ConvexBoundBuilder
    {
        private Func<Vector, double> Compute { get; }
        private Predicate<double> IsInBound { get; }
        private Dictionary<norm, ClosestPointFromPoint> GetClosestPointOfNorm { get; }

        private ConvexBoundBuilder(Func<Vector, double> compute, Predicate<double> isInBound, Dictionary<int, ClosestPointFromPoint> getClosestPointOfNorm)
        {
            Compute = compute;
            IsInBound = isInBound;
            GetClosestPointOfNorm = getClosestPointOfNorm;
        }

        public static ConvexBoundBuilder Create(Func<Vector, double> computeFunction, Predicate<double> isInBound) 
            => new ConvexBoundBuilder(computeFunction, isInBound, new Dictionary<int, ClosestPointFromPoint>(2));

        public ConvexBoundBuilder WithDistanceNorm(int norm, ClosestPointFromPoint closestPointFunction)
        {
            this.GetClosestPointOfNorm[norm] = closestPointFunction;
            return this;
        }

        public ConvexBound ToConvexBound() => new ConvexBound(Compute, IsInBound, GetClosestPointOfNorm);
    }
}
