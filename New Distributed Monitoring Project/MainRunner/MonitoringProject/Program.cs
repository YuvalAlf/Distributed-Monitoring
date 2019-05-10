using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Entropy;
using InnerProduct;
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

        static void Main(string[] args)
        {
            var random = new Random(1631);
            int numOfNodes = 10;
            var values = new[] { (20, 21) };
            var window = 100;
            var distrubteUsers = UsersDistributing.UnevenHashing();
            var epsilon = new MultiplicativeEpsilon(0.9);
            foreach (var (width, height) in values)
                SecondMomentRunner.RunDatabaseAccesses(random, numOfNodes, window, epsilon, width, height, distrubteUsers, databaseAccessesPath, resultDir);
        }
    }
}
