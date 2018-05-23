using System;
using System.Collections.Generic;
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
        public static void RunChars(Random rnd)
        {
            var optionalChars = new SortedSet<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
            var vectorLength = optionalChars.Count;
            var globalVectorType = GlobalVectorType.Sum;
            var epsilon = new AdditiveEpsilon(40000.0);
            int windowSize = 1500;
            var stepSize = 25;
            var path1 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\4 Harry Potter and the Goblet of Fire - 4.txt";
            var path2 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\Bible.txt";
            var path3 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\The-Oxford-Thesaurus-An-A-Z-Dictionary-Of-Synonyms.txt";
            var path4 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\Universal-History_Vol-I.txt";
            int numOfNodes = 4;
            var resultPath = @"C:\Users\Yuval\Desktop\ResultCSV.csv";

            using (var charDataParser = DataParser<char>.Init(StreamReaderUtils.EnumarateChars , windowSize, optionalChars, path1, path2, path3, path4))
            using (var resultCsvFile = File.CreateText(resultPath))
            {
                var initVectors = charDataParser.Histograms.Map(h => h.CountVector());
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                    epsilon, InnerProductFunction.MonitoredFunction, 2);

                resultCsvFile.WriteLine(multiRunner.HeaderCsv);
                while (charDataParser.Next(stepSize))
                {
                    var changes = charDataParser.Histograms.Map(h => h.ChangedCountVector());
                    multiRunner.Run(changes, rnd).Select(result => result.AsCsvString()).ForEach((Action<string>)resultCsvFile.WriteLine);
                }
            }

            Process.Start(resultPath);
        }
        public static void RunBagOfWords(Random rnd)
        {
            var wordsPath = @"C:\Users\Yuval\Desktop\Distributed Data Sets\MostCommonEnglishWords.txt";
            var vectorLength = 200;
            var optionalStrings = new SortedSet<string>(File.ReadLines(wordsPath).Take(vectorLength).ToArray(), StringComparer.OrdinalIgnoreCase);
            var globalVectorType = GlobalVectorType.Sum;
            var epsilon = new MultiplicativeEpsilon(0.1);
            int windowSize = 2000;
            var stepSize = 40;
            var path1 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\All Books Combined.txt";
            var path2 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\blogs\blogs.txt";
            var path3 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\India News\india-news-headlines.txt";
            var path4 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Jepordy\JEOPARDY_QUESTIONS1.txt";
            var path5 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\News Headlines\examiner-date-tokens.txt";
            int numOfNodes = 5;
            var resultPath = @"C:\Users\Yuval\Desktop\BOWResultCSV.csv";

            var i = 0;
            using (var charDataParser = DataParser<string>.Init(StreamReaderUtils.EnumarateWords, windowSize, optionalStrings, path1, path2, path3, path4, path5))
            using (var resultCsvFile = File.CreateText(resultPath))
            {
                var initVectors = charDataParser.Histograms.Map(h => h.CountVector());
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                    epsilon, InnerProductFunction.MonitoredFunction, 2);

                resultCsvFile.WriteLine(multiRunner.HeaderCsv);
                while (charDataParser.Next(stepSize))
                {
                    var changes = charDataParser.Histograms.Map(h => h.ChangedCountVector());
                    multiRunner.Run(changes, rnd).Select(result => result.AsCsvString()).ForEach((Action<string>)resultCsvFile.WriteLine);
                    if (i++ == 4000)
                        break;
                    if (i%100 == 0)
                        Console.WriteLine(i);
                }
            }

            Process.Start(resultPath);
        }
    }
}
