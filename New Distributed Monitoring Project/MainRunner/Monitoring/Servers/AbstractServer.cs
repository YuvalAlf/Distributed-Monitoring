using System;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.VectorType;
using Utils.TypeUtils;

namespace Monitoring.Servers
{
    public abstract class AbstractServer<InheritedType>
    {
        public Vector<double> GlobalVector => GlobalVectorType.GetValue(NodesVectors);
        public double FunctionValue => Function(GlobalVector);
        public double[] NodesFunctionValues => NodesVectors.Map(Function);

        public Vector<double>[] NodesVectors { get; }
        public int NumOfNodes { get; }
        public int VectorLength { get; }
        public GlobalVectorType GlobalVectorType { get; }
        public double UpperBound { get; }
        public double LowerBound { get; }
        public Func<Vector<double>, double> Function { get; }
        public EpsilonType Epsilon { get; }

        protected AbstractServer(Vector<double>[] nodesVectors, int numOfNodes, int vectorLength, GlobalVectorType globalVectorType, double upperBound, double lowerBound, Func<Vector<double>, double> function, EpsilonType epsilon)
        {
            NodesVectors = nodesVectors;
            NumOfNodes = numOfNodes;
            VectorLength = vectorLength;
            GlobalVectorType = globalVectorType;
            UpperBound = upperBound;
            LowerBound = lowerBound;
            Function = function;
            Epsilon = epsilon;
        }

        public (InheritedType, SingleResult) Change(Vector<double>[] changeMatrix, Random rnd)
        {
            for (int i = 0; i < NodesVectors.Length; i++)
                NodesVectors[i].AddInPlace(changeMatrix[i]);

            return LocalChange(changeMatrix, rnd);
        }



        public abstract (InheritedType, SingleResult) LocalChange(Vector<double>[] changeMatrix, Random rnd);

        public SingleResult NoBandwidthResult() => SingleResult.Nothing(FunctionValue, LowerBound, UpperBound, NodesFunctionValues);
        public abstract SingleResult FullResolutionBandwidthResult();
    }
}
