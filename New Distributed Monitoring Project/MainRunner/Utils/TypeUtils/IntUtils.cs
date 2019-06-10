using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public static class IntUtils
    {
        public static int ClosestPowerOf2FromBelow(this int @this)
        {
            int num = 1;
            var half = @this / 2;
            while (num <= half)
                num *= 2;
            return num;
        }

        public static int ToRange(this int @this, int min, int max)
        {
            if (@this < min)
                return min;
            if (@this > max)
                return max;
            return @this;
        }
    }
}
