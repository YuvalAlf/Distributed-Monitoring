using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using MoreLinq;
using Utils.MathUtils;
using Utils.TypeUtils;

namespace SecondMomentSketch
{
    public partial class SecondMoment
    {
        private ConvexBound LowerBound(Vector<double> data, double threshold)
        {
            Debug.Assert(Height % 2 == 1);
            var halfHeight   = 1 + Height / 2;
            var rowToAverage = new Dictionary<int, double>(Height);
            for (int row = 0; row < Height; row++)
                rowToAverage[row] = RowSquarredAverage(data, row);
            var releventRows = rowToAverage.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).Take(halfHeight).ToArray();
            var rowToLine = new Dictionary<int, Dictionary<int, Line>>();
            foreach (var row in releventRows)
            {
                rowToLine.Add(row, new Dictionary<int, Line>(Width));
                for (int col = 0; col < Width; col++)
                {
                    var x = GetValue(data, row, col);
                    var y = x * x;
                    var m = 2 * x;
                    rowToLine[row].Add(col, Line.OfPointAndGradient(m,x,y));
                }   
            }


            Func<int, double> CalcAverageValueOfRow(Vector<double> currentData) => row =>
            {
                var sum = 0.0;
                for (int col = 0; col < Width; col++)
                    sum += rowToLine[row][col].Compute(GetValue(currentData, row, col));
                return sum / Width;
            };

            double LowerBoundFunction(Vector<double> currentData)
            {
              //  var realValue = this.Compute(currentData);
              //  var lowerBound   = releventRows.Select(CalcAverageValueOfRow(currentData)).Min();

                return releventRows.Select(CalcAverageValueOfRow(currentData)).Min();
            }
            var sumGradientSquared = new Dictionary<int, double>(halfHeight);
            releventRows.ForEach(r => sumGradientSquared.Add(r, Math.Sqrt(rowToLine[r].Values.Sum(l => l.M * l.M))));

            Either<Vector<double>, double> DistanceL2(Vector<double> point, int nodeId)
            {
                double DistanceOfRow(int row)
                {
                    var residual = threshold - CalcAverageValueOfRow(point)(row);
                    var sqrt = sumGradientSquared[row];
                    if (sqrt <= 0.0)
                        return double.PositiveInfinity;
                    return Math.Abs(residual) / sqrt;
                }
                return releventRows.Select(DistanceOfRow).Min();
            }


            return ConvexBoundBuilder.Create(LowerBoundFunction, value => value >= threshold)
                                     .WithDistanceNorm(2, DistanceL2)
                                     .ToConvexBound();
        }
    }
}
