using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using MoreLinq.Extensions;
using Utils.TypeUtils;

namespace Sphere
{
    public static class SphereRunner
    {
        public static void Run(Random rnd, string resultPath)
        {
            var numOfNodes = 10;
            var vectorLength  = 20;
            var iterations = 1000;
            var globalVectorType = GlobalVectorType.Average;
            var epsilon = new AdditiveEpsilon(20.0);

            Vector<double>[] GetChange()
            {
                Vector<double> GenerateChange()
                {
                    return ArrayUtils.Init(vectorLength, i => rnd.NextDouble() - 0.5).ToVector();
                }

                //  return ArrayUtils.Init(numOfNodes, i => i == 0 ? GenerateChange() : ArrayUtils.Init(vectorLength, _ => 0.0).ToVector());
                return ArrayUtils.Init(numOfNodes, _ => GenerateChange());
            }

            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));
                var zeroVector = ArrayUtils.Init(vectorLength, _ => 0.0).ToVector();
                var initVectors = ArrayUtils.Init(numOfNodes, _ => zeroVector.ToVector());
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, globalVectorType,
                                                      epsilon, SphereFunction.MonitoredFunction);
                for (int i = 0; i < iterations; i++)
                    multiRunner.Run(GetChange(), rnd, false)
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>)resultCsvFile.WriteLine);

            }
            Process.Start(resultPath);
        }
    }
}
