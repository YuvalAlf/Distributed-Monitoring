using System;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.GeometricMonitoring.Epsilon;
using Monitoring.GeometricMonitoring.VectorType;
using Monitoring.Nodes;
using Utils.TypeUtils;

namespace Monitoring.Servers
{
    public delegate Either<(NodeServer<TNode>, Communication), Communication> ResolveNodesFunction<TNode>
        (NodeServer<TNode> server, TNode[] nodes, Random rnd)
        where TNode: AbstractNode;


    public sealed class NodeServer<NodeType> : AbstractServer<NodeServer<NodeType>>
        where NodeType : AbstractNode
    {
        public NodeType[] UpperNodes { get; }
        public NodeType[] LowerNodes { get; }
        public ResolveNodesFunction<NodeType> ResolveNodes { get; }
        public Func<Vector<double>[], NodeServer<NodeType>> ReCreate { get; }

        public NodeServer(Vector<double>[]                             nodesVectors,
                          int                                          numOfNodes, int vectorLength,
                          GlobalVectorType                             globalVectorType,
                          double                                       upperBound, double lowerBound,
                          Func<Vector<double>, double>                 function,
                          EpsilonType                                  epsilon, NodeType[] upperNodes,
                          NodeType[]                                   lowerNodes,
                          ResolveNodesFunction<NodeType>               resolveNodes,
                          Func<Vector<double>[], NodeServer<NodeType>> reCreate)
            : base(nodesVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, function, epsilon)
        {
            UpperNodes   = upperNodes;
            LowerNodes   = lowerNodes;
            ResolveNodes = resolveNodes;
            ReCreate     = reCreate;
        }

        public override (NodeServer<NodeType>, Communication, bool fullSync) LocalChange(Vector<double>[] changeMatrix, Random rnd)
        {
            double mulBy = GlobalVectorType.MulBy(NumOfNodes);
            for (int nodeNum = 0; nodeNum < NumOfNodes; nodeNum++)
            {
                this.UpperNodes[nodeNum].Change(changeMatrix[nodeNum] * mulBy);
                this.LowerNodes[nodeNum].Change(changeMatrix[nodeNum] * mulBy);
            }
            var (newServerLowerResolved, lowerCommunication, isFullSync1) = this.Resolve(this.LowerNodes, rnd);
            var (newServerAllResolved, upperCommunication, isFullSync2) = newServerLowerResolved.Resolve(newServerLowerResolved.UpperNodes, rnd);
            return (newServerAllResolved, lowerCommunication.Add(upperCommunication), isFullSync1 || isFullSync2);
        }

        public (NodeServer<NodeType>, Communication, bool isFullSync) Resolve(NodeType[] nodes, Random rnd)
        {
            var result = this.ResolveNodes(this, nodes, rnd);
            if (result.IsChoice2)
                return (this.ReCreate(this.NodesVectors), nodes[0].FullSyncAdditionalCost(NumOfNodes, VectorLength).Add(result.GetChoice2), true);

            var server = result.GetChoice1.Item1;
            return result.GetChoice1.AddLast(false);
        }

        public static NodeServer<NodeType> Create(
            Vector<double>[] initVectors, 
            int numOfNodes, 
            int vectorLength, 
            GlobalVectorType globalVectorType,
            EpsilonType epsilon, 
            MonitoredFunction monitoredFunction,
            ResolveNodesFunction<NodeType> resolveNodes, 
            Func<Vector<double>, ConvexBound, NodeType> createNode)
        {
            initVectors = initVectors.Map(v => v.Clone());
            var globalVector = globalVectorType.GetValue(initVectors);
            var (lowerBound, upperBound) = epsilon.Calc(monitoredFunction.Function(globalVector));
            var upperConvexBound = monitoredFunction.UpperBound(globalVector, upperBound);
            var lowerConvexBound = monitoredFunction.LowerBound(globalVector, lowerBound);
            var upperNodes = Enumerable.Repeat(0, numOfNodes).Map(_ => createNode(globalVector.Clone(), upperConvexBound));
            var lowerNodes = Enumerable.Repeat(0, numOfNodes).Map(_ => createNode(globalVector.Clone(), lowerConvexBound));
            NodeServer<NodeType> ReCreate(Vector<double>[] newInitVectors)
                => Create(newInitVectors, numOfNodes, vectorLength, globalVectorType, epsilon, monitoredFunction, resolveNodes, createNode);

            return new NodeServer<NodeType>(initVectors, numOfNodes, vectorLength, globalVectorType, upperBound, lowerBound, monitoredFunction.Function,
                epsilon, upperNodes, lowerNodes, resolveNodes, ReCreate);
        }
    }
}
