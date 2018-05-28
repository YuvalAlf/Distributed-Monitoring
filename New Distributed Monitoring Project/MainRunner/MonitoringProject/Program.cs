using System;
using Entropy;
using InnerProduct;
using PCA;
using Utils.TypeUtils;

namespace MonitoringProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random(12313424);
            InnerProductRunner.CompareToArnonBow(random);
           // InnerProductRunner.RunBagOfWords(random);
            //InnerProductRunner.RunChars(random);
            // EntropyRunner.RunChars(random);
            //  EntropyRunner.RunBagOfWords(random);
        }
    }
}
