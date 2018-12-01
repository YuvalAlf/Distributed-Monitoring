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


            Either<Vector<double>, double> DistanceL1(Vector<double> currentData, int nodeId)
            {
                var rowStatistics = new Dictionary<int, (double maxValue, double average)>(releventRows.Length);
                foreach(var row in releventRows)
                    rowStatistics[row] = (GetRowValues(currentData, row).Max(), RowSquarredAverage(currentData, row));

                double calcDistance(double maxValue, double rowSquarredAverage)
                {
                    return Math.Sqrt(Width * (threshold - rowSquarredAverage) + maxValue * maxValue) - maxValue;
                }

                return rowStatistics.Values.Select(r => Math.Abs(calcDistance(r.maxValue, r.average))).Min();
            }

            Either<Vector<double>, double> DistanceL2(Vector<double> currentData, int nodeId)
            {
                var rowStatistics = new Dictionary<int, double>(releventRows.Length);
                foreach (var row in releventRows)
                    rowStatistics[row] = RowSquarredAverage(currentData, row);

                var (maxAverage, maxRow) = rowStatistics.MaxWithKey();
                if (maxAverage <= 0.0)
                    return Math.Sqrt(threshold);

                var rowData = GetRowValues(currentData, maxRow).ToVector();
                var closestData = rowData * (Math.Sqrt(threshold / maxAverage));
                var value = closestData * closestData / closestData.Count;
                return closestData.DistL2FromVector()(rowData);
            }



            return ConvexBoundBuilder.Create(UpperBoundFunction, value => value <= threshold)
                                     .WithDistanceNorm(1, DistanceL1)
                                     .WithDistanceNorm(2, DistanceL2)
                                     .ToConvexBound();
        }
    }
}
