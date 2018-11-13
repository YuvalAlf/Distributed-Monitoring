using System;
using System.Linq;
using ClassLibrary1;
using Entropy;
using InnerProduct;
using MoreLinq.Extensions;
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

          //  SpectralGapFunction.Run(random, 40, 0.5, 20, resultDir);

             Func<int, bool> isLeft = i => i < 4;
            //   SphereRunner.Run(random, resultDir);

          //  Console.WriteLine("Left:");
          //  textFilesPathes.Where((_, i) => isLeft(i)).Select(t => "\t" + t).ForEach(s => Console.WriteLine(s));
         //   Console.WriteLine("Right:");
         //   textFilesPathes.Where((_, i) => !isLeft(i)).Select(t => "\t" + t).ForEach(s => Console.WriteLine(s));



            //   InnerProductRunner.RunBagOfWords(random, 2500, wordsPath, resultDir, isLeft, textFilesPathes);
            EntropyRunner.RunBagOfWords(random, 100, wordsPath, resultDir, textFilesPathes);
        }
    }
}
