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
using PCA;
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
        public static void RunBagOfWords(Random rnd)
        {
            var wordsPath = @"C:\Users\Yuval\Desktop\Distributed Data Sets\MostCommonEnglishWords.txt";
            var globalVectorType = GlobalVectorType.Average;
            var epsilon = new MultiplicativeEpsilon(0.02);
            int windowSize = 5000;
            var stepSize = 20;
            var path1 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\All Books Combined.txt";
            var path2 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\blogs\blogs.txt";
            var path3 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\India News\india-news-headlines.txt";
            var path4 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Jepordy\JEOPARDY_QUESTIONS1.txt";
            var path5 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\News Headlines\examiner-date-tokens.txt";
            int numOfNodes = 5;
            var resultPath = @"C:\Users\Yuval\Desktop\EntropyBOWResultCSV.csv";
            var amountOfIterations = 10000;
            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));
                for (int vectorLength = 50; vectorLength <= 50; vectorLength = (int)(vectorLength * 1.8))
                {
                    Console.WriteLine("Vector Length " + vectorLength);
                    var optionalStrings = new SortedSet<string>(File.ReadLines(wordsPath).Take(vectorLength).ToArray(),
                        StringComparer.OrdinalIgnoreCase);
                    using (var stringDataParser = DataParser<string>.Init(StreamReaderUtils.EnumarateWords, windowSize,
                        optionalStrings, path1, path2, path3, path4, path5))
                    {
                        var initVectors =
                            stringDataParser.Histograms.Map(h => h.CountVector().Multiply(1.0 / windowSize));
                        var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                            epsilon, EntropyFunction.MonitoredFunction, 1);
                        var changes = stringDataParser.AllCountVectors(stepSize)
                            .Select(vectors => vectors.Map(v => v.Multiply(1.0 / windowSize))).Take(amountOfIterations);
                        var results = multiRunner.RunAll(changes, rnd);
                        results.ForEach(r => resultCsvFile.WriteLine(r.AsCsvString()));
                        resultCsvFile.Flush();
                    }

                }
            }

            Process.Start(resultPath);
        }
    }
}
