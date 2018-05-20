﻿using System;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.TypeUtils;

namespace Monitoring.Servers
{
    public sealed class OracleServer : AbstractServer<OracleServer>
    {
        public OracleServer(Vector<double>[] nodesVectors, int numOfNodes, int vectorLength, GlobalVectorType globalVectorType, double upperBound, double lowerBound, Func<Vector<double>, double> function, EpsilonType epsilonType)
            : base(nodesVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, function, epsilonType)
        {}

        public override (OracleServer, SingleResult) LocalChange(Vector<double>[] changeMatrix, Random rnd)
        {
            if (FunctionValue <= UpperBound && FunctionValue >= LowerBound)
                return (this, base.NoBandwidthResult());

            var (lowerBound, upperBound) = base.Epsilon.Calc(FunctionValue);
            var newOracleServer = new OracleServer(NodesVectors, NumOfNodes, VectorLength, GlobalVectorType, upperBound, lowerBound, Function, Epsilon);

            return (newOracleServer, newOracleServer.FullResolutionBandwidthResult());
        }

        public override SingleResult FullResolutionBandwidthResult()
        {
            var numberOfChannels = NumOfNodes;
            var numberOfMessages = numberOfChannels * 2;
            var bandwidth = numberOfMessages * this.VectorLength;
            return new SingleResult(bandwidth, numberOfMessages, numberOfChannels, true, FunctionValue, UpperBound, LowerBound, NodesFunctionValues);
        }

        public static OracleServer Create(
            Vector<double>[] initVectors,
            int numOfNodes,
            int vectorLength,
            GlobalVectorType globalVectorType,
            EpsilonType epsilon,
            MonitoredFunction monitoredFunction)
        {
            initVectors = initVectors.Map(v => v.Clone());
            var globalVector = globalVectorType.GetValue(initVectors);
            var (lowerBound, upperBound) = epsilon.Calc(monitoredFunction.Function(globalVector));

            return new OracleServer(initVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, monitoredFunction.Function, epsilon);
        }
    }
}
