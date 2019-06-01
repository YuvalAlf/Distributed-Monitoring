using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataParsing;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.Approximation;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using Monitoring.Nodes;
using Monitoring.Servers;
using MoreLinq;
using Utils.AiderTypes;
using Utils.MathUtils.Sketches;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace InnerProduct
{
    public static class InnerProductRunner
    {
        private static Vector[] PadWithZeros(this Vector[] vectors, int amount, Func<int, bool> isLeft)
        {
            Vector PadLeft(Vector  v) => new Vector().Concat(v, amount);
            Vector PadRight(Vector v) => v;
            return vectors.Select((v, i) => isLeft(i) ? PadRight(v) : PadLeft(v)).ToArray();
        }

        public static void RunRandomly(Random rnd, int numOfNodes, ApproximationType approximation, int vectorLength, int iterations,
                                       string resultDir)
        {
            var windowSize         = vectorLength * 2;
            var stepSize           = windowSize / 5;
            var resultPath =
                PathBuilder.Create(resultDir, "InnerProduct")
                           .AddProperty("Dataset",      "Random")
                           .AddProperty("Nodes",        numOfNodes.ToString())
                           .AddProperty("VectorLength", vectorLength.ToString())
                           .AddProperty("Window",       windowSize.ToString())
                           .AddProperty("Iterations",   iterations.ToString())
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");

            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes)))
            {
                var innerProduct = new InnerProductFunction(vectorLength);
                var initVectors =
                    ArrayUtils.Init(numOfNodes,
                                    _ => ArrayUtils.Init(vectorLength, __ => rnd.NextDouble() * 10).ToVector());
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength,
                                                      approximation, innerProduct.MonitoredFunction);
                for (int i = 0; i < iterations; i++)
                {
                    var changes =
                        ArrayUtils.Init(numOfNodes,
                                        _ => ArrayUtils.Init(vectorLength, __ => rnd.NextDouble() - 0.5).ToVector());
                    multiRunner.Run(changes, rnd, false)
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>) resultCsvFile.WriteLine);
                }
            }

        }


        public static void RunBagOfWords(Random          rnd, int vectorLength, string wordsPath,
                                         ApproximationType approximation,
                                         string          resultDir,
                                         Func<int, bool> isLeft, string[] textFilesPathes)
        {
            var numOfNodes         = textFilesPathes.Length;
            var windowSize         = 20000;
            var amountOfIterations = 2000;
            var stepSize           = 1000;
            var halfVectorLength   = vectorLength / 2;
            var optionalWords      = File.ReadLines(wordsPath).Take(halfVectorLength).ToArray();
            var optionalStrings    = new SortedSet<string>(optionalWords, StringComparer.OrdinalIgnoreCase);
            var resultPath =
                PathBuilder.Create(resultDir, "InnerProduct")
                           .AddProperty("Dataset",      "BagOfWords")
                           .AddProperty("Nodes",        numOfNodes.ToString())
                           .AddProperty("VectorLength", vectorLength.ToString())
                           .AddProperty("Window",       windowSize.ToString())
                           .AddProperty("Iterations",   amountOfIterations.ToString())
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");

            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes)))
            using (var stringDataParser =
                TextParser<string>.Init(StreamReaderUtils.EnumarateWords, windowSize, optionalStrings,
                                        textFilesPathes))
            {
                var innerProduct = new InnerProductFunction(vectorLength);
                var initVectors = stringDataParser.Histograms.Map(h => h.CountVector())
                                                  .PadWithZeros(halfVectorLength, isLeft);
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength,
                                                      approximation, innerProduct.MonitoredFunction);
                var changes = stringDataParser.AllCountVectors(stepSize)
                                              .Select(ch => ch.PadWithZeros(halfVectorLength, isLeft))
                                              .Take(amountOfIterations);
                multiRunner.RunAll(changes, rnd, true)
                           .Select(r => r.AsCsvString())
                           .ForEach((Action<string>) resultCsvFile.WriteLine);
            }

        }
    }
}
