﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Entropy;
using InnerProduct;
using MoreLinq.Extensions;
using SecondMomentSketch;
using Sphere;
using Utils.TypeUtils;

namespace MonitoringProject
{
    public static class Program
    {
        public static readonly string resultDir = @"C:\Users\Yuval\Desktop";
        //public static readonly string databaseAccessesPath = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\TDADateSet.csv";
        public static readonly string databaseAccessesPath = @"C:\Users\Yuval\Desktop\Data\Traffic of Database Accesses\trimmed.csv";

        static void Main(string[] args)
        {
            var random = new Random(1631);
            int numOfNodes = 2;
            double epsilon = 0.1;
            var vectorLength = 50;
            EntropyRunner.RunDatabaseAccesses(random, numOfNodes, epsilon, vectorLength, databaseAccessesPath, resultDir);
           // InnerProductRunner.RunRandomly(random, numOfNodes, epsilon, vectorLength, resultDir);


            var width = 16;
            var height = 15;
            //SecondMomentRunner.RunRandomly(random, width, height, numOfNodes, epsilon, resultDir);
        }
    }
}
