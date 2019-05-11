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
            int numOfNodes     = 5;
            var window         = 2;
            var vectorLength   = 11654;
            var distrubteUsers = UsersDistributing.RoundRobin();
            var epsilon        = new MultiplicativeEpsilon(0.15);
            EntropyRunner.RunDatabaseAccesses(random, numOfNodes, window, epsilon, vectorLength, distrubteUsers, databaseAccessesPath, resultDir);
        }

        static void Main(string[] args)
        {
            var random = new Random(1631);
            //RunEntropy(random);
            RunSecondMomentSketch(random);
        }
    }
}
