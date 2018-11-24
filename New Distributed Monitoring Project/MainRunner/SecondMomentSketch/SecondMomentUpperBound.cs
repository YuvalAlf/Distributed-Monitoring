using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Utils.TypeUtils;

namespace SecondMomentSketch
{
    public partial class SecondMoment
    {
        private ConvexBound UpperBound(Vector<double> data, double threshold)
        {
            Debug.Assert(Height % 2 == 1);
            var halfHeight = Height / 2;
            var rowToAverage = new Dictionary<int, double>(Height);
            for (int row = 0; row < Height; row++)
                rowToAverage[row] = RowSquarredAverage(data, row);
            var releventRows = rowToAverage.OrderBy(pair => pair.Value).Select(pair => pair.Key).Take(halfHeight + 1).ToArray();

            double UpperBoundFunction(Vector<double> currentData)
            {
                return releventRows.Select(row => RowSquarredAverage(currentData, row)).Max();
            }


            Either<Vector<double>, double> DistanceL2(Vector<double> currentData, int nodeId)
            {
                var rowStatistics = new Dictionary<int, (double maxValue, double average)>(Height);
                for (int row = 0; row < Height; row++)
                    rowStatistics[row] = (GetRowValues(currentData, row).Max(), RowSquarredAverage(currentData, row));

                double calcDistance(double maxValue, double rowSquarredAverage)
                {
                    return Math.Sqrt(threshold + maxValue * maxValue - rowSquarredAverage) - maxValue;
                }

                return rowStatistics.Values.Select(r => Math.Abs(calcDistance(r.maxValue, r.average))).Min();
            }



            return ConvexBoundBuilder.Create(UpperBoundFunction, value => value <= threshold)
                                     .WithDistanceNorm(2, DistanceL2)
                                     .ToConvexBound();
        }
    }
}
