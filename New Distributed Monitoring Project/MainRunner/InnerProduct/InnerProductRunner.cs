using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DataParsing;
using Monitoring.Data;
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
                    multiRunner.Run(changes, rnd, false).Select(result => result.AsCsvString()).ForEach((Action<string>)resultCsvFile.WriteLine);
                }
            }

            Process.Start(resultPath);
        }
        public static void RunBagOfWords(Random rnd)
        {
            var wordsPath = @"C:\Users\Yuval\Desktop\Distributed Data Sets\MostCommonEnglishWords.txt";
            var globalVectorType = GlobalVectorType.Sum;
            var epsilon = new MultiplicativeEpsilon(0.12);
            int windowSize = 50000;
            var stepSize = 100;
            var path1 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\All Books Combined.txt";
            var path2 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\blogs\blogs.txt";
            var path3 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\India News\india-news-headlines.txt";
            var path4 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Jepordy\JEOPARDY_QUESTIONS1.txt";
            var path5 = @"C:\Users\Yuval\Desktop\Distributed Data Sets\News Headlines\examiner-date-tokens.txt";
            int numOfNodes = 5;
            var resultPath = @"C:\Users\Yuval\Desktop\5200InnerBOWResultCSVResults.csv";
            
            var amountOfIterations = 10000;
            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));
                for (int vectorLength = 5200; vectorLength <= 10000; vectorLength += 100)
                {
                    if (vectorLength % 2 == 1)
                        vectorLength += 1;
                    Console.WriteLine("Vector Length " + vectorLength);
                    var optionalStrings = new SortedSet<string>(File.ReadLines(wordsPath).Take(vectorLength).ToArray(),
                        StringComparer.OrdinalIgnoreCase);
                    using (var stringDataParser = DataParser<string>.Init(StreamReaderUtils.EnumarateWords, windowSize,
                        optionalStrings, path1, path2, path3, path4, path5))
                    {
                        var initVectors = stringDataParser.Histograms.Map(h => h.CountVector());
                        var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                            epsilon, InnerProductFunction.MonitoredFunction, 2);
                        var changes = stringDataParser.AllCountVectors(stepSize).Take(amountOfIterations);
                        var results = multiRunner.RunToEnd(changes, rnd, true);
                        results.ForEach(r => resultCsvFile.WriteLine(r.AsCsvString()));
                    }

                }
            }

            Process.Start(resultPath);
        }

        public static void CompareToArnonBow(Random rnd)
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
              //  try
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
              //  catch (Exception e)
                {
             //       Console.WriteLine(e);
                }
            }

            Process.Start(resultPath);
        }
    }
}
