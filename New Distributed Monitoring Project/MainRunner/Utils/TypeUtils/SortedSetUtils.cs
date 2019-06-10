using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public static class SortedSetUtils
    {
        public static T ExtractMin<T>(this SortedSet<T> @this)
        {
            var min = @this.Min;
            @this.Remove(min);
            return min;
        }
    }
}
