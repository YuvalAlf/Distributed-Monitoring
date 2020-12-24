using System;
using Utils.TypeUtils;

namespace Monitoring.Data
{
    public sealed class SingleResult
    {
        public Communication Communication { get; }
        public bool IsFullSync { get; }
        public double FunctionValue { get; }
        public double UpperBound { get; }
        public double LowerBound { get; }
        public double[] NodesFunctionValues { get; }

        public SingleResult(Communication communication, bool isFullSync, double functionValue, double upperBound, double lowerBound, double[] nodesFunctionValues)
        {
            Communication = communication;
            IsFullSync = isFullSync;
            FunctionValue = functionValue;
            UpperBound = upperBound;
            LowerBound = lowerBound;
            NodesFunctionValues = nodesFunctionValues.Copy();
        }


        /*public SingleResult CombineWith(SingleResult nextResult)
            => new SingleResult(
                this.Communication.Add(nextResult.Communication),
                this.IsFullSync || nextResult.IsFullSync,
                nextResult.FunctionValue,
                nextResult.UpperBound,
                nextResult.LowerBound,
                nextResult.NodesFunctionValues);*/
    }
}
