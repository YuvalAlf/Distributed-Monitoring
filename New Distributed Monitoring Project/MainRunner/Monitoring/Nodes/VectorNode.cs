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
        public static (NodeServer<VectorNode>, SingleResult) ResolveNodes(NodeServer<VectorNode> server, VectorNode[] nodes, Random rnd)
        {
            var convexFunction = nodes[0].ConvexBound;
            var violatedNodesIndices = nodes.IndicesWhere(n => !convexFunction.IsInBound(n.ConvexValue));
            if (violatedNodesIndices.Count == 0)
                return (server, server.NoBandwidthResult());

            var referenceVector = nodes[0].ReferencePoint;
            var initiallyViolated = violatedNodesIndices.Count;
            var nodesIndicesToPollNext = new Stack<int>(Enumerable.Range(0, nodes.Length).Except(violatedNodesIndices).ToArray().Shuffle(rnd));
            while (nodesIndicesToPollNext.Count > 0)
            {
                violatedNodesIndices.Add(nodesIndicesToPollNext.Pop());
                var averageChangeVector = violatedNodesIndices.Map(i => nodes[i].ChangeVector).AverageVector();
                if (convexFunction.IsInBound(convexFunction.Compute(referenceVector + averageChangeVector)))
                {
                    foreach (var nodeIndex in violatedNodesIndices)
                        nodes[nodeIndex].ChangeVector = averageChangeVector.Clone();
                    var numOfChannels = violatedNodesIndices.Count;
                    var numOfMessages = violatedNodesIndices.Count * 3 - initiallyViolated;
                    var bandwidth = 2 * violatedNodesIndices.Count * server.VectorLength + (violatedNodesIndices.Count - initiallyViolated);
                    var result = new SingleResult(bandwidth, numOfMessages, numOfChannels, false, server.FunctionValue, server.UpperBound, server.LowerBound, server.NodesFunctionValues);
                    return (server, result);
                }
            }

            var fullyResolvedServer = server.ReCreate(server.NodesVectors);
            return (fullyResolvedServer, fullyResolvedServer.FullResolutionBandwidthResult());
        }
    }
}
