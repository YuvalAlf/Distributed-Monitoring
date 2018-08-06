using Utils.TypeUtils;

namespace Monitoring.Data
{
    public sealed class SingleResult
    {
        public int Bandwidth { get; }
        public int NumberOfMessages { get; }
        public bool IsFullSync { get; }
        public double FunctionValue { get; }
        public double UpperBound { get; }
        public double LowerBound { get; }
        public double[] NodesFunctionValues { get; }

        public SingleResult(int bandwidth, int numberOfMessages, bool isFullSync, double functionValue, double upperBound, double lowerBound, double[] nodesFunctionValues)
        {
            Bandwidth = bandwidth;
            NumberOfMessages = numberOfMessages;
            IsFullSync = isFullSync;
            FunctionValue = functionValue;
            UpperBound = upperBound;
            LowerBound = lowerBound;
            NodesFunctionValues = nodesFunctionValues.Copy();
        }


        public SingleResult CombineWith(SingleResult nextResult)
            => new SingleResult(
                this.Bandwidth + nextResult.Bandwidth,
                this.NumberOfMessages + nextResult.NumberOfMessages,
                this.IsFullSync || nextResult.IsFullSync,
                nextResult.FunctionValue,
                nextResult.UpperBound,
                nextResult.LowerBound,
                nextResult.NodesFunctionValues);
    }
}
