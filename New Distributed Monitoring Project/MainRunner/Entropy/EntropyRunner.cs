using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParsing;
using MathNet.Numerics;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using Monitoring.Nodes;
using Monitoring.Servers;
using MoreLinq;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Entropy
{
    public static class EntropyRunner
    {
        public static void RunBagOfWords(Random rnd, int vectorLength, string wordsPath, string resultDir, string[] textFilesPathes)
        {
            var globalVectorType   = GlobalVectorType.Average;
            var epsilon            = new MultiplicativeEpsilon(0.012);
            var numOfNodes         = textFilesPathes.Length;
            var windowSize         = 16384;
            var amountOfIterations = 1000;
            var stepSize           = 1024;
            var optionalWords      = File.ReadLines(wordsPath).Take(vectorLength).ToArray();
            var optionalStrings    = new SortedSet<string>(optionalWords, StringComparer.OrdinalIgnoreCase);
            var fileName = $"Entropy_VecSize_{vectorLength}_WindowSize_{windowSize}_Iters_{amountOfIterations}_StepSize_{stepSize}_Nodes_{textFilesPathes.Length}_Epsilon_{epsilon.EpsilonValue}.csv";
            var resultPath = Path.Combine(resultDir, fileName);

            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));

                using (var stringDataParser = TextParser<string>.Init(StreamReaderUtils.EnumarateWords, windowSize, optionalStrings, textFilesPathes))
                {
                    var entropy = new EntropyFunction(vectorLength);
                    var initVectors = stringDataParser.Histograms.Map(h => h.CountVector() / windowSize);
                    var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                                                          epsilon, entropy.MonitoredFunction);
                    var changes = stringDataParser.AllCountVectors(stepSize).Select(c => c.Map(v => v / windowSize)).Take(amountOfIterations);
                    multiRunner.RunAll(changes, rnd, false)
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>)resultCsvFile.WriteLine);
                }
            }

            Process.Start(resultPath);
        }

        public static void RunDatabaseAccesses(Random rnd, int numOfNodes, double epsilonValue, int maxVectorLength, string databaseAccessesPath, string resultDir)
        {
            var globalVectorType = GlobalVectorType.Average;
            var epsilon = new MultiplicativeEpsilon(epsilonValue);
            var fileName = $"Entropy_Database_Accesses_Nodes_{numOfNodes}_Epsilon_{epsilonValue}_MaxVector_{maxVectorLength}.csv";
            var resultPath = Path.Combine(resultDir, fileName);
            var hashUser = new Func<int, int>(userId => userId % numOfNodes);
            var maxIterations = 100000;

            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));

                using (var databaseReader = DatabaseAccessesParser.Init(databaseAccessesPath, maxVectorLength))
                {
                    bool didEnd;
                    var vectorLength = databaseReader.VectorLength + 1;
                    var entropy = new EntropyFunction(vectorLength);
                    var initVectors = databaseReader.TakeStep(numOfNodes, hashUser, vectorLength - 1, out didEnd).Map(v => v / v.Sum());
                    var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                                                          epsilon, entropy.MonitoredFunction);
                    var lastStep = initVectors;
                    for (int i = 0; i < maxIterations; i++)
                    {
                        if (didEnd)
                            break;
                        var step = databaseReader.TakeStep(numOfNodes, hashUser, vectorLength - 1, out didEnd).Map(v => v / v.Sum());
                        var change = step.Zip(lastStep, (v1, v2) => v1 - v2).ToArray();
                        lastStep = step;
                        multiRunner.Run(change, rnd, false)
                                   .Select(r => r.AsCsvString())
                                   .ForEach((Action<string>)resultCsvFile.WriteLine);
                    }
                }
            }

            Process.Start(resultPath);
        }
    }
}
