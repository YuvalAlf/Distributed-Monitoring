using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Entropy;
using InnerProduct;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Approximation;
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
        public static readonly string phoneActivitiesBaseFolder = @"C:\Users\Yuval\Downloads\MilanoPhoneActivity";
        //  public static readonly string databaseAccessesPath = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\trimmed.csv";



        private static void RunMilanoPhonesSecondMomentSketch(Random random)
        {
            int numOfNodes     = 5;
            var values         = new[] { (10, 11) };
            var window         = 10;
            var approximation  = new MultiplicativeUpperLowerApproximation(0.3, 3.0);
            foreach (var (width, height) in values)
                SecondMomentRunner.RunMilanoPhoneActivity(random, numOfNodes, window, approximation, width, height, phoneActivitiesBaseFolder, resultDir);
        }

        private static void RunDatabaseSecondMomentSketch(Random random)
        {
            int numOfNodes     = 10;
            var values         = new[] { (30, 31) };
            var window         = 100;
            var distrubteUsers = UsersDistributing.UnevenHashing();
            var approximation        = new MultiplicativeApproximation(0.9);
            foreach (var (width, height) in values)
                SecondMomentRunner.RunDatabaseAccesses(random, numOfNodes, window, approximation, width, height, distrubteUsers, databaseAccessesPath, resultDir);
        }
        private static void RunEntropy(Random random)
        {
            int numOfNodes     = 10;
            var window         = 14;
            var vectorLength   = 11654;
            var distrubteUsers = UsersDistributing.RoundRobin();
            var approximation = new MultiplicativeApproximation(0.02);
            EntropyRunner.RunDatabaseAccesses(random, numOfNodes, window, approximation, vectorLength, distrubteUsers, databaseAccessesPath, resultDir);
        }

        private static void RunInnerProduct(Random random)
        {
            int numOfNodes = 10;
            ApproximationType approximation = new MultiplicativeApproximation(0.05);
            int vectorLength = 200;
            int iterations = 500;
            InnerProductRunner.RunRandomly(random, numOfNodes, approximation, vectorLength, iterations, resultDir);
        }
        public static void Main(string[] args)
        {
            var random = new Random(1631);
            RunMilanoPhonesSecondMomentSketch(random);
            // RunEntropy(random);
            // RunDatabaseSecondMomentSketch(random);
            //  RunInnerProduct(random);
        }

    }
}
