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
using Utils.TypeUtils;

namespace SecondMomentSketch
{
    public static class SecondMomentRunner
    {
        public static void Run(Random rnd, int width, int height, string resultDir)
        {
            var numOfNodes       = 7;
            var valuesRange      = 100;
            var windowSize       = 500;
            var stepSize         = 50;
            var vectorLength     = width * height;
            var iterations       = 200;
            var globalVectorType = GlobalVectorType.Average;
            var epsilon          = new AdditiveEpsilon(10000);
            var fileName         = $"F2_VecSize_{vectorLength}_Iters_{iterations}_Nodes_{numOfNodes}_Epsilon_{epsilon.EpsilonValue}.csv";
            var resultPath       = Path.Combine(resultDir, fileName);
            var secondMomentFunction = new SecondMoment(width, height);
            var trnd = Troschuetz.Random.TRandom.New(rnd.Next());

            var md5 = MD5.Create();
            int indicator(int vectorId, int index, int num)
            {
                var value = md5.ComputeHash(vectorId, index, num) % 2;
                return value * 2 - 1;
            }

            var vectors = ArrayUtils.Init(numOfNodes, _ => VectorUtils.CreateVector(vectorLength, __ => 0.0));

            Vector<double>[] InitVectors()
            {
                for (int time = 0; time < windowSize; time++)
                for (int j = 0; j < vectors.Length; j++)
                {
                    var valueToAdd = trnd.Binomial(0.5, valuesRange);
                    for (int i = 0; i < vectorLength; i++)
                        vectors[j][i] += 0.0; //indicator(j, i, valueToAdd);
                }

                return vectors;
            }
            
            Vector<double>[] GetChange()
            {
                var vecs = ArrayUtils.Init(numOfNodes, _ => VectorUtils.CreateVector(vectorLength, __ => 0.0));
                for (var time = 0; time < stepSize * 2; time++)
                {
                //    for (int j = 0; j < vectors.Length; j++)
                    for (int j = 0; j < 1; j++)
                    {
                        var valueToAddOrSubtruct = trnd.Binomial(0.5, valuesRange);
                        var mul = rnd.NextDouble() <= 0.75 ? 1 : -1;
                        for (int i = 0; i < vectorLength; i++)
                            vecs[j][i] += indicator(j, i, valueToAddOrSubtruct) * mul;
                    }
                }

                return vecs;
            }


            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));
                var multiRunner = MultiRunner.InitAll(InitVectors(), numOfNodes, vectorLength, globalVectorType,
                                                      epsilon, secondMomentFunction.MonitoredFunction);
                multiRunner.OnlySchemes(new MonitoringScheme.Value(), new MonitoringScheme.Vector(), new MonitoringScheme.Distance(2));
                var changes = Enumerable.Range(0, iterations).Select(_ => GetChange());
                    multiRunner.RunAll(changes, rnd, false)
                               .Select(r => r.AsCsvString())
                               .ForEach((Action<string>)resultCsvFile.WriteLine);
            }
            Process.Start(resultPath);
        }
    }
}
