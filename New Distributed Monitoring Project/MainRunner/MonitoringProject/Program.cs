using System;
using Entropy;
using InnerProduct;
using Utils.TypeUtils;

namespace MonitoringProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var resultPath = @"C:\Users\Yuval\Desktop\csvResult.csv";
            var wordsPath = @"C:\Users\Yuval\Desktop\Distributed Data Sets\MostCommonEnglishWords.txt";
            var textFilesPathes = new[]
                                  {
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\News Headlines\news headlines.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Jepordy\jeopardy.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\India News\india-news-headlines.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\Books\All Books Combined.txt",
                                      @"C:\Users\Yuval\Desktop\Distributed Data Sets\blogs\blogs.txt"
                                  };
            var random = new Random(12313424);

            InnerProductRunner.RunBagOfWords(random, wordsPath, resultPath, textFilesPathes);

        }
    }
}
