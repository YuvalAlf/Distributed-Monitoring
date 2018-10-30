using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.MathUtils
{
    public static class RandomExtensions
    {
        public static (int, int) ChooseTwoDifferentRandomsInRange(this Random @this, int min, int max)
        {
            var first = @this.Next(min, max);
            while (true)
            {
                var second = @this.Next(min, max);
                if (first != second)
                    return (first, second);
            }
        }
    }
}
