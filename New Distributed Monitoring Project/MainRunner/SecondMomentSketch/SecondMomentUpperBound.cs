using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace SecondMomentSketch
{
    public partial class SecondMoment
    {
        private ConvexBound UpperBound(Vector data, double threshold)
        {
            var halfHeight = 1 + Height / 2;
            var rowToAverage = new Dictionary<int, double>(Height);
            for (int row = 0; row < Height; row++)
                rowToAverage[row] = RowSquarredAverage(data, row);
            var releventRows = rowToAverage.OrderBy(pair => pair.Value).Select(pair => pair.Key).Take(halfHeight).ToArray();

            double UpperBoundFunction(Vector currentData)
            {
                return releventRows.Select(row => RowSquarredAverage(currentData, row)).Max();
            }


#pragma warning disable CS8321 // Local function is declared but never used
            Either<Vector, double> DistanceL1(Vector currentData, int nodeId)
#pragma warning restore CS8321 // Local function is declared but never used
            {
                throw new NotImplementedException();
 /*               var rowStatistics = new Dictionary<int, (double maxValue, double average)>(releventRows.Length);
                foreach(var row in releventRows)
                    rowStatistics[row] = (GetRowValues(currentData, row).Max(), RowSquarredAverage(currentData, row));

                double calcDistance(double maxValue, double rowSquarredAverage)
                {
                    return Math.Sqrt(Width * (threshold - rowSquarredAverage) + maxValue * maxValue) - maxValue;
                }

                return rowStatistics.Values.Select(r => Math.Abs(calcDistance(r.maxValue, r.average))).Min();*/
            }

            Either<Vector, double> DistanceL2(Vector currentData, int nodeId)
            {
                double DistanceOfRow(int row)
                {
                    var rowValue = RowSquarredAverage(currentData, row);
                    if (rowValue <= 0.0)
                        return Math.Sqrt(threshold * Width);
                    var rowData = GetRowValues(currentData, row).ToVector();
                    var closestData = rowData * Math.Sqrt(threshold / rowValue);
                    var value = closestData * closestData / Width;
                    var t = threshold;
                    var mul = rowValue <= threshold ? -1 : 1;
                    return mul * closestData.DistL2FromVector()(rowData);
                }
                var distances = releventRows.Map(DistanceOfRow);
                if (distances.All(d => d <= 0.0))
                    return -distances.Max();
                else
                    return distances.Where(d => d > 0.0).Sum();
            }
            
            return ConvexBoundBuilder.Create(UpperBoundFunction, value => value <= threshold)
                                     .WithDistanceNorm(2, DistanceL2)
                                     .ToConvexBound();
        }
    }
}
