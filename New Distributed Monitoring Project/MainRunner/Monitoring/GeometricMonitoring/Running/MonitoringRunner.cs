using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.GeometricMonitoring.MonitoringType;
using Monitoring.GeometricMonitoring.Running;
using Monitoring.Servers;

namespace Monitoring.Data
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


        public override AccumaltedResult Run(Vector<double>[] change, Random rnd)
        {
            var (newServer, singleResult) = Server.Change(change, rnd);
            Server = newServer;
            AccumalatedResult = AccumalatedResult.AddSingleRsult(singleResult);
            return AccumalatedResult;
        }

    }
}
