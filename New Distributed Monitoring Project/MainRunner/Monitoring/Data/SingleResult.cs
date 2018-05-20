namespace Monitoring.Data
{
    public sealed class SingleResult
    {
        public int Bandwidth { get; }
        public int NumberOfMessages { get; }
        public int NumberOfChannels { get; }
        public bool IsFullSync { get; }
        public double FunctionValue { get; }
        public double UpperBound { get; }
        public double LowerBound { get; }
        public double[] NodesFunctionValues { get; }

        public SingleResult(int bandwidth, int numberOfMessages, int numberOfChannels, bool isFullSync, double functionValue, double upperBound, double lowerBound, double[] nodesFunctionValues)
        {
            Bandwidth = bandwidth;
            NumberOfMessages = numberOfMessages;
            NumberOfChannels = numberOfChannels;
            IsFullSync = isFullSync;
            FunctionValue = functionValue;
            UpperBound = upperBound;
            LowerBound = lowerBound;
            NodesFunctionValues = nodesFunctionValues;
        }

        public static SingleResult Nothing(double functionValue, double lowerBound, double upperBound, double[] nodesFunctionValues) 
            => new SingleResult(0, 0, 0, false, functionValue, upperBound, lowerBound, nodesFunctionValues);

        public SingleResult CombineWith(SingleResult nextResult)
            => new SingleResult(
                this.Bandwidth + nextResult.Bandwidth,
                this.NumberOfMessages + nextResult.NumberOfMessages,
                this.NumberOfChannels + nextResult.NumberOfChannels,
                this.IsFullSync || nextResult.IsFullSync,
                nextResult.FunctionValue,
                nextResult.UpperBound,
                nextResult.LowerBound,
                nextResult.NodesFunctionValues);
    }
}
