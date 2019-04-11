using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Statistics;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.SparseTypes;

namespace SecondMomentSketch
{
    public partial class SecondMoment
    {
        public int Width { get; }
        public int Height { get; }
        public MonitoredFunction MonitoredFunction { get; }

        public SecondMoment(int width, int height)
        {
            Width = width;
            Height = height;
            MonitoredFunction = new MonitoredFunction(Compute, UpperBound, LowerBound, GlobalVectorType.Average, 2);
        }

        private IEnumerable<double> GetRowValues(Vector data, int row)
        {
            var baseIndex = row * Width;
            for (int i = 0; i < Width; i++)
                yield return data[baseIndex + i];
        }
        private double GetValue(Vector data, int row, int col)
        {
            return data[row * Width + col];
        }

        private double RowSquarredAverage(Vector data, int row) => GetRowValues(data, row).Select(x => x * x).Average();

        public double Compute(Vector vector)
        {
            return Enumerable.Range(0, Height).Select(row => RowSquarredAverage(vector, row)).Median();
        }
    }
}
