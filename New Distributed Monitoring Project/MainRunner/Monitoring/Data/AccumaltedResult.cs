using System.Globalization;
using System.Linq;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.MonitoringType;
using Utils.TypeUtils;

namespace Monitoring.Data
{
    public sealed class AccumaltedResult
    {
        public int Bandwidth { get; }
        public int NumberOfMessages { get; }
        public int NumberOfChannels { get; }
        public int NumberOfFullSyncs { get; }
        public double FunctionValue { get; }
        public double UpperBound { get; }
        public double LowerBound { get; }
        public double[] NodesFunctionValues { get; }

        public int LoopIndex { get; }

        public EpsilonType Epsilon { get; }
        public int NumOfNodes { get; }
        public int VectorLength { get; }
        public MonitoringScheme MonitoringScheme { get; }

        public static AccumaltedResult Init(EpsilonType epsilon, int numOfNodes, int vectorLength, MonitoringScheme monitoringScheme) 
            => new AccumaltedResult(0, 0, 0, 0, 0, 0, 0, new[]{0.0}, 0, epsilon, numOfNodes, vectorLength, monitoringScheme);

        public AccumaltedResult(int bandwidth, int numberOfMessages, int numberOfChannels, int numberOfFullSyncs, double functionValue, double upperBound, double lowerBound, double[] nodesFunctionValues, int loopIndex, EpsilonType epsilon, int numOfNodes, int vectorLength, MonitoringScheme monitoringScheme)
        {
            Bandwidth = bandwidth;
            NumberOfMessages = numberOfMessages;
            NumberOfChannels = numberOfChannels;
            NumberOfFullSyncs = numberOfFullSyncs;
            FunctionValue = functionValue;
            UpperBound = upperBound;
            LowerBound = lowerBound;
            NodesFunctionValues = nodesFunctionValues;
            LoopIndex = loopIndex;
            Epsilon = epsilon;
            NumOfNodes = numOfNodes;
            VectorLength = vectorLength;
            MonitoringScheme = monitoringScheme;
        }

        public static string Header(int numOfNodes) =>
            "LoopIndex"
                .ConcatCsv("MonitoringScheme")
                .ConcatCsv("VectorLength")
                .ConcatCsv("NumOfNodes")
                .ConcatCsv("Epsilon")
                .ConcatCsv("Bandwidth")
                .ConcatCsv("NumberOfMessages")
                .ConcatCsv("NumberOfChannels")
                .ConcatCsv("NumberOfFullSyncs")
                .ConcatCsv("LowerBound")
                .ConcatCsv("FunctionValue")
                .ConcatCsv("UpperBound")
                .ConcatCsv(Enumerable.Range(1, numOfNodes).Aggregate("", (csv, numNode) => csv.ConcatCsv("Node " + numNode)));

        public string HeaderCsv() => Header(NumOfNodes);

        public string AsCsvString() =>
            LoopIndex.ToString()
                .ConcatCsv(MonitoringScheme.AsString())
                .ConcatCsv(VectorLength.ToString())
                .ConcatCsv(NumOfNodes.ToString())
                .ConcatCsv(Epsilon.AsString())
                .ConcatCsv(Bandwidth.ToString())
                .ConcatCsv(NumberOfMessages.ToString())
                .ConcatCsv(NumberOfChannels.ToString())
                .ConcatCsv(NumberOfFullSyncs.ToString())
                .ConcatCsv(LowerBound.AsCsvString())
                .ConcatCsv(FunctionValue.ToString(CultureInfo.InvariantCulture))
                .ConcatCsv(UpperBound.AsCsvString())
                .ConcatCsv(NodesFunctionValues.Aggregate("", (csv, value) => csv.ConcatCsv(value.ToString(CultureInfo.InvariantCulture))));

        public AccumaltedResult AddSingleRsult(SingleResult singleResult) =>
            new AccumaltedResult(
                this.Bandwidth + singleResult.Bandwidth,
                this.NumberOfMessages + singleResult.NumberOfMessages,
                this.NumberOfMessages + singleResult.NumberOfMessages,
                this.NumberOfFullSyncs + (singleResult.IsFullSync ? 1 : 0),
                singleResult.FunctionValue,
                singleResult.UpperBound,
                singleResult.LowerBound,
                singleResult.NodesFunctionValues,
                this.LoopIndex + 1,
                this.Epsilon,
                this.NumOfNodes,
                this.VectorLength,
                this.MonitoringScheme);

    }
}
