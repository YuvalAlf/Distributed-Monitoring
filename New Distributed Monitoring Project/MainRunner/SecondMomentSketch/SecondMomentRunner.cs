using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Random;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Approximation;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.GeometricMonitoring.VectorType;
using Monitoring.Utils.DataDistributing;
using MoreLinq.Extensions;
using Parsing;
using SecondMomentSketch.Hashing;
using Utils.AiderTypes;
using Utils.MathUtils;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace SecondMomentSketch
{

    public static class SecondMomentRunner
    {
        public static void RunRandomly(Random rnd, int width, int height, int numOfNodes, int iterations, ApproximationType approximation,
                                       string resultDir)
        {
            var vectorLength = width * height;
            var resultPath =
                PathBuilder.Create(resultDir, "AMS_F2")
                           .AddProperty("Dataset",       "Random")
                           .AddProperty("Nodes",         numOfNodes.ToString())
                           .AddProperty("VectorLength",  vectorLength.ToString())
                           .AddProperty("Width",        width.ToString())
                           .AddProperty("Height",        height.ToString())
                           .AddProperty("Iterations",    iterations.ToString())
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");
            var secondMomentFunction = new SecondMoment(width, height);

            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes)))
            {
                var initVectors =
                    ArrayUtils.Init(numOfNodes,
                                    _ => ArrayUtils.Init(vectorLength, __ => (double) 0.0).ToVector());

                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength,
                                                      approximation, secondMomentFunction.MonitoredFunction);

                for (int i = 0; i < iterations; i++)
                {
                    var changes = ArrayUtils.Init(numOfNodes,
                                                  nodeIndex =>
                                                      //nodeIndex == 0 ? 
                                                      ArrayUtils.Init(vectorLength, __ => (double) rnd.Next(-1, 3)).ToVector());
                                                         // : ArrayUtils.Init(vectorLength, __ => 0.0).ToVector());
                    var stop = new StrongBox<bool>(false);
                    multiRunner.Run(changes, rnd, false)
                               .SideEffect(r => stop.Value = stop.Value || (r.MonitoringScheme.Equals(new MonitoringScheme.Oracle()) && r.NumberOfFullSyncs > 3))
                               .Select(r => r.AsCsvString())
                               .ForEach(resultCsvFile.WriteLine);
                    if (stop.Value)
                        break;
                }
            }
        }

        public static void RunDatabaseAccesses(Random rnd,          int    numOfNodes, int window,
                                               ApproximationType approximation, int    width,
                                               int    height, UsersDistributing distributing,
                                               string databaseAccessesPath, string resultDir)
        {
            var vectorLength       = width * height;
            var hashFunction       = FourwiseIndepandantFunction.Init(rnd);
            var hashFunctionsTable = HashFunctionTable.Init(numOfNodes, vectorLength, hashFunction);
            var secondMomentFunction = new SecondMoment(width, height);
            var resultPath =
                PathBuilder.Create(resultDir, "AMS_F2")
                           .AddProperty("Dataset",       "DatabaseAccesses")
                           .AddProperty("Nodes",         numOfNodes.ToString())
                           .AddProperty("VectorLength",  vectorLength.ToString())
                           .AddProperty("Width",         width.ToString())
                           .AddProperty("Height",        height.ToString())
                           .AddProperty("Window",        window.ToString())
                           .AddProperty("Distributing",        distributing.Name)
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");

            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes)))
            using (var databaseAccessesStatistics = DatabaseAccessesStatistics.Init(databaseAccessesPath, numOfNodes, window, distributing.DistributeFunc))
            {
                var initCountVectors = databaseAccessesStatistics.InitCountVectors();
                var initVectors = hashFunction.TransformToAMSSketch(initCountVectors, vectorLength, hashFunctionsTable);
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength,
                                                      approximation, secondMomentFunction.MonitoredFunction);
                while (databaseAccessesStatistics.TakeStep())
                {
                    var changeCountVectors = databaseAccessesStatistics.GetChangeCountVectors();
                    var changeVectors = hashFunction.TransformToAMSSketch(changeCountVectors, vectorLength, hashFunctionsTable);
                    multiRunner.Run(changeVectors, rnd, true)
                               .Select(r => r.AsCsvString())
                               .ForEach(resultCsvFile.WriteLine);
                }
            }
        }

        public static void RunMilanoPhoneActivity(Random            rnd,           int numOfNodes, int window,
                                                  ApproximationType approximation, int width,
                                                  int               height, GeographicalDistributing distributingMethod,
                                                  string            phoneActivityDir, string resultDir)
        {
            var vectorLength         = width * height;
            var hashFunction         = FourwiseIndepandantFunction.Init(rnd);
            var hashFunctionsTable   = HashFunctionTable.Init(numOfNodes, vectorLength, hashFunction);
            var secondMomentFunction = new SecondMoment(width, height);
            var resultPath =
                PathBuilder.Create(resultDir, "AMS_F2")
                           .AddProperty("Dataset",       "MilanoPhoneActivity")
                           .AddProperty("Nodes",         numOfNodes.ToString())
                           .AddProperty("VectorLength",  vectorLength.ToString())
                           .AddProperty("Width",         width.ToString())
                           .AddProperty("Height",        height.ToString())
                           .AddProperty("Window",        window.ToString())
                           .AddProperty("Distributing",  distributingMethod.Name)
                           .AddProperty("Approximation", approximation.AsString())
                           .ToPath("csv");

            using (var resultCsvFile = AutoFlushedTextFile.Create(resultPath, AccumaltedResult.Header(numOfNodes)))
            {
                var phonesActivityDataParser = PhonesActivityDataParser.Create(phoneActivityDir);
                var phonesActivityWindowManger = PhonesActivityWindowManger.Init(window, numOfNodes, vectorLength, hashFunctionsTable, phonesActivityDataParser, distributingMethod);
                var initVectors = phonesActivityWindowManger.GetCurrentVectors();
                var multiRunner = MultiRunner.InitAll(initVectors, numOfNodes, vectorLength, approximation, secondMomentFunction.MonitoredFunction);
                while (phonesActivityWindowManger.TakeStep())
                {
                    var shouldEnd = new StrongBox<bool>(false);
                    var changeVectors = phonesActivityWindowManger.GetChangeVector();
                    multiRunner.Run(changeVectors, rnd, true)
                           //    .SideEffect(a => shouldEnd.Value = shouldEnd.Value || (a.MonitoringScheme is MonitoringScheme.Oracle && a.NumberOfFullSyncs > 0))
                               .Select(r => r.AsCsvString())
                               .ForEach(resultCsvFile.WriteLine);
                    //if (shouldEnd.Value)
                      //  break;
                }
            }
        }
    }
}
