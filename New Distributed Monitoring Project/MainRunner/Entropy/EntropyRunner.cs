using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Entropy
{
    public static class EntropyRunner
    {
        public static void RunBagOfWords(Random rnd, string wordsPath, string resultPath, string[] textFilesPathes)
        {
            var globalVectorType   = GlobalVectorType.Average;
            var epsilon            = new MultiplicativeEpsilon(0.015);
            var numOfNodes         = textFilesPathes.Length;
            var windowSize         = 10000;
            var amountOfIterations = 5000;
            var vectorLength       = 200;
            var stepSize           = 200;
            var optionalWords      = File.ReadLines(wordsPath).Take(vectorLength).ToArray();
            var optionalStrings    = new SortedSet<string>(optionalWords, StringComparer.OrdinalIgnoreCase);

            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));

                using (var stringDataParser = DataParser<string>.Init(StreamReaderUtils.EnumarateWords, windowSize, optionalStrings, textFilesPathes))
                {
                    var initVectors = stringDataParser.Histograms.Map(h => h.CountVector() / windowSize);
                    var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                                                          epsilon, EntropyFunction.MonitoredFunction);
                    var changes = stringDataParser.AllCountVectors(stepSize).Select(c => c.Map(v => v / windowSize)).Take(amountOfIterations);
                    multiRunner.RunAll(changes, rnd, false)
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>)resultCsvFile.WriteLine);
                }
            }

            Process.Start(resultPath);
        }
    }
}
