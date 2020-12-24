using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Approximation;
using Monitoring.GeometricMonitoring.VectorType;
using MoreLinq;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.Servers
{
    public sealed class OracleServer : AbstractServer<OracleServer>
    {
        private Vector[] CurrentChanges;

        private void Init() => CurrentChanges = Vector.Init(NumOfNodes);

        public OracleServer(Vector[]             nodesVectors,     int         numOfNodes, int    vectorLength,
                            GlobalVectorType     globalVectorType, double      upperBound, double lowerBound,
                            Func<Vector, double> function,         ApproximationType approximation)
            : base(nodesVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, function,
                   approximation)
        {
            Init();
        }



        protected override (OracleServer, Communication, bool fullSync) LocalChange(Vector[] changeMatrix, Random rnd)
        {
            CurrentChanges.ForEach(((vector, i) => vector.AddInPlace(changeMatrix[i])));
            if (FunctionValue <= UpperBound && FunctionValue >= LowerBound)
                return (this, Communication.Zero, false);

            var (lowerBound, upperBound) = base.Approximation.Calc(FunctionValue);
            var newOracleServer = new OracleServer(NodesVectors, NumOfNodes, VectorLength, GlobalVectorType, upperBound, lowerBound, Function, Approximation);

            var messages  = 2 * NumOfNodes;
           // var bandwidth = CurrentChanges.Sum(c => c.CountNonZero()) + NumOfNodes * CurrentChanges.SumVector().CountNonZero();
            var bandwidth = CurrentChanges.Sum(c => VectorLength) * 2;
            var (udpMessages, udpBandwidth) = changeMatrix.Select(v => Communication.DataMessage(VectorLength)).Aggregate(TupleUtils.PointwiseAdd);
            Init();
            return (newOracleServer, new Communication(bandwidth, messages, 2 * udpBandwidth, 2 * udpMessages, 2 * Communication.OneWayLatencyMs), true);
        }

        public static OracleServer Create(
            Vector[] initVectors,
            int numOfNodes,
            int vectorLength,
            GlobalVectorType globalVectorType,
            ApproximationType approximation,
            MonitoredFunction monitoredFunction)
        {
            initVectors = initVectors.Map(v => v.Clone());
            var globalVector = globalVectorType.GetValue(initVectors);
            var (lowerBound, upperBound) = approximation.Calc(monitoredFunction.Function(globalVector));

            return new OracleServer(initVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, monitoredFunction.Function, approximation);
        }
    }
}
