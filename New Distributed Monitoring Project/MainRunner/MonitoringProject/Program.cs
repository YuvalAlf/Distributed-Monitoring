using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Entropy;
using InnerProduct;
using Monitoring.GeometricMonitoring.Epsilon;
using MoreLinq.Extensions;
using SecondMomentSketch;
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
            int numOfNodes = 5;
           // double epsilon = 0.5;
           // double epsilon = 0.05;
           // var vectorLength = 100000;
            //foreach (var numOfNodes in new[] {2})
            {
                //Console.WriteLine(numOfNodes);
                //EntropyRunner.RunDatabaseAccesses(random, numOfNodes, window, epsilon, vectorLength, databaseAccessesPath, resultDir);
            }
                
           
            // InnerProductRunner.RunRandomly(random, numOfNodes, epsilon, vectorLength, resultDir);

            //var values = new[] {(10, 11), (20, 21), (30, 31), (40, 41), (50, 51), (60, 61), (70, 71), (80, 81)};
            //var values = new[] {(10, 11), (20, 21), (30, 31), (40, 41), (50, 51), (60, 61), (70, 71), (80, 81)};
            var values = new[] { (50, 51) };
            //foreach (var (width, height) in values)
            //    SecondMomentRunner.RunRandomly(random, width, height, numOfNodes, resultDir);
            //var width = 10;
            //var height = 11;
            var window = 30;
            //double additiveEpsilon = 10000000 * window;
            var epsilon = new MultiplicativeEpsilon(0.9);
            foreach (var (width, height) in values)
            {
                SecondMomentRunner.RunDatabaseAccesses(random, numOfNodes, window, epsilon, width, height, databaseAccessesPath, resultDir);
            }
                
        }
    }
}
