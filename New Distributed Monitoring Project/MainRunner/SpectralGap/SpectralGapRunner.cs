using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Win32;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using MoreLinq.Extensions;
using SpectralGap.Parsing;
using Utils.MathUtils;
using Utils.TypeUtils;

namespace SpectralGap
{
    public static class SpectralGapRunner
    {
        private static Vector<double> GenerateMatrix(int size, double p, Random rnd)
        {
            var array = new double[size * (size - 1) / 2];
            for (int i = 0; i < array.Length; i++)
                if (rnd.NextDouble() <= p)
                    array[i] = 1;

            return array.ToVector();
        }

        public static void Run(Random rnd, int size, double edgeProb, int numOfNodes, string resultDir)
        {
            var globalVectorType   = GlobalVectorType.Sum;
            var epsilon            = new ThresholdEpsilon(10);
            var amountOfIterations = 500;
            var initMatrix = GenerateMatrix(size, edgeProb, rnd);
            var vectorLength = initMatrix.Count;
            var fileName = $"SpectralGap_VectorLength_{vectorLength}_Nodes_{numOfNodes}_Iterations_{amountOfIterations}.csv";
            var resultPath         = Path.Combine(resultDir, fileName);

            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));
                var multiRunner = MultiRunner.InitAll(SplitTo(initMatrix, size, numOfNodes, rnd), numOfNodes,
                                                      vectorLength, globalVectorType,
                                                      epsilon, SpectralGapFunction.MonitoredFunction);
                multiRunner.OnlySchemes(new MonitoringScheme.Distance(2),
                                        new MonitoringScheme.Value(),
                                        new MonitoringScheme.Vector(),
                                        new MonitoringScheme.Naive(),
                                        new MonitoringScheme.Oracle());
                var changes = GenerateChanges(initMatrix, numOfNodes, rnd).Take(amountOfIterations);
                multiRunner.RunAll(changes, rnd, false)
                           .FinishAfter(multiRunner.Runners.Count , r => double.IsNegativeInfinity(r.LowerBound))
                           .Select(r => r.AsCsvString())
                           .ForEach((Action<string>) resultCsvFile.WriteLine);
            }

            Process.Start(resultPath);
        }

        private static IEnumerable<Vector<double>[]> GenerateChanges(Vector<double> initMatrix,
                                                                     int            numOfNodes, Random rnd)
        {
            var amountToAdd = 3;
            var amountToRemove = 4;

            HashSet<int> GetIndicesToAdd(int amount)
            {
                var indices = new HashSet<int>();
                while (indices.Count < amount)
                {
                    var index = rnd.Next(initMatrix.Count);
                    if (initMatrix[index].AlmostEqual(0.0))
                        indices.Add(index);
                }
                return indices;
            }

            HashSet<int> GetIndicesToRemove(int amount)
            {
                var indices = new HashSet<int>();
                while (indices.Count < amount)
                {
                    var index = rnd.Next(initMatrix.Count);
                    if (initMatrix[index].AlmostEqual(1.0))
                        indices.Add(index);
                }
                return indices;
            }

            while (true)
            {
                var vectors = ArrayUtils.Init(numOfNodes, _ => Vector<double>.Build.Sparse(initMatrix.Count));
                foreach (var vector in vectors)
                {
                    var indicesToAdd = GetIndicesToAdd(amountToAdd);
                    var indicesToRemove = GetIndicesToRemove(amountToRemove);
                    indicesToAdd.ForEach(i => vector[i] = 1.0);
                    indicesToAdd.ForEach(i => initMatrix[i] = 1.0);
                    indicesToRemove.ForEach(i => vector[i] = -1.0);
                    indicesToRemove.ForEach(i => initMatrix[i] = 0.0);
                }

                yield return vectors;
            }
        }

        private static Vector<double>[] SplitTo(Vector<double> initMatrix, int size, int numOfNodes, Random rnd)
        {
            var count = 0;
            var vectors = ArrayUtils.Init(numOfNodes, _ => Vector<double>.Build.Sparse(initMatrix.Count));
            for (int i = 0; i < initMatrix.Count; i++)
                    if (initMatrix[i].AlmostEqual(1.0))
                    {
                        var index = count++ % vectors.Length;
                        vectors[index][i] = 1;
                    }
            return vectors;
        }

        public static void RunOnData(string graphPath, string resultDir)
        {
            var globalVectorType = GlobalVectorType.Sum;
            var epsilon          = new ThresholdEpsilon(10);
            var numOfNodes = 2;
            var fileName   = $"SpectralGap__YoutubeGraph__Nodes_{numOfNodes}.csv";
            var resultPath = Path.Combine(resultDir, fileName);
            var operations = GraphParsing.ReadGraph(graphPath);
            var initGraph = (GraphOperation.InitGraph) operations.First();
            var numOfVerices = initGraph.NumOfVertices;

            using (var resultCsvFile = File.CreateText(resultPath))
            {

            }


            Process.Start(resultPath);
        }
    }
}