using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public static class TupleUtils
    {
        public static (int, int) PointwiseAdd((int, int) a, (int, int) b)
        {
            return (a.Item1 + b.Item1, a.Item2 + b.Item2);
        }

        public static (int, int) PointwiseMul(int scalar, (int, int) a)
        {
            return (a.Item1 * scalar, a.Item2 * scalar);
        }

        public static (int, int) Zeros()
        {
            return (0, 0);
        }
    }
}
