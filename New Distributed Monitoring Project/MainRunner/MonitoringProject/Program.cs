using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Entropy;
using InnerProduct;
using MoreLinq.Extensions;
using SecondMomentSketch;
using Sphere;
using Utils.TypeUtils;

namespace MonitoringProject
{
    public static class Program
    {
        public static readonly string dataPcaPath = @"C:\Users\Yuval\Desktop\dataPca.dat";
        public static readonly string resultPathInnerProduct = @"C:\Users\Yuval\Desktop\csvResultInnerProduct.csv";
        public static readonly string resultDir = @"C:\Users\Yuval\Desktop";
        public static readonly string wordsPath = @"C:\Users\Yuval\Desktop\Distributed Data Sets\MostCommonEnglishWords.txt";

        static void Main(string[] args)
        {
            var textFilesPathes = new[]
                                  {
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Data Sets\Amazon Reviews\amazon.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Data Sets\Wikipedia\wikipedia.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Data Sets\News Headlines\news headlines.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Data Sets\India News\india.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Data Sets\Reddit Comments\reddit.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Data Sets\Restaurant Reviews\restaurant.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Data Sets\blogs\blogs.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Data Sets\Tweets\tweets.txt",
                                  };
            var random = new Random(631);

            IEnumerable<int> Func()
            {
                yield return 1;
                yield return 2;
                yield return 3;
                yield return 4;
            }

            //      GraphParsing.WriteGraph(@"C:\Users\Yuval\Desktop\youtube.graph", GraphParsing.TransformGraph(@"C:\Users\Yuval\Downloads\youtube-u-growth\out.youtube-u-growth"));

            //  var youtubeGraph = @"C:\Users\Yuval\Desktop\Distributed Data Sets\Data Sets\YoutubeGraph\youtube.graph";
            //    SpectralGapFunction.RunOnData(youtubeGraph, resultDir);
            //   SpectralGapFunction.Run(random, 150, 0.2, 5, resultDir);


            //  InnerProductRunner.RunOneChange(random, 2500, resultDir);

            //       var seed = random.Next();
            //SecondMomentRunner.Run(new Random(seed), 6, 5, resultDir);
            //       SecondMomentRunner.Run(new Random(seed), 12, 11, resultDir);
            /* SecondMomentRunner.Run(new Random(seed), 18, 17, 200000, resultDir);
             SecondMomentRunner.Run(new Random(seed), 24, 21, 200000, resultDir);
             SecondMomentRunner.Run(new Random(seed), 30, 27, 200000, resultDir);
             SecondMomentRunner.Run(new Random(seed), 36, 31, 200000, resultDir);
             SecondMomentRunner.Run(new Random(seed), 42, 37, 200000, resultDir);
             SecondMomentRunner.Run(new Random(seed), 46, 41, 200000, resultDir);
             SecondMomentRunner.Run(new Random(seed), 52, 47, 200000, resultDir);*/



            //   Func<int, bool> isLeft = i => i < 4;
            //    SphereRunner.Run(random, resultDir);

            //  Console.WriteLine("Left:");
            //  textFilesPathes.Where((_, i) => isLeft(i)).Select(t => "\t" + t).ForEach(s => Console.WriteLine(s));
            //   Console.WriteLine("Right:");
            //   textFilesPathes.Where((_, i) => !isLeft(i)).Select(t => "\t" + t).ForEach(s => Console.WriteLine(s));



            //InnerProductRunner.RunBagOfWords(random, 1000, wordsPath, resultDir, isLeft, textFilesPathes);
            //  InnerProductRunner.RunBagOfWords(random, 250, wordsPath, resultDir, isLeft, textFilesPathes);
            //  InnerProductRunner.RunBagOfWords(random, 500, wordsPath, resultDir, isLeft, textFilesPathes);
            // InnerProductRunner.RunBagOfWords(random, 2500, wordsPath, resultDir, isLeft, textFilesPathes);
            // InnerProductRunner.RunBagOfWords(random, 5000, wordsPath, resultDir, isLeft, textFilesPathes);
            // InnerProductRunner.RunBagOfWords(random, 7500, wordsPath, resultDir, isLeft, textFilesPathes);
            //  InnerProductRunner.RunBagOfWords(random, 10000, wordsPath, resultDir, isLeft, textFilesPathes);
            // EntropyRunner.RunBagOfWords(random, 250, wordsPath, resultDir, textFilesPathes);
            //  EntropyRunner.RunBagOfWords(random, 500, wordsPath, resultDir, textFilesPathes);
            //  EntropyRunner.RunBagOfWords(random, 750, wordsPath, resultDir, textFilesPathes);
        }
    }
}
