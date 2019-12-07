using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Utils.SparseTypes;
using norm = System.Int32;

namespace Monitoring.GeometricMonitoring
{
    public sealed class ConvexBoundBuilder
    {
        private Func<Vector, double> MonitoredFunction { get; }
        private Func<Vector, double> Compute { get; }
        public double Threshold { get; }
        public ConvexBound.Type Type { get; }
        private Dictionary<norm, ClosestPointFromPoint> GetClosestPointOfNorm { get; }

        private ConvexBoundBuilder(Func<Vector, double> monitoredFunction, Func<Vector, double> compute, Dictionary<int, ClosestPointFromPoint> getClosestPointOfNorm, double threshold, ConvexBound.Type type)
        {
            MonitoredFunction = monitoredFunction;
            Compute = compute;
            GetClosestPointOfNorm = getClosestPointOfNorm;
            Threshold = threshold;
            Type = type;
        }

        public static ConvexBoundBuilder Create(Func<Vector, double> monitoredFuntion, Func<Vector, double> computeFunction, ConvexBound.Type type, double threhsold) 
            => new ConvexBoundBuilder(monitoredFuntion, computeFunction, new Dictionary<int, ClosestPointFromPoint>(2), threhsold, type);

        public ConvexBoundBuilder WithDistanceNorm(int norm, ClosestPointFromPoint closestPointFunction)
        {
            this.GetClosestPointOfNorm[norm] = closestPointFunction;
            return this;
        }

        public ConvexBound ToConvexBound() => new ConvexBound(MonitoredFunction, Compute, GetClosestPointOfNorm, Threshold, Type);
    }
}
