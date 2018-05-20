using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataParsing;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using MoreLinq;
using Utils.TypeUtils;

namespace InnerProduct
{
    public static class InnerProductRunner
    {
        public static void Run(Random rnd)
        {
            var optionalChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var vectorLength = optionalChars.Distinct().Count();
            var globalVectorType = GlobalVectorType.Sum;
           // var epsilon = new AdditiveEpsilon(40000.0);
            var epsilon = new ThresholdEpsilon(100000.0);
            int windowSize = 1500;
            var stepSize = 25;
            var path1 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\4 Harry Potter and the Goblet of Fire - 4.txt";
            var path2 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\Bible.txt";
            var path3 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\The-Oxford-Thesaurus-An-A-Z-Dictionary-Of-Synonyms.txt";
            var path4 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\Universal-History_Vol-I.txt";
            int numOfNodes = 4;
            var resultPath = @"C:\Users\Yuval\Desktop\ResultCSV.csv";

            using (var charDataParser = CharDataParser.Init(windowSize, optionalChars, path1, path2, path3, path4))
            using (var resultCsvFile = File.CreateText(resultPath))
            {
                var initVectors = charDataParser.CharHistograms.Map(h => h.CountVector());
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                    epsilon, InnerProductFunction.MonitoredFunction, 2);

                resultCsvFile.WriteLine(multiRunner.HeaderCsv);
                while (charDataParser.Next(stepSize))
                {
                    var changes = charDataParser.CharHistograms.Map(h => h.ChangedCountVector());
                    multiRunner.Run(changes, rnd).Select(result => result.AsCsvString()).ForEach((Action<string>)resultCsvFile.WriteLine);
                }
            }

            Process.Start(resultPath);
        }
    }
}
