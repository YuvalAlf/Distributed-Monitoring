using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public static class TupleUtils
    {
        public static (int, int, double) PointwiseAdd((int, int, double) a, (int, int, double) b)
        {
            return (a.Item1 + b.Item1, a.Item2 + b.Item2, a.Item3 + b.Item3);
        }

        public static (int, int, double) PointwiseAddKeepLast((int, int, double) a, (int, int, double) b)
        {
            return (a.Item1 + b.Item1, a.Item2 + b.Item2, b.Item3);
        }

        public static (int, int, double) PointwiseMul(int scalar, (int, int, double) a)
        {
            return (a.Item1 * scalar, a.Item2 * scalar, a.Item3 * scalar);
        }

        public static (int, int, double) PointwiseMulKeepLast(int scalar, (int, int, double) a)
        {
            return (a.Item1 * scalar, a.Item2 * scalar, a.Item3);
        }

        public static (int, int, double) Zeros()
        {
            return (0, 0, 0.0);
        }
    }
}
