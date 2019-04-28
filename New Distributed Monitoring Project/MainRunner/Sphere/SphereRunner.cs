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
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using MoreLinq.Extensions;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Sphere
{
    public static class SphereRunner
    {
        public static void Run(Random rnd, string resultDir)
        {
            var numOfNodes = 20;
            var vectorLength  = 100;
            var iterations = 3000;
            var epsilon = new AdditiveEpsilon(80.0);
            var fileName   = $"Sphere_VecSize_{vectorLength}_Iters_{iterations}_Nodes_{numOfNodes}_Epsilon_{epsilon.EpsilonValue}.csv";
            var resultPath = Path.Combine(resultDir, fileName);

            Vector[] GetChange()
            {
                Vector GenerateChange()
                {
                    return ArrayUtils.Init(vectorLength, i => (rnd.NextDouble() - 0.5) / 5).ToVector();
                }

                //  return ArrayUtils.Init(numOfNodes, GenerateChange() : ArrayUtils.Init(vectorLength, _ => 0.0).ToVector());
                return ArrayUtils.Init(numOfNodes, _ => GenerateChange());
            }

            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));
                var sphereFunction = new SphereFunction(vectorLength);
                var initVectors = Vector.Init(numOfNodes);
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength,
                                                      epsilon, sphereFunction.MonitoredFunction);
                multiRunner.OnlySchemes(new MonitoringScheme.Value(), new MonitoringScheme.Distance(2), new MonitoringScheme.Oracle(), new MonitoringScheme.Vector());
                for (int i = 0; i < iterations; i++)
                    multiRunner.Run(GetChange(), rnd, false)
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>)resultCsvFile.WriteLine);

            }
            Process.Start(resultPath);
        }
    }
}
