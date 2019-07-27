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
using Utils.AiderTypes;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace SecondMomentSketch
{
    public partial class SecondMoment
    {
        private ConvexBound LowerBound(Vector data, double threshold)
        {
            Debug.Assert(Height % 2 == 1);
            var halfHeight   = 1 + Height / 2;
            var rowToAverage = new Dictionary<int, double>(Height);
            for (int row = 0; row < Height; row++)
                rowToAverage[row] = RowSquarredAverage(data, row);
            var releventRows = rowToAverage.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).Take(halfHeight).ToArray();
            var rowToColToLine = new Dictionary<int, Dictionary<int, Line>>();
            foreach (var row in releventRows)
            {
                rowToColToLine.Add(row, new Dictionary<int, Line>(Width));
                for (int col = 0; col < Width; col++)
                {
                    var x = GetValue(data, row, col);
                    var y = x * x;
                    var m = 2 * x;
                    rowToColToLine[row].Add(col, Line.OfPointAndGradient(m,x,y));
                }   
            }


            Func<int, double> CalcAverageValueOfRow(Vector currentData) => row =>
            {
                var sum = 0.0;
                for (int col = 0; col < Width; col++)
                    sum += rowToColToLine[row][col].Compute(GetValue(currentData, row, col));
                return sum / Width;
            };

            double LowerBoundFunction(Vector currentData)
            {
                return releventRows.Select(CalcAverageValueOfRow(currentData)).Min();
            }
            var formulaDenominators = new Dictionary<int, double>(halfHeight);
            releventRows.ForEach(r => formulaDenominators.Add(r, Math.Sqrt(rowToColToLine[r].Values.Sum(l => l.M * l.M))));
           // releventRows.ForEach(r => formulaDenominators.Add(r, rowToColToLine[r].Values.Sum(l => l.M * l.M)));

            Either<Vector, double> DistanceL2(Vector point, int nodeId)
            {
               // if (threshold <= 0.0)
                    //return double.PositiveInfinity;

                double DistanceOfRow(int row)
                {
                    var residual = threshold - CalcAverageValueOfRow(point)(row);
                    var denominator = formulaDenominators[row];
                    if (denominator <= 0.0)
                        return double.PositiveInfinity;
                    return residual / denominator;
                }
                var distances = releventRows.Map(DistanceOfRow);
                if (distances.All(d => d <= 0.0))
                    return -distances.Max();
                else
                    return distances.Where(d => d > 0.0).Sum();
            }
            Either<Vector, double> DistanceL_Inf(Vector point, int nodeId)
            {
             //   if (threshold <= 0.0)
                   //return double.PositiveInfinity;

                double DistanceOfRow(int row)
                {
                    var residual = threshold - CalcAverageValueOfRow(point)(row);
                    var denominator = formulaDenominators[row];
                    if (denominator <= 0.0)
                        return double.PositiveInfinity;
                    return residual / denominator;
                }
                var distances = releventRows.Map(DistanceOfRow);
                if (distances.All(d => d <= 0.0))
                    return -distances.Max();
                else
                    return distances.Max();
            }

            return ConvexBoundBuilder.Create(MonitoredFunction.Function, LowerBoundFunction, value => value >= threshold)
                                     .WithDistanceNorm(2, DistanceL2)
                                     .WithDistanceNorm(0, DistanceL_Inf)
                                     .ToConvexBound();
        }
    }
}
