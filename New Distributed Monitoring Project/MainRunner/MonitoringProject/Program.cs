using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Entropy;
using InnerProduct;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Epsilon;
using MoreLinq.Extensions;
using SecondMomentSketch;
using SecondMomentSketch.Hashing;
using Sphere;
using Utils.TypeUtils;

namespace MonitoringProject
{
    public static class Program
    {
        public static readonly string resultDir = @"C:\Users\Yuval\Desktop";
        public static readonly string databaseAccessesPath = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\TDADateSet.csv";
      //  public static readonly string databaseAccessesPath = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\trimmed.csv";


        private static void RunSecondMomentSketch(Random random)
        {
            int numOfNodes     = 10;
            var values         = new[] { (30, 31) };
            var window         = 100;
            var distrubteUsers = UsersDistributing.UnevenHashing();
            var epsilon        = new MultiplicativeEpsilon(0.9);
            foreach (var (width, height) in values)
                SecondMomentRunner.RunDatabaseAccesses(random, numOfNodes, window, epsilon, width, height, distrubteUsers, databaseAccessesPath, resultDir);
        }
        private static void RunEntropy(Random random)
        {
            int numOfNodes     = 10;
            var window         = 14;
            var vectorLength   = 11654;
            var distrubteUsers = UsersDistributing.RoundRobin();
            var epsilon        = new MultiplicativeEpsilon(0.02);
            EntropyRunner.RunDatabaseAccesses(random, numOfNodes, window, epsilon, vectorLength, distrubteUsers, databaseAccessesPath, resultDir);
        }

        private static void RunInnerProduct(Random random)
        {
            int numOfNodes = 5;
            EpsilonType epsilon = new MultiplicativeEpsilon(0.05);
            int vectorLength = 100;
            InnerProductRunner.RunRandomly(random, numOfNodes, epsilon, vectorLength, resultDir);
        }
        static void Main(string[] args)
        {
            var random = new Random(1631);
           // RunEntropy(random);
           // RunSecondMomentSketch(random);
            RunInnerProduct(random);
        }

    }
}
