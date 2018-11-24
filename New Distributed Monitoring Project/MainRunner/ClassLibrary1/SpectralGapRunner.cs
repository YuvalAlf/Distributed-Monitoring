﻿using System;
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
using Utils.MathUtils;
using Utils.TypeUtils;

namespace ClassLibrary1
{
    public static partial class SpectralGapFunction
    {
        private static Matrix<double> GenerateMatrix(int size, double p, Random rnd)
        {
            var array = new double[size, size];
            for (int i = 0; i < size; i++)
                for (int j = i + 1; j < size; j++)
                {
                    var value                 = Convert.ToInt32(rnd.NextDouble() <= p);
                    array[i, j] = array[j, i] = value;
                }

            return Matrix<double>.Build.DenseOfArray(array);
        }

        public static void Run(Random rnd, int size, double edgeProb, int numOfNodes, string resultDir)
        {
            var globalVectorType   = GlobalVectorType.Sum;
            var epsilon            = new ThresholdEpsilon(0.5);
            var amountOfIterations = 50;
            var fileName           = "spectralGap.csv";
            var resultPath         = Path.Combine(resultDir, fileName);

            using (var resultCsvFile = File.CreateText(resultPath))
            {
                resultCsvFile.AutoFlush = true;
                resultCsvFile.WriteLine(AccumaltedResult.Header(numOfNodes));
                var initMatrix = GenerateMatrix(size, edgeProb, rnd);
                //var globalVector = initMatrix.ToRowArrays().SelectMany(x => x).ToVector();
                var multiRunner = MultiRunner.InitAll(SplitTo(initMatrix, size, numOfNodes, rnd), numOfNodes,
                                                      size * size, globalVectorType,
                                                      epsilon, SpectralGapFunction.MonitoredFunction);
                multiRunner.RemoveScheme(new MonitoringScheme.SketchedChangeValue("Standard Base"));
                multiRunner.RemoveScheme(new MonitoringScheme.SketchedChangeDistance("Standard Base", 2));
                var changes = GenerateChanges(initMatrix, size, numOfNodes, rnd).Take(amountOfIterations);
                multiRunner.RunAll(changes, rnd, false)
                           .Select(r => r.AsCsvString())
                           .ForEach((Action<string>) resultCsvFile.WriteLine);
            }

            Process.Start(resultPath);
        }

        private static IEnumerable<Vector<double>[]> GenerateChanges(Matrix<double> initMatrix, int    size,
                                                                     int            numOfNodes, Random rnd)
        {
            while (true)
            {
                var vectors = ArrayUtils.Init(numOfNodes, _ => Vector<double>.Build.Sparse(size * size));
                foreach (var vector in vectors)
                {
                    var (i, j) = rnd.ChooseTwoDifferentRandomsInRange(0, size);
                    var index1 = i * size + j;
                    var index2 = j * size + i;
                    var value  = initMatrix[i, j];
                    var change = value.AlmostEqual(0.0) ? 1 : -1;
                    vector[index1]   += change;
                    vector[index2]   += change;
                    initMatrix[i, j] += change;
                    initMatrix[j, i] += change;
                }

                yield return vectors;
            }
        }

        private static Vector<double>[] SplitTo(Matrix<double> initMatrix, int size, int numOfNodes, Random rnd)
        {
            var count = 0;
            var vectors = ArrayUtils.Init(numOfNodes, _ => Vector<double>.Build.Sparse(size * size));
            for (int i = 0; i < initMatrix.ColumnCount; i++)
                for (int j = i + 1; j < initMatrix.RowCount; j++)
                    if (initMatrix[i, j].AlmostEqual(1.0))
                    {
                        var index = count++ % vectors.Length;
                        vectors[index][i * size + j] = 1;
                        vectors[index][j * size + i] = 1;
                    }
            return vectors;
        }
    }
}