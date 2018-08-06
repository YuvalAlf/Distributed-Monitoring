using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataParsing;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using Monitoring.Nodes;
using Monitoring.Servers;
using MoreLinq;
using Utils.TypeUtils;

namespace InnerProduct
{
    public static class InnerProductRunner
    {
        public static void RunBagOfWords(Random rnd, string wordsPath, string resultPath, string[] textFilesPathes)
        {
            var globalVectorType   = GlobalVectorType.Sum;
            var epsilon            = new MultiplicativeEpsilon(0.08);
            var numOfNodes         = textFilesPathes.Length;
            var windowSize         = 50000;
            var amountOfIterations = 1000;
            var vectorLength       = 1000;
            var stepSize           = 100;

            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));
                var optionalStrings = new SortedSet<string>(File.ReadLines(wordsPath).Take(vectorLength).ToArray(),
                                                            StringComparer.OrdinalIgnoreCase);
                using (var stringDataParser = 
                    DataParser<string>.Init(StreamReaderUtils.EnumarateWords, windowSize, optionalStrings, textFilesPathes))
                {
                    var initVectors = stringDataParser.Histograms.Map(h => h.CountVector());
                    var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                                                          epsilon, InnerProductFunction.MonitoredFunction, 2);
                    var changes = stringDataParser.AllCountVectors(stepSize).Take(amountOfIterations);
                    multiRunner.RunAll(changes, rnd, false)
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>)resultCsvFile.WriteLine);
                }
            }

            Process.Start(resultPath);
        }

       /* public static void CompareToArnonBow(Random rnd)
        {
            var wordsPath = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Arnon\mostCommomWords.txt";
            var tweetsPath = @"C:\Users\Yuval\Desktop\Thesis Data Arnon\Arnon InnerProd ConvexBound\tweets\tweets_tfvec_sorted_5M.csv";
            var globalVectorType = GlobalVectorType.Sum;
            var epsilon = new ThresholdEpsilon(11000);
            var windowSize = 1000;
            var numOfNodes = 10;
            var resultPath = @"C:\Users\Yuval\Desktop\ArnonComparison.csv";
            var optionalStrings = File.ReadLines(wordsPath).ToArray();
            var vectorLength = optionalStrings.Length;
            var numOfIterations = 10000;
            using (var resultCsvFile = File.CreateText(resultPath))
            using (var tweetsFile = File.OpenText(tweetsPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));
                var arnonParser = ArnonDataParser.Init(tweetsFile, windowSize, optionalStrings, numOfNodes);
                var initVectors = arnonParser.Histograms.Map(ch => ch.CountVector());
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                      epsilon, InnerProductFunction.MonitoredFunction, 2);
                {
                    for (int i = 0; i < numOfIterations; i++)
                    {
                        var change = arnonParser.NextChange(out bool finished);
                        if (finished)
                            break;
                        multiRunner.Run(change, rnd, false).ForEach(a => resultCsvFile.WriteLine(a.AsCsvString()));
                        if (i % 10 == 0)
                            Console.WriteLine("{0}%",100.0 * i / numOfIterations);
                    }
                }
            }

            Process.Start(resultPath);
        } */
    }
}
