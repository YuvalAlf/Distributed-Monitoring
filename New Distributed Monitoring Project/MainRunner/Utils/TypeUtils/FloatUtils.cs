using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public static class FloatUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRange(this float @this, float min, float max)
            => @this >= min && @this <= max;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InRange(this double @this, double min, double max)
            => @this >= min && @this <= max;
    }
}
