using System;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.Servers;
using Utils.SparseTypes;

namespace Monitoring.GeometricMonitoring.Running
{
    public sealed class MonitoringRunner<ServerType> : Runner
        where ServerType : AbstractServer<ServerType>
    {
        public ServerType Server { get; private set; }

        public MonitoringRunner(ServerType server, AccumaltedResult accumalatedResult) : base(accumalatedResult) 
            => Server = server;

        public static MonitoringRunner<ServerType> Create(ServerType server, MonitoringScheme monitoringScheme)
        {
            var accumalatedResult = AccumaltedResult.Init(server.Epsilon, server.NumOfNodes, server.VectorLength, monitoringScheme);
            return new MonitoringRunner<ServerType>(server, accumalatedResult);
        }

        public string HeaderCsv => AccumalatedResult.HeaderCsv();


        public override AccumaltedResult Run(Vector[] change, Random rnd)
        {
            var (newServer, singleResult) = Server.Change(change, rnd);
            Server = newServer;
            AccumalatedResult = AccumalatedResult.AddSingleRsult(singleResult);
            return AccumalatedResult;
        }

        public (AccumaltedResult, Vector[]) RunWithDataPca(Vector[] change, Random rnd)
        {
            var (newServer, singleResult) = Server.Change(change, rnd);
            Server = newServer;
            AccumalatedResult = AccumalatedResult.AddSingleRsult(singleResult);
            return (AccumalatedResult, Server.NodesVectors);
        }

    }
}
