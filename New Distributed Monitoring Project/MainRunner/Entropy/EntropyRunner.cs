﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DataParsing;
using DataParsing.Databse;
using EntropyMathematics;
using EntropySketch;
using MathNet.Numerics;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Approximation;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using Monitoring.Nodes;
using Monitoring.Servers;
using MoreLinq;
using Parsing;
using SecondMomentSketch;
using SecondMomentSketch.Hashing;
using Utils.AiderTypes;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Entropy
{
    public static class EntropyRunner
    {

        public static void RunRandomly(Random rnd, int numOfNodes, ApproximationType approximation, int vectorLength, int collapeDimension, int iterations, bool oneChanges,
                                       string resultDir)
        {
            var entropyResultPath =
                PathBuilder.Create(resultDir, "Entropy")
                           .AddProperty("Dataset", "Random")
                           .AddProperty("Nodes", numOfNodes.ToString())
                           .AddProperty("VectorLength", vectorLength.ToString())
                           .AddProperty("OneChanges", oneChanges.ToString())
                           .AddProperty("Iterations", iterations.ToString())
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");
            var sketchEntropyResultPath =
                PathBuilder.Create(resultDir, "SketchEntropy")
                           .AddProperty("Dataset", "Random")
                           .AddProperty("Nodes", numOfNodes.ToString())
                           .AddProperty("VectorLength", collapeDimension.ToString())
                           .AddProperty("OneChanges", oneChanges.ToString())
                           .AddProperty("Iterations", iterations.ToString())
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");

            using (var entropyResultCsvFile = AutoFlushedTextFile.Create(entropyResultPath, AccumaltedResult.Header(numOfNodes)))
            using (var sketchEntropyResultCsvFile = AutoFlushedTextFile.Create(sketchEntropyResultPath, AccumaltedResult.Header(numOfNodes)))
            {
                var sqrt = (int)Math.Sqrt(vectorLength);
                var entropyFunction = new EntropyFunction(vectorLength);
                var entropySketchFunction = new EntropySketchFunction(collapeDimension);

                Func<int, Vector> genVector = node => ArrayUtils.Init(vectorLength, rnd.NextDouble())
                                                     .ToVector().NormalizeInPlaceToSum(1.0);

                var initVectors = ArrayUtils.Init(numOfNodes, genVector);
                var sketchInitVectors = initVectors.Map(entropySketchFunction.CollapseProbabilityVector);
                var globalVector = Vector.AverageVector(initVectors);

                Vector getChangeVector(int node)
                {
                    if (oneChanges && node >= 1)
                        return new Vector();

                    var someVector = genVector(node);
                    Vector changeVector = someVector - globalVector;
                    if (!oneChanges)
                        changeVector.DivideInPlace(numOfNodes);

                    globalVector.AddInPlace(changeVector / numOfNodes);
                    return changeVector;
                }


                var entropyMultiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength,
                                                      approximation, entropyFunction.MonitoredFunction);
                var sketchEntropyMultiRunner = MultiRunner.InitAll(sketchInitVectors, numOfNodes, vectorLength,
                                                      approximation, entropySketchFunction.MonitoredFunction);
                var shouldStop = new StrongBox<bool>(false);
                for (int i = 0; i < iterations; i++)
                {
                    var changes = ArrayUtils.Init(numOfNodes, getChangeVector);
                    var sketchChanges = changes.Map(entropySketchFunction.CollapseProbabilityVector);

//entropyMultiRunner.Run(changes, rnd, false)
//.SideEffect(r => shouldStop.Value = shouldStop.Value || (r.MonitoringScheme is MonitoringScheme.Oracle && r.NumberOfFullSyncs >= 8))
//.Select(r => r.AsCsvString())
//.ForEach((Action<string>)entropyResultCsvFile.WriteLine);

                    sketchEntropyMultiRunner.Run(sketchChanges, rnd, false)
                               .SideEffect(r => shouldStop.Value = shouldStop.Value || (r.MonitoringScheme is MonitoringScheme.Oracle && r.NumberOfFullSyncs >= 8))
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>)sketchEntropyResultCsvFile.WriteLine);
                    if (shouldStop.Value)
                        break;
                }
            }
        }



        public static void RunStocks(Random rnd, MonitoredFunction function, string cbName, int iterations, Tree<long> closestValueQuery, int numOfNodes, int window, DateTime startingDateTime,
                                     int minAmountAtDay , ApproximationType approximation,
                                     string stocksDirPath, string resultDir)
        {
            var vectorLength = closestValueQuery.Data.Length;
            var resultPath =
                PathBuilder.Create(resultDir, "Entropy_" + cbName)
                           .AddProperty("Dataset", "Stocks")
                           .AddProperty("VectorLength", vectorLength.ToString())
                           .AddProperty("Nodes", numOfNodes.ToString())
                           .AddProperty("Window", window.ToString())
                           .AddProperty("StartingTime", startingDateTime.ToShortDateString().Replace('/','-'))
                           .AddProperty("MinAmountAtDay", minAmountAtDay.ToString())
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");

            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes)))
            using (var stocksProbabilityWindow = StocksProbabilityWindow.Init(stocksDirPath, startingDateTime, minAmountAtDay, numOfNodes, window, closestValueQuery))
            {
                var initProbabilityVectors = stocksProbabilityWindow.CurrentProbabilityVector();
                if (!initProbabilityVectors.All(v => v.Sum().AlmostEqual(1.0, 0.000001)))
                    throw new Exception();
                var multiRunner = MultiRunner.InitAll(initProbabilityVectors, numOfNodes, vectorLength,
                                                      approximation, function);
                int i = 0;
                while (stocksProbabilityWindow.MoveNext() && (i++ < iterations))
                {
                    var changeProbabilityVectors = stocksProbabilityWindow.CurrentChangeProbabilityVector();

                    if (!changeProbabilityVectors.All(v => v.Sum().AlmostEqual(0.0, 0.000001)))
                        throw new Exception();
                    multiRunner.Run(changeProbabilityVectors, rnd, false)
                                .Select(r => r.AsCsvString())
                                .ForEach(resultCsvFile.WriteLine);
                }
            }
        }


        public static void RunDatabaseAccesses(Random      rnd, int numOfNodes,
                                               int         window,
                                               ApproximationType approximation, int vectorLength,
                                               UsersDistributing distributing,
                                               string      databaseAccessesPath, string            resultDir)
        {
            var resultPath =
                PathBuilder.Create(resultDir, "Entropy")
                           .AddProperty("Dataset",            "DatabaseAccesses")
                           .AddProperty("Nodes",              numOfNodes.ToString())
                           .AddProperty("Window",             window.ToString())
                           .AddProperty("Approximation",            approximation.AsString())
                           .AddProperty("DistributingMethod", distributing.Name)
                           .ToPath("csv");
            var hashUser      = new Func<int, int>(userId => userId % numOfNodes);
            var entropy = new EntropyFunction(vectorLength);

            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes)))
            using (var databaseAccessesStatistics = DatabaseAccessesStatistics.Init(databaseAccessesPath, numOfNodes, window, distributing.DistributeFunc))
            {
                var initProbabilityVectors = databaseAccessesStatistics.InitProbabilityVectors();
                if (!initProbabilityVectors.All(v => v.Sum().AlmostEqual(1.0, 0.000001)))
                    throw new Exception();
                var multiRunner = MultiRunner.InitAll(initProbabilityVectors, numOfNodes, vectorLength,
                                                      approximation, entropy.MonitoredFunction);
                while (databaseAccessesStatistics.TakeStep())
                {
                    var changeProbabilityVectors = databaseAccessesStatistics.GetChangeProbabilityVectors();

                    if (!changeProbabilityVectors.All(v => v.Sum().AlmostEqual(0.0, 0.000001)))
                        throw new Exception();
                    multiRunner.Run(changeProbabilityVectors, rnd, true)
                               .Select(r => r.AsCsvString())
                               .ForEach(resultCsvFile.WriteLine);
                }
            }
        }
   
    }
}
