using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.MathUtils
{
    public static class RandomUtils
    {
        public static int GenLargePrime(this Random rnd)
        {
            while (true)
            {
                var randomNumber = rnd.Next(0x40000000, 0x7FFFFFFF);
                if (randomNumber.IsPrime())
                    return randomNumber;
            }
        }

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
