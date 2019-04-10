using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.VectorType;
using MoreLinq;
using Utils.TypeUtils;

namespace Monitoring.Servers
{
    public sealed class OracleServer : AbstractServer<OracleServer>
    {
        private Vector<double>[] CurrentChanges;

        private void Init() => CurrentChanges = ArrayUtils.Init(NumOfNodes, _ => VectorUtils.CreateVector(VectorLength, __ => 0.0));

        public OracleServer(Vector<double>[]             nodesVectors,     int         numOfNodes, int    vectorLength,
                            GlobalVectorType             globalVectorType, double      upperBound, double lowerBound,
                            Func<Vector<double>, double> function,         EpsilonType epsilonType)
            : base(nodesVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, function,
                   epsilonType)
        {
            Init();
        }



        protected override (OracleServer, Communication, bool fullSync) LocalChange(Vector<double>[] changeMatrix, Random rnd)
        {
            CurrentChanges.ForEach(((vector, i) => vector.AddInPlace(changeMatrix[i])));
            if (FunctionValue <= UpperBound && FunctionValue >= LowerBound)
                return (this, Communication.Zero, false);

            var (lowerBound, upperBound) = base.Epsilon.Calc(FunctionValue);
            var newOracleServer = new OracleServer(NodesVectors, NumOfNodes, VectorLength, GlobalVectorType, upperBound, lowerBound, Function, Epsilon);

            var messages  = 2 * NumOfNodes;
           // var bandwidth = CurrentChanges.Sum(c => c.CountNonZero()) + NumOfNodes * CurrentChanges.SumVector().CountNonZero();
            var bandwidth = CurrentChanges.Sum(c => c.Count) * 2;
            Init();
            return (newOracleServer, new Communication(bandwidth, messages), true);
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
