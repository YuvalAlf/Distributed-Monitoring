using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Utils.TypeUtils;

namespace SecondMomentSketch
{
    public partial class SecondMoment
    {
        private ConvexBound LowerBound(Vector<double> data, double threshold)
        {
            return ConvexBoundBuilder.Create(_ => 0.0, value => value >= threshold)
                                     .WithDistanceNorm(2, (point, id) => 0.0)
                                     .ToConvexBound();

            /*Debug.Assert(Height % 2 == 1);
            var halfHeight   = Height / 2;
            var rowToAverage = new Dictionary<int, double>(Height);
            for (int row = 0; row < Height; row++)
                rowToAverage[row] = GetRowAverage(data, row);
            var releventRows = rowToAverage.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).Take(halfHeight).ToArray();

            double LowerBoundFunction(Vector<double> currentData)
            {
                return releventRows.Select(row => GetRowAverage(currentData, row)).Min();
            }


            Either<Vector<double>, double> DistanceL2(Vector<double> point, int nodeId)
            {
                return Width * (UpperBoundFunction(point) - threshold);
            }

            return ConvexBoundBuilder.Create(LowerBoundFunction, value => value <= threshold)
                                     .WithDistanceNorm(2, DistanceL2)
                                     .ToConvexBound();
            */
        }
    }
}
