using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring;
using Utils.AiderTypes;
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

            Either<Vector, double> DistanceL2(Vector currentData, int nodeId)
            {
                double L2DistanceOfRow(int row)
                {
                    var rowValue = RowSquarredAverage(currentData, row);
                    if (rowValue <= 0.0)
                        return -Math.Sqrt(threshold * Width);
                    var rowData = GetRowValues(currentData, row).ToVector();
                    var closestData = rowData * Math.Sqrt(threshold / rowValue);
                    var value = closestData * closestData / Width;
                    var mul = rowValue <= threshold ? -1 : 1;
                    return mul * closestData.DistL2FromVector()(rowData);
                }
                var distances = releventRows.Map(L2DistanceOfRow);
                if (distances.All(d => d <= 0.0))
                    return -distances.Max();
                else
                    return distances.Where(d => d > 0.0).Sum();
            }

            Either<Vector, double> DistanceL_Inf(Vector currentData, int nodeId)
            {
                double DistanceOfRow(int row)
                {
                    var rowValue = RowSquarredAverage(currentData, row);
                    if (rowValue <= 0.0)
                        return -Math.Sqrt(threshold * Width);
                    var rowData = GetRowValues(currentData, row).ToVector();
                    var closestData = rowData * Math.Sqrt(threshold / rowValue);
                    var value = closestData * closestData / Width;
                    var mul = rowValue <= threshold ? -1 : 1;
                    return mul * closestData.DistL2FromVector()(rowData);
                }
                var distances = releventRows.Map(DistanceOfRow);
                if (distances.All(d => d <= 0.0))
                    return -distances.Max();
                else
                    return distances.Max();
            }
            
            return ConvexBoundBuilder.Create(MonitoredFunction.Function, UpperBoundFunction, ConvexBound.Type.UpperBound, threshold)
                                     .WithDistanceNorm(2, DistanceL2)
                                     .WithDistanceNorm(0, DistanceL_Inf)
                                     .ToConvexBound();
        }
    }
}
