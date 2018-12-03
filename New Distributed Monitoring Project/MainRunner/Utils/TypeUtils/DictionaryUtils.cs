using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.TypeUtils
{
    public static class DictionaryUtils
    {
        public static (double maxValue, int key) MaxWithKey(this Dictionary<int, double> @this)
        {
            if (@this.Count == 0)
                throw new ArgumentException();

            int maxKey = 0;
            double maxValue = double.NegativeInfinity;

            foreach (var pair in @this)
            {
                var key = pair.Key;
                var value = pair.Value;
                if (value >= maxValue)
                {
                    maxKey = key;
                    maxValue = value;
                }
            }

            return (maxValue, maxKey);
        }
    }
}
