using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.MathUtils
{
    public static class PrimeNumbers
    {
        public static bool IsPrime(this int number)
        {
            if (number < 2)
                return false;
            if (number % 2 == 0)
                return number == 2;
            int root = (int)Math.Sqrt((double)number);
            for (int i = 3; i <= root; i += 2)
                if (number % i == 0)
                    return false;
            return true;
        }
    }
}
