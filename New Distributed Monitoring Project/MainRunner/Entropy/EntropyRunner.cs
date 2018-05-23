using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataParsing;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using MoreLinq;
using Utils.TypeUtils;

namespace Entropy
{
    public static class EntropyRunner
    {
        public static void RunChars(Random rnd)
        {
            var optionalChars = new SortedSet<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            var vectorLength = optionalChars.Count;
            var globalVectorType = GlobalVectorType.Average;
            var epsilon = new AdditiveEpsilon(0.05);
            int windowSize = 2000;
            var stepSize = 50;
            var path1 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\4 Harry Potter and the Goblet of Fire - 4.txt";
            var path2 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\Bible.txt";
            var path3 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\The-Oxford-Thesaurus-An-A-Z-Dictionary-Of-Synonyms.txt";
            var path4 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\Universal-History_Vol-I.txt";
            int numOfNodes = 4;
            var resultPath = @"C:\Users\Yuval\Desktop\ResultCSV.csv";

            using (var charDataParser = DataParser<char>.Init(StreamReaderUtils.EnumarateChars, windowSize, optionalChars, path1, path2, path3, path4))
            using (var resultCsvFile = File.CreateText(resultPath))
            {
                var initVectors = charDataParser.Histograms.Map(h => h.CountVector().Multiply(1.0 / windowSize));
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                    epsilon, EntropyFunction.MonitoredFunction, 1);

                resultCsvFile.WriteLine(multiRunner.HeaderCsv);
                int i = 0;
                while (charDataParser.Next(stepSize))
                {
                    var changes = charDataParser.Histograms.Map(h => h.ChangedCountVector().Multiply(1.0 / windowSize));
                    multiRunner.Run(changes, rnd).Select(result => result.AsCsvString())
                        .ForEach((Action<string>) resultCsvFile.WriteLine);
                    if (i++ % 50 == 0)
                        Console.WriteLine(i-1);
                    if (i == 5000)
                        break;
                }
            }

            Process.Start(resultPath);
        }
    }
}
