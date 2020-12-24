using System;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.Approximation;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.Servers
{
    public abstract class AbstractServer<InheritedType>
        where InheritedType : AbstractServer<InheritedType>
    {
        public Vector GlobalVector => GlobalVectorType.GetValue(NodesVectors);
        public double FunctionValue => Function(GlobalVector);
        public double[] NodesFunctionValues => NodesVectors.Map(Function);

        public Vector[] NodesVectors { get; }
        public int NumOfNodes { get; }
        public int VectorLength { get; }
        public GlobalVectorType GlobalVectorType { get; }
        public double UpperBound { get; }
        public double LowerBound { get; }
        public Func<Vector, double> Function { get; }
        public ApproximationType Approximation { get; }

        protected AbstractServer(Vector[] nodesVectors, int numOfNodes, int vectorLength, GlobalVectorType globalVectorType, double upperBound, double lowerBound, Func<Vector, double> function, ApproximationType approximation)
        {
            NodesVectors = nodesVectors;
            NumOfNodes = numOfNodes;
            VectorLength = vectorLength;
            GlobalVectorType = globalVectorType;
            UpperBound = upperBound;
            LowerBound = lowerBound;
            Function = function;
            Approximation = approximation;
        }

        public (InheritedType, SingleResult) Change(Vector[] changeMatrix, Random rnd)
        {
            for (int i = 0; i < NodesVectors.Length; i++)
                NodesVectors[i].AddInPlace(changeMatrix[i]);
            var (newServer, communication, fullSync) = LocalChange(changeMatrix, rnd);
            return (newServer, newServer.CreateResult(communication, fullSync));
        }

        protected abstract (InheritedType, Communication, bool fullSync) LocalChange(Vector[] changeMatrix, Random rnd);

        private SingleResult CreateResult(Communication communication, bool isFullSync) 
            => new SingleResult(communication, isFullSync, FunctionValue, UpperBound, LowerBound, NodesFunctionValues);
    }
}
