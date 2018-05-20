using System;
using Entropy;
using InnerProduct;

namespace MonitoringProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random(12313424);
            InnerProductRunner.Run(random);
          //  EntropyRunner.Run(random);
        }
    }
}
