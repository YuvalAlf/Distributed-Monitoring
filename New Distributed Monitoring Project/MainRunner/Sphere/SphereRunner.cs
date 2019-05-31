using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.Approximation;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using MoreLinq.Extensions;
using Utils.SparseTypes;
using Utils.TextualUtils;
using Utils.TypeUtils;

namespace Sphere
{
    public static class SphereRunner
    {
        public static void Run(Random rnd, int numOfNodes, int vectorLength, int iterations, ApproximationType approximation, string resultDir)
        {
            var resultPath =
                PathBuilder.Create(resultDir, "Sphere")
                           .AddProperty("Data",          "Random")
                           .AddProperty("VectorLength",  vectorLength.ToString())
                           .AddProperty("Iterations",    iterations.ToString())
                           .AddProperty("Nodes",         numOfNodes.ToString())
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");

            Vector[] GetChange()
            {
                Vector GenerateChange()
                {
                    return ArrayUtils.Init(vectorLength, i => (rnd.NextDouble() - 0.5) / 5).ToVector();
                }

                //  return ArrayUtils.Init(numOfNodes, GenerateChange() : ArrayUtils.Init(vectorLength, _ => 0.0).ToVector());
                return ArrayUtils.Init(numOfNodes, _ => GenerateChange());
            }

            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes)))
            {
                var sphereFunction = new SphereFunction(vectorLength);
                var initVectors = Vector.Init(numOfNodes);
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength,
                                                      approximation, sphereFunction.MonitoredFunction);
                multiRunner.OnlySchemes(new MonitoringScheme.Value(), new MonitoringScheme.Distance(2), new MonitoringScheme.Oracle(), new MonitoringScheme.Vector());
                for (int i = 0; i < iterations; i++)
                    multiRunner.Run(GetChange(), rnd, false)
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>)resultCsvFile.WriteLine);

            }
        }
    }
}
