using System;
using System.Linq;
using System.Runtime.InteropServices;
using ClassLibrary1;
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
            var             random = new Random(631);

            SecondMomentRunner.Run(random, 30000, resultDir);
            SecondMomentRunner.Run(random, 31000, resultDir);
            SecondMomentRunner.Run(random, 32000, resultDir);
            SecondMomentRunner.Run(random, 33000, resultDir);
            SecondMomentRunner.Run(random, 34000, resultDir);
            SecondMomentRunner.Run(random, 35000, resultDir);
            SecondMomentRunner.Run(random, 36000, resultDir);
            SecondMomentRunner.Run(random, 37000, resultDir);

          //  SpectralGapFunction.Run(random, 100, 0.05, 5, resultDir);

         //    Func<int, bool> isLeft = i => i < 4;
         //    SphereRunner.Run(random, resultDir);

          //  Console.WriteLine("Left:");
          //  textFilesPathes.Where((_, i) => isLeft(i)).Select(t => "\t" + t).ForEach(s => Console.WriteLine(s));
         //   Console.WriteLine("Right:");
         //   textFilesPathes.Where((_, i) => !isLeft(i)).Select(t => "\t" + t).ForEach(s => Console.WriteLine(s));



            //   InnerProductRunner.RunBagOfWords(random, 1500, wordsPath, resultDir, isLeft, textFilesPathes);
           // EntropyRunner.RunBagOfWords(random, 100, wordsPath, resultDir, textFilesPathes);
        }
    }
}
