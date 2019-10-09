using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntropyMathematics;
using MathNet.Numerics;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Approximation;
using Monitoring.GeometricMonitoring.Running;
using MoreLinq.Extensions;
using Parsing;
using Utils.AiderTypes;
using Utils.TypeUtils;

namespace EntropySketch
{
    public static class EntropySketchRunner
    {
        public static void RunStocks(Random rnd, int iterations, Tree<long> closestValueQuery, int numOfNodes, int window, int collapseDimension, DateTime startingDateTime,
                                     int minAmountAtDay, ApproximationType approximation,
                                     string stocksDirPath, string resultDir)
        {
            var vectorLength = closestValueQuery.Data.Length;
            var resultPath =
                PathBuilder.Create(resultDir, "EntropySketch")
                           .AddProperty("Dataset", "Stocks")
                           .AddProperty("BucketLength", vectorLength.ToString())
                           .AddProperty("SketchDimension", collapseDimension.ToString())
                           .AddProperty("Nodes", numOfNodes.ToString())
                           .AddProperty("Window", window.ToString())
                           .AddProperty("StartingTime", startingDateTime.ToShortDateString().Replace('/', '-'))
                           .AddProperty("MinAmountAtDay", minAmountAtDay.ToString())
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");
            var entropySketch = new EntropySketchFunction(collapseDimension);

            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes)))
            using (var stocksProbabilityWindow = StocksProbabilityWindow.Init(stocksDirPath, startingDateTime, minAmountAtDay, numOfNodes, window, closestValueQuery))
            {
                var initProbabilityVectors = stocksProbabilityWindow.CurrentProbabilityVector().Map(entropySketch.CollapseProbabilityVector);
                var multiRunner = MultiRunner.InitAll(initProbabilityVectors, numOfNodes, collapseDimension,
                                                      approximation, entropySketch.MonitoredFunction);
                int i = 0;
                while (stocksProbabilityWindow.MoveNext() && (i++ < iterations))
                {
                    var changeProbabilityVectors = stocksProbabilityWindow.CurrentChangeProbabilityVector()
                                                                          .Map(entropySketch.CollapseProbabilityVector);

                    multiRunner.Run(changeProbabilityVectors, rnd, false)
                                .Select(r => r.AsCsvString())
                                .ForEach(resultCsvFile.WriteLine);
                }
            }
        }
    }
}
