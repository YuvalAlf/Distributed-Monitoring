using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.Servers;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    public sealed class VectorNode : AbstractNode
    {
        public ConvexBound ConvexBound { get; }
        public double ConvexValue { get; private set; }

        public VectorNode(Vector<double> referencePoint, ConvexBound convexBound) : base(referencePoint)
        {
            ConvexBound = convexBound;
            ThingsChangedUpdateState();
        }

        public static VectorNode Create(Vector<double> initialVector, ConvexBound convexBound) => new VectorNode(initialVector, convexBound);

        protected override void ThingsChangedUpdateState()
        {
            ConvexValue = ConvexBound.Compute(LocalVector);
        }
        public static Either<(NodeServer<VectorNode>, CommunicationPrice), CommunicationPrice> ResolveNodes
            (NodeServer<VectorNode> server, VectorNode[] nodes, Random rnd)
        {
            var convexFunction = nodes[0].ConvexBound;
            var violatedNodesIndices = nodes.IndicesWhere(n => !convexFunction.IsInBound(n.ConvexValue));
            if (violatedNodesIndices.Count == 0)
                return (server, CommunicationPrice.Zero);

            var referenceVector = nodes[0].ReferencePoint;
            var nodesIndicesToPollNext = new Stack<int>(Enumerable.Range(0, nodes.Length).Except(violatedNodesIndices).ToArray().ShuffleInPlace(rnd));
            var messages = violatedNodesIndices.Count;
            var bandwidth = violatedNodesIndices.Count * server.VectorLength;
            while (nodesIndicesToPollNext.Count > 0)
            {
                bandwidth += server.VectorLength;
                messages += 2;
                violatedNodesIndices.Add(nodesIndicesToPollNext.Pop());
                var averageChangeVector = violatedNodesIndices.Map(i => nodes[i].ChangeVector).AverageVector();
                if (convexFunction.IsInBound(convexFunction.Compute(referenceVector + averageChangeVector)))
                {
                    foreach (var nodeIndex in violatedNodesIndices)
                        nodes[nodeIndex].ChangeChangeVector(averageChangeVector.Clone());
                    messages  += violatedNodesIndices.Count;
                    bandwidth += violatedNodesIndices.Count * server.VectorLength;
                    return (server, new CommunicationPrice(bandwidth, messages));
                }
            }

            return new CommunicationPrice(bandwidth, messages);
        }


        public override CommunicationPrice FullSyncAdditionalCost(int numOfNodes, int vectorLength)
            => new CommunicationPrice(numOfNodes * vectorLength, numOfNodes);
    }
}
