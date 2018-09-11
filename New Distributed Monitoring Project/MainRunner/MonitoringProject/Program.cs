using System;
using Entropy;
using InnerProduct;
using Sphere;
using Utils.TypeUtils;

namespace MonitoringProject
{
    public static class Program
    {
        public static readonly string dataPcaPath = @"C:\Users\Yuval\Desktop\dataPca.dat";
        public static readonly string resultPath = @"C:\Users\Yuval\Desktop\csvResult.csv";
        public static readonly string wordsPath = @"C:\Users\Yuval\Desktop\Distributed Data Sets\MostCommonEnglishWords.txt";

        static void Main(string[] args)
        {
            var textFilesPathes = new[]
                                  {
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\News Headlines\news headlines.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Jepordy\jeopardy.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\India News\india-news-headlines.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\All Books Combined.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\blogs\blogs.txt"
                                  };
            var random = new Random(125424);

            SphereRunner.Run(random, resultPath);

            //InnerProductRunner.CalculatePca(random, wordsPath, dataPcaPath, textFilesPathes);

          //  InnerProductRunner.RunBagOfWords(random, wordsPath, resultPath, textFilesPathes);

        }
    }
}
