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

namespace Monitoring.GeometricMonitoring.Running
{
    public sealed class MultiRunner
    {
        public Dictionary<MonitoringScheme, Runner> Runners { get; }

        public MultiRunner(Dictionary<MonitoringScheme, Runner> runners) => Runners = runners;

        public void RemoveScheme(MonitoringScheme scheme) => Runners.Remove(scheme);

        public IEnumerable<AccumaltedResult> Run(Vector<double>[] change, Random rnd, bool parrallel) 
            => Runners.Values.AsParallel().Select(runner => runner.Run(change, rnd));

        public IEnumerable<AccumaltedResult> RunAll(IEnumerable<Vector<double>[]> changes, Random rnd, bool parrallel) 
            => changes.SelectMany(change => this.Run(change, rnd, parrallel));

        public IEnumerable<AccumaltedResult> RunToEnd(IEnumerable<Vector<double>[]> changes, Random rnd, bool parrallel)
            => RunAll(changes, rnd, parrallel).TakeLast(Runners.Count);

        public string HeaderCsv => Runners.Values.First().AccumalatedResult.HeaderCsv();


        public static MultiRunner InitAll(
            Vector<double>[] initVectors,
            int numOfNodes,
            int vectorLength,
            GlobalVectorType globalVectorType,
            EpsilonType epsilon,
            MonitoredFunction monitoredFunction,
            params int[] distanceNorms)
        {
            var runners = new Dictionary<MonitoringScheme, Runner>();

            void AddRunner<Server>(MonitoringScheme monitoringScheme, Server server)
                where Server : AbstractServer<Server>
            {
                var runner = MonitoringRunner<Server>.Create(server, monitoringScheme);
                runners[monitoringScheme] = runner;
            }
            var valueServer = NodeServer<ValueNode>.Create(initVectors, numOfNodes, vectorLength, globalVectorType,
                epsilon, monitoredFunction, ValueNode.ResolveNodes, ValueNode.Create);
            var vectorServer = NodeServer<VectorNode>.Create(initVectors, numOfNodes, vectorLength, globalVectorType,
                epsilon, monitoredFunction, VectorNode.ResolveNodes, VectorNode.Create);
            var oracleServer = OracleServer.Create(initVectors, numOfNodes, vectorLength, globalVectorType,
                epsilon, monitoredFunction);

            AddRunner(new MonitoringScheme.Value(), valueServer);
            AddRunner(new MonitoringScheme.Vector(), vectorServer);
            AddRunner(new MonitoringScheme.Oracle(), oracleServer);
            foreach (var distanceNorm in distanceNorms)
            {
                var distanceServer = NodeServer<DistanceNode>.Create(initVectors, numOfNodes, vectorLength, globalVectorType,
                    epsilon, monitoredFunction, DistanceNode.ResolveNodes, DistanceNode.CreateNorm(distanceNorm));
                AddRunner(new MonitoringScheme.Distance(distanceNorm), distanceServer);
            }

            return new MultiRunner(runners);
        }
    }
}
