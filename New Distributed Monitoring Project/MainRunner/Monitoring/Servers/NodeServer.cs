using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Approximation;
using Monitoring.GeometricMonitoring.VectorType;
using Monitoring.Nodes;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.Servers
{
    public sealed class NodeServer<NodeType> : AbstractServer<NodeServer<NodeType>>
        where NodeType : AbstractNode
    {
        public NodeType[] UpperNodes { get; }
        public NodeType[] LowerNodes { get; }
        public ResolveNodesFunction<NodeType> ResolveNodes { get; }
        public Func<Vector[], NodeServer<NodeType>> ReCreate { get; }

        public NodeServer(Vector[]                             nodesVectors,
                          int                                  numOfNodes, int vectorLength,
                          GlobalVectorType                     globalVectorType,
                          double                               upperBound, double lowerBound,
                          Func<Vector, double>                 function,
                          ApproximationType                    approximation, NodeType[] upperNodes,
                          NodeType[]                           lowerNodes,
                          ResolveNodesFunction<NodeType>       resolveNodes,
                          Func<Vector[], NodeServer<NodeType>> reCreate)
            : base(nodesVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, function,
                   approximation)
        {
            UpperNodes   = upperNodes;
            LowerNodes   = lowerNodes;
            ResolveNodes = resolveNodes;
            ReCreate     = reCreate;
        }

        protected override (NodeServer<NodeType>, Communication, bool fullSync) LocalChange(Vector[] changeMatrix, Random rnd)
        {
            double mulBy = GlobalVectorType.MulBy(NumOfNodes);
            for (int nodeNum = 0; nodeNum < NumOfNodes; nodeNum++)
            {
                this.UpperNodes[nodeNum].Change(changeMatrix[nodeNum] * mulBy);
                this.LowerNodes[nodeNum].Change(changeMatrix[nodeNum] * mulBy);
            }
            var (newServerLowerResolved, lowerCommunication, isFullSync1) = this.Resolve(this.LowerNodes, rnd);
            var (newServerAllResolved, upperCommunication, isFullSync2) = newServerLowerResolved.Resolve(newServerLowerResolved.UpperNodes, rnd);

            if (newServerAllResolved.FunctionValue > newServerAllResolved.UpperBound)
                newServerAllResolved = newServerLowerResolved;

            return (newServerAllResolved, lowerCommunication.Add(upperCommunication), isFullSync1 || isFullSync2);
        }

        private readonly Lazy<MethodInfo> fullSyncCost = new Lazy<MethodInfo>(() => typeof(NodeType).GetMethod("FullSyncAdditionalCost"));

        public (NodeServer<NodeType>, Communication, bool isFullSync) Resolve(NodeType[] nodes, Random rnd)
        {
            var result = this.ResolveNodes(this, nodes, rnd);

            if (result.IsChoice2)
            {
                var parameters = new []{nodes as object};
                var additionalCost = fullSyncCost.Value.Invoke(null, parameters) as Communication;
                return (this.ReCreate(this.NodesVectors), additionalCost.Add(result.GetChoice2), true);
            }
                

            var server = result.GetChoice1.Item1;
            return result.GetChoice1.AddLast(false);
        }

        public static NodeServer<NodeType> Create(
            Vector[] initVectors, 
            int numOfNodes, 
            int vectorLength, 
            GlobalVectorType globalVectorType,
            ApproximationType approximation, 
            MonitoredFunction monitoredFunction,
            ResolveNodesFunction<NodeType> resolveNodes, 
            Func<Vector, ConvexBound, int, int, NodeType> createNode)
        {
            initVectors = initVectors.Map(v => v.Clone());
            var globalVector = globalVectorType.GetValue(initVectors);
            var (lowerBound, upperBound) = approximation.Calc(monitoredFunction.Function(globalVector));
            var upperConvexBound = monitoredFunction.UpperBound(globalVector, upperBound);
            var lowerConvexBound = monitoredFunction.LowerBound(globalVector, lowerBound);
            var upperNodes = ArrayUtils.Init(numOfNodes, i => createNode(globalVector.Clone(), upperConvexBound, i, vectorLength));
            var lowerNodes = ArrayUtils.Init(numOfNodes, i => createNode(globalVector.Clone(), lowerConvexBound, i, vectorLength));
            NodeServer<NodeType> ReCreate(Vector[] newInitVectors)
                => Create(newInitVectors, numOfNodes, vectorLength, globalVectorType, approximation, monitoredFunction, resolveNodes, createNode);

            return new NodeServer<NodeType>(initVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, monitoredFunction.Function,
                approximation, upperNodes, lowerNodes, resolveNodes, ReCreate);
        }
    }
}
