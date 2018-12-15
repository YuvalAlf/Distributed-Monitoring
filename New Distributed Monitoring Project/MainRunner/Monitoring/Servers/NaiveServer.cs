using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.Servers
{
    public sealed class NaiveServer : AbstractServer<NaiveServer>
    {
        public NaiveServer(Vector[] nodesVectors, int numOfNodes, int vectorLength, GlobalVectorType globalVectorType, double upperBound, double lowerBound, Func<Vector, double> function, EpsilonType epsilonType)
            : base(nodesVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, function, epsilonType)
        { }

        protected override (NaiveServer, Communication, bool fullSync) LocalChange(Vector[] changeMatrix, Random rnd)
        {
            var (lowerBound, upperBound) = base.Epsilon.Calc(FunctionValue);
            var newNaiveServer = new NaiveServer(NodesVectors, NumOfNodes, VectorLength, GlobalVectorType, upperBound, lowerBound, Function, Epsilon);

            var numberOfMessages = NumOfNodes;
           // var bandwidth        = changeMatrix.Sum(v => v.CountNonZero());
            var bandwidth        = changeMatrix.Sum(v => VectorLength);

            return (newNaiveServer, new Communication(bandwidth, numberOfMessages), true);
        }

        public static NaiveServer Create(
            Vector[]  initVectors,
            int               numOfNodes,
            int               vectorLength,
            GlobalVectorType  globalVectorType,
            EpsilonType       epsilon,
            MonitoredFunction monitoredFunction)
        {
            initVectors = initVectors.Map(v => v.Clone());
            var globalVector = globalVectorType.GetValue(initVectors);
            var (lowerBound, upperBound) = epsilon.Calc(monitoredFunction.Function(globalVector));

            return new NaiveServer(initVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, monitoredFunction.Function, epsilon);
        }
    }
}
