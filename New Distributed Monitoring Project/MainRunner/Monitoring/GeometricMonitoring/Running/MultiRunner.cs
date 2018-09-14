using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.VectorType;
using Monitoring.Nodes;
using Monitoring.Servers;
using MoreLinq;
using Utils.MathUtils;
using Utils.MathUtils.Sketches;
using Utils.TypeUtils;
using SketchFunction = Utils.MathUtils.Sketches.SketchFunction;

namespace Monitoring.GeometricMonitoring.Running
{
    public sealed class MultiRunner
    {
        public Dictionary<MonitoringScheme, Runner> Runners { get; }

        public MultiRunner(Dictionary<MonitoringScheme, Runner> runners) => Runners = runners;

        public void RemoveScheme(MonitoringScheme scheme) => Runners.Remove(scheme);

        public IEnumerable<AccumaltedResult> Run(Vector<double>[] change, Random rnd, bool parallel)
        {
            if (parallel)
                return Runners.Values.AsParallel().Select(runner => runner.Run(change, rnd));
            else
                return Runners.Values.Select(runner => runner.Run(change, rnd));
        }

        public IEnumerable<AccumaltedResult> RunAll(IEnumerable<Vector<double>[]> changes, Random rnd, bool parallel) 
            => changes.SelectMany(change => this.Run(change, rnd, parallel));

        public IEnumerable<AccumaltedResult> RunToEnd(IEnumerable<Vector<double>[]> changes, Random rnd, bool parallel)
            => RunAll(changes, rnd, parallel).TakeLast(Runners.Count);

        public string HeaderCsv => Runners.Values.First().AccumalatedResult.HeaderCsv();


        public static MultiRunner InitAll(
            Vector<double>[]  initVectors,
            int               numOfNodes,
            int               vectorLength,
            GlobalVectorType  globalVectorType,
            EpsilonType       epsilon,
            MonitoredFunction monitoredFunction)
        {
            var runners = new Dictionary<MonitoringScheme, Runner>();

            void AddRunner<Server>(MonitoringScheme monitoringScheme, Server server)
                where Server : AbstractServer<Server>
            {
                var runner = MonitoringRunner<Server>.Create(server, monitoringScheme);
                runners[monitoringScheme] = runner;
            }

            var valueServer  = NodeServer<ValueNode>.Create (initVectors, numOfNodes, vectorLength, globalVectorType, epsilon, monitoredFunction, ValueNode.ResolveNodes, ValueNode.Create);
            var vectorServer = NodeServer<VectorNode>.Create(initVectors, numOfNodes, vectorLength, globalVectorType, epsilon, monitoredFunction, VectorNode.ResolveNodes, VectorNode.Create);
            var oracleServer = OracleServer.Create          (initVectors, numOfNodes, vectorLength, globalVectorType, epsilon, monitoredFunction);
            var naiveServer  = NaiveServer.Create           (initVectors, numOfNodes, vectorLength, globalVectorType, epsilon, monitoredFunction);
            var dctSketchedChangeValueServer          = NodeServer<SketchedChangeValueNode>.Create(initVectors, numOfNodes, vectorLength, globalVectorType, epsilon, monitoredFunction, SketchedChangeValueNode.ResolveNodes, SketchedChangeValueNode.Create(SketchFunction.DCTSketch));
            var standardBaseSketchedChangeValueServer = NodeServer<SketchedChangeValueNode>.Create(initVectors, numOfNodes, vectorLength, globalVectorType, epsilon, monitoredFunction, SketchedChangeValueNode.ResolveNodes, SketchedChangeValueNode.Create(SketchFunction.StandardBaseSketch));
            //var dctSketchedDataValueServer = NodeServer<SketchedDataValueNode>.Create(initVectors, numOfNodes, vectorLength, globalVectorType, epsilon, monitoredFunction, SketchedDataValueNodeResolveNodes, SketchedDataValueNodeCreate(SketchFunctionDCTSketch));
            //var standardBaseSketchedDataValueServer = NodeServer<SketchedDataValueNode>.Create(initVectors, numOfNodes, vectorLength, globalVectorType, epsilon, monitoredFunction, SketchedDataValueNodeResolveNodes, SketchedDataValueNodeCreate(SketchFunctionStandardBaseSketch));
            
            AddRunner(new MonitoringScheme.Value(),  valueServer);
            AddRunner(new MonitoringScheme.Vector(), vectorServer);
            AddRunner(new MonitoringScheme.Oracle(), oracleServer);
            AddRunner(new MonitoringScheme.Naive(),  naiveServer);
            AddRunner(new MonitoringScheme.SketchedChangeValue("DCT"),           dctSketchedChangeValueServer);
            AddRunner(new MonitoringScheme.SketchedChangeValue("Standard Base"), standardBaseSketchedChangeValueServer);
           // AddRunner(new MonitoringScheme.SketchedDataValue("DCT"),             dctSketchedDataValueServer);
           // AddRunner(new MonitoringScheme.SketchedDataValue("Standard Base"),   standardBaseSketchedDataValueServer);

            foreach (var distanceNorm in monitoredFunction.Norms)
            {
                var distanceServer = NodeServer<DistanceNode>.Create(initVectors, numOfNodes, vectorLength, globalVectorType, epsilon, monitoredFunction, DistanceNode.ResolveNodes, DistanceNode.CreateNorm(distanceNorm));
                AddRunner(new MonitoringScheme.Distance(distanceNorm), distanceServer);
            }

            return new MultiRunner(runners);
        }

        public void OnlyScheme(MonitoringScheme onlyScheme)
        {
            foreach (var monitoringScheme in this.Runners.Keys.Except(new [] {onlyScheme}).ToArray())
                this.RemoveScheme(monitoringScheme);
        }
    }
}
