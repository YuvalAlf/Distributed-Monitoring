using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.MathUtils
{
    public sealed class Line
    {
        public double M { get; }
        public double N { get; }
        public double Compute(double x) => M * x + N;

        public Line(double m, double n)
        {
            M = m;
            N = n;
        }

        public static Line OfPointAndGradient(double m, double x, double y)
        {
            var n = -m * x + y;
            return new Line(m, n);
        }
    }
}
