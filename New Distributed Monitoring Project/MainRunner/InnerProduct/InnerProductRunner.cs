using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataParsing;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using Monitoring.Nodes;
using Monitoring.Servers;
using MoreLinq;
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

        public static void RunBagOfWords(Random          rnd, int vectorLength, string wordsPath,
                                         string          resultDir,
                                         Func<int, bool> isLeft, string[] textFilesPathes)
        {
            var globalVectorType   = GlobalVectorType.Sum;
            var epsilon            = new MultiplicativeEpsilon(0.08);
            var numOfNodes         = textFilesPathes.Length;
            var windowSize         = 20000;
            var amountOfIterations = 2000;
            var stepSize           = 1000;
            var halfVectorLength = vectorLength / 2;
            var optionalWords      = File.ReadLines(wordsPath).Take(halfVectorLength).ToArray();
            var optionalStrings    = new SortedSet<string>(optionalWords, StringComparer.OrdinalIgnoreCase);
            var fileName =
                $"InnerProduct_VecSize_{vectorLength}_WindowSize_{windowSize}_Iters_{amountOfIterations}_StepSize_{stepSize}_Nodes_{textFilesPathes.Length}_Epsilon_{epsilon.EpsilonValue}.csv";
            var resultPath = Path.Combine(resultDir, fileName);

            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));

                using (var stringDataParser =
                    DataParser<string>.Init(StreamReaderUtils.EnumarateWords, windowSize, optionalStrings,
                                            textFilesPathes))
                {
                    var innerProduct = new InnerProductFunction(vectorLength);
                    var initVectors = stringDataParser.Histograms.Map(h => h.CountVector()).PadWithZeros(halfVectorLength, isLeft);
                    var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                                                          epsilon, innerProduct.MonitoredFunction);
                    var changes = stringDataParser.AllCountVectors(stepSize).Select(ch => ch.PadWithZeros(halfVectorLength, isLeft))
                                                  .Take(amountOfIterations);
                    multiRunner.RunAll(changes, rnd, true)
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>) resultCsvFile.WriteLine);
                }
            }

            Process.Start(resultPath);
        }
    }
}
