using System;

namespace TaxiTripsDataParsing
{
    public sealed class DataSplitter<T>
    {
        public Func<T, bool> IsY { get; }
        public string Name { get; }

        public DataSplitter(Func<T, bool> isY, string name)
        {
            IsY = isY;
            Name = name;
        }
    }
}
