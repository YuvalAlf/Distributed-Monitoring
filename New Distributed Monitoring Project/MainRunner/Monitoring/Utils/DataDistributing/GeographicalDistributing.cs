namespace Monitoring.Utils.DataDistributing
{
    public abstract class GeographicalDistributing
    {
        public int MinValue { get; }
        public int MaxValue { get; }
        public int NumOfNodes { get; }

        public abstract string Name { get; }
        public abstract int NodeOf(int value);

        protected GeographicalDistributing(int minValue, int maxValue, int numOfNodes)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            NumOfNodes = numOfNodes;
        }
    }
}
