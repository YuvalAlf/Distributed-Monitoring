using System;

namespace Utils.MathUtils
{
    /*public static class BinarySearch
    {
        public static double FindWhere(double            minValue,
                                       double            maxValue,
                                       Predicate<double> pointOk,
                                       double            approximation)
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

        public static void GoUpIncreasing<T>(double            minValue,
                                             double            maxValue,
                                             Predicate<T>      pointOk,
                                             double            approximation,
                                             Action<T, double> move,
                                             T                 result,
                                             T                 nextResult,
                                             Func<T, T>        deepCopy,
                                             Action<T, T>      copyInPlace)
        {
            while (true)
            {
                var step = approximation;
                nextResult = deepCopy(result);
                move(nextResult, step);
                if (!pointOk(nextResult))
                    return;
                while (true)
                {
                    copyInPlace(nextResult, result);
                    step *= 2;
                    move(nextResult, step);
                    if (!pointOk(nextResult))
                        break;
                }
            }
        }
    }*/
}
