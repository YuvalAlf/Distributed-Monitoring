using System.Numerics;
using MathNet.Numerics;

namespace Utils.MathUtils
{
    public static class ComplexUtils
    {
        public static bool IsNearReal(this Complex @this) => @this.Imaginary.AlmostEqual(0, 0.00000001);
    }
}
