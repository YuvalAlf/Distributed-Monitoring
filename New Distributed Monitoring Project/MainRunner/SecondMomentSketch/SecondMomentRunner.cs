using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using MoreLinq.Extensions;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace SecondMomentSketch
{
    public static class SecondMomentRunner
    {
        public static void RunRandomly(Random rnd, int width, int height, int numOfNodes, double epsilonValue, string resultDir)
        {
            var vectorLength     = width * height;
            var iterations       = 1000;
            var globalVectorType = GlobalVectorType.Average;
            var epsilon          = new MultiplicativeEpsilon(epsilonValue);
            var fileName         = $"F2_VecSize_{vectorLength}_Iters_{iterations}_Nodes_{numOfNodes}_Epsilon_{epsilon.EpsilonValue}.csv";
            var resultPath       = Path.Combine(resultDir, fileName);
            var secondMomentFunction = new SecondMoment(width, height);
            
            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));

                var initVectors = ArrayUtils.Init(numOfNodes, _ => ArrayUtils.Init(vectorLength, __ => (double)rnd.Next(-100, 101)).ToVector());

                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                                                      epsilon, secondMomentFunction.MonitoredFunction);

                for (int i = 0; i < iterations; i++)
                {
                    var changes = ArrayUtils.Init(numOfNodes, _ => ArrayUtils.Init(vectorLength, __ => (double)rnd.Next(-1, 2)).ToVector());
                    multiRunner.Run(changes, rnd, false)
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>)resultCsvFile.WriteLine);
                }
            }
            Process.Start(resultPath);
        }
    }
}
