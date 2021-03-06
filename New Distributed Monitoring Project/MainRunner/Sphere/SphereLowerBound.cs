﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Monitoring.Utils;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Sphere
{
    public partial class SphereFunction
    {
        public ConvexBound LowerBound(Vector initialVector, double threshold)
        {
            if (threshold <= 0)
            {
                ClosestPointFromPoint closestPoint = (pt, nodeId) => double.MaxValue;
                return ConvexBoundBuilder.Create(MonitoredFunction.Function, Compute, ConvexBound.Type.LoweBound, double.NegativeInfinity)
                                         .WithDistanceNorm(1, closestPoint)
                                         .WithDistanceNorm(2, closestPoint)
                                         .ToConvexBound();
            }

            var currentNorm = Compute(initialVector);
            var newPointNorm = threshold;
            var mulBy = Math.Sqrt(newPointNorm / currentNorm);
            var point = initialVector * mulBy;

            var constantPart  = -currentNorm;
            var parameters    = initialVector * 2;
            var lineHalfPlane = LineHalfPlane.Create(parameters, constantPart, threshold, Dimension);

            return lineHalfPlane.ToConvexLowerBound(MonitoredFunction.Function, threshold);
        }
    }
}
