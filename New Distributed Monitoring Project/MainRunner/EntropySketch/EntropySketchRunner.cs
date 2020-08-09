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
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.Servers;
using MoreLinq.Extensions;
using Parsing;
using Utils.AiderTypes;
using Utils.TypeUtils;
using Utils.SparseTypes;

namespace EntropySketch
{
    public static class EntropySketchRunner
    {

        public static void RunCTU(Random rnd, int maxIterations, int numOfNodes, int window, int collapseDimension,
                                     ApproximationType approximation, string ctuBinaryPath, string resultDir)
        {
            var resultPath =
                PathBuilder.Create(resultDir, "EntropySketch")
                           .AddProperty("Dataset", "CTU")
                           .AddProperty("SketchDimension", collapseDimension.ToString())
                           .AddProperty("Nodes", numOfNodes.ToString())
                           .AddProperty("Window", window.ToString())
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");
            var entropySketch = new EntropySketchFunction(collapseDimension);
            var header = string.Join(",", Enumerable.Range(1, collapseDimension).Select(i => "y" + i)) + "," +
                         string.Join(",", Enumerable.Range(1, numOfNodes).Select(i => "server_max_" + i));
            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes) + ",Entropy," + header))
            using (var ctuProbabilityWindow = CtuProbabilityWindow.Init(ctuBinaryPath, numOfNodes, window))
            {
                var initProbabilityVectors = ctuProbabilityWindow.CurrentProbabilityVector().Map(entropySketch.CollapseProbabilityVector);
                var multiRunner = MultiRunner.InitAll(initProbabilityVectors, numOfNodes, collapseDimension,
                                                      approximation, entropySketch.MonitoredFunction);
                //multiRunner.OnlySchemes(new MonitoringScheme.Oracle());
                int i = 0;
                while (ctuProbabilityWindow.MoveNext() && (i++ < maxIterations))
                {
                    var entropy = Vector.AverageVector(ctuProbabilityWindow.CurrentProbabilityVector())
                                        .IndexedValues.Select(p => p.Value).Sum(v => -v * Math.Log(v));
                    var changeProbabilityVectors = ctuProbabilityWindow.CurrentChangeProbabilityVector()
                                                                          .Map(entropySketch.CollapseProbabilityVector);

                    multiRunner.Run(changeProbabilityVectors, rnd, false)
                                .Select(r =>
                                        {
                                            return r.AsCsvString();
                                            //var oracle = multiRunner.Runners.Values.OfType<MonitoringRunner<OracleServer>>().First().Server;
                                            //var nodesMax = string.Join(",", oracle.NodesVectors.Select(v => v[v.MaximumIndex()]));
                                            //var res = string.Join(",",oracle.GlobalVector.Enumerate(collapseDimension).Select(y => y.ToString()));
                                            //return r.AsCsvString() + "," + entropy.ToString() + "," + res + "," + nodesMax;
                                        })
                                .ForEach(resultCsvFile.WriteLine);
                }
            }
        }

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

            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes) + ",Entropy"))
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
                    var entropy = Vector.AverageVector(stocksProbabilityWindow.CurrentProbabilityVector())
                                        .IndexedValues.Values.Sum(p => -p * Math.Log(p));
                    multiRunner.Run(changeProbabilityVectors, rnd, false)
                                .Select(r => r.AsCsvString() + "," + entropy)
                                .ForEach(resultCsvFile.WriteLine);
                }
            }
        }
    }
}
