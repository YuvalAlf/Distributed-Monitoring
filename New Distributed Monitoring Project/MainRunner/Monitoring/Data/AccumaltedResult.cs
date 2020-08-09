using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Approximation;
using Monitoring.GeometricMonitoring.MonitoringType;
using Utils.TypeUtils;

namespace Monitoring.Data
{
    public sealed class AccumaltedResult
    {
        public long Bandwidth { get; }
        public int NumberOfMessages { get; }
        public int NumberOfFullSyncs { get; }
        public double FunctionValue { get; }
        public double UpperBound { get; }
        public double LowerBound { get; }
        public double[] NodesFunctionValues { get; }

        public int LoopIndex { get; }

        public ApproximationType Approximation { get; }
        public int NumOfNodes { get; }
        public int VectorLength { get; }
        public MonitoringScheme MonitoringScheme { get; }

        public static AccumaltedResult Init(ApproximationType approximation, int numOfNodes, int vectorLength, MonitoringScheme monitoringScheme) 
            => new AccumaltedResult(0, 0, 0, 0, 0, 0, new[]{0.0}, 0, approximation, numOfNodes, vectorLength, monitoringScheme);

        public AccumaltedResult(long bandwidth, int numberOfMessages, int numberOfFullSyncs, double functionValue, double upperBound, double lowerBound, double[] nodesFunctionValues, int loopIndex, ApproximationType approximation, int numOfNodes, int vectorLength, MonitoringScheme monitoringScheme)
        {
            Bandwidth = bandwidth;
            NumberOfMessages = numberOfMessages;
            NumberOfFullSyncs = numberOfFullSyncs;
            FunctionValue = functionValue;
            UpperBound = upperBound;
            LowerBound = lowerBound;
            NodesFunctionValues = nodesFunctionValues;
            LoopIndex = loopIndex;
            Approximation = approximation;
            NumOfNodes = numOfNodes;
            VectorLength = vectorLength;
            MonitoringScheme = monitoringScheme;
        }

        public static string Header(int numOfNodes) =>
            "LoopIndex"
               .ConcatCsv("Monitoring Scheme")
               .ConcatCsv("Vector Length")
               .ConcatCsv("# Nodes")
               .ConcatCsv("Approximation")
               .ConcatCsv("Bandwidth")
               .ConcatCsv("# Messages")
               .ConcatCsv("# Full Syncs")
               .ConcatCsv("Lower-Bound")
               .ConcatCsv("Function's Value")
               .ConcatCsv("Upper-Bound")
               .ConcatCsv(Enumerable.Range(1, numOfNodes).Aggregate("", (csv, numNode) => csv.ConcatCsv("Server " + numNode)));

        public string HeaderCsv() => Header(NumOfNodes);

        public string AsCsvString() =>
            LoopIndex.ToString()
                .ConcatCsv(MonitoringScheme.AsString())
                .ConcatCsv(VectorLength.ToString())
                .ConcatCsv(NumOfNodes.ToString())
                .ConcatCsv(Approximation.AsString())
                .ConcatCsv(Bandwidth.ToString())
                .ConcatCsv(NumberOfMessages.ToString())
                .ConcatCsv(NumberOfFullSyncs.ToString())
                .ConcatCsv(LowerBound.AsCsvString())
                .ConcatCsv(FunctionValue.ToString(CultureInfo.InvariantCulture))
                .ConcatCsv(UpperBound.AsCsvString())
                .ConcatCsv(NodesFunctionValues.Aggregate("", (csv, value) => csv.ConcatCsv(value.ToString(CultureInfo.InvariantCulture))));

        public AccumaltedResult AddSingleRsult(SingleResult singleResult) =>
            new AccumaltedResult(
                this.Bandwidth + singleResult.Bandwidth,
                this.NumberOfMessages + singleResult.NumberOfMessages,
                this.NumberOfFullSyncs + (singleResult.IsFullSync ? 1 : 0),
                singleResult.FunctionValue,
                singleResult.UpperBound,
                singleResult.LowerBound,
                singleResult.NodesFunctionValues,
                this.LoopIndex + 1,
                this.Approximation,
                this.NumOfNodes,
                this.VectorLength,
                this.MonitoringScheme);

    }
}
