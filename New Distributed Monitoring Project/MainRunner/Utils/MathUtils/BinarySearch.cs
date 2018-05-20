using System;

namespace Utils.MathUtils
{
    public static class BinarySearch
    {
        public static double FindWhere(double minValue, double maxValue, Predicate<double> pointOk, double approximation)
        {
            while (true)
            {
                var middle = (maxValue + minValue) / 2.0;
                if (!pointOk(middle))
                {
                    maxValue = middle;
                    continue;
                }

                if (pointOk(middle + approximation))
                {
                    minValue = middle;
                    continue;
                }

                return middle;
            }
        }
    }
}
