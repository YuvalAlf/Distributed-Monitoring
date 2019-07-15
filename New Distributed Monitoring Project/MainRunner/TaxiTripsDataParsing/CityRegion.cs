using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.MathUtils;
using Utils.TypeUtils;

namespace TaxiTripsDataParsing
{
    public sealed class CityRegion
    {
        public double MinLat { get; }
        public double MaxLat { get; }
        public double MinLong { get; }
        public double MaxLong { get; }

        public CityRegion(double minLat, double maxLat, double minLong, double maxLong)
        {
            MinLat = minLat;
            MaxLat = maxLat;
            MinLong = minLong;
            MaxLong = maxLong;
        }

        private Dictionary<int, (Line, Line)> Lines { get; } = new Dictionary<int, (Line, Line)>();
        public int Get(int sqrtAmount, double latatitude, double longtitude)
        {
            if (!Lines.ContainsKey(sqrtAmount))
                Lines[sqrtAmount] = (Line.OfTwoPoints(MinLat, 0, MaxLat, sqrtAmount),
                                     Line.OfTwoPoints(MinLong, 0, MaxLong, sqrtAmount));
            var (latitudeLine, longtitudeLine) = Lines[sqrtAmount];
            var col = ((int) latitudeLine.Compute(latatitude)).ToRange(0, sqrtAmount - 1);
            var row = ((int) longtitudeLine.Compute(longtitude)).ToRange(0, sqrtAmount - 1);
            return row * sqrtAmount + col;
        }
    }
}
