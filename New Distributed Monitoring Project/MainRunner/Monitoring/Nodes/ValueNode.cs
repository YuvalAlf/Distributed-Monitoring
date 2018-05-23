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
    public sealed class ValueNode : AbstractNode
    {
        public ConvexBound ConvexBound { get; }
        public double RealValue { get; private set; }
        public double SlackValue { get; private set; }
        public double ConvexValue => RealValue + SlackValue;

        public ValueNode(Vector<double> referencePoint, ConvexBound convexBound, double slackValue) : base(
            referencePoint)
        {
            ConvexBound = convexBound;
            SlackValue = slackValue;
            ThingsChangedUpdateState();
        }

        public static ValueNode Create(Vector<double> initialVector, ConvexBound convexBound)
            => new ValueNode(initialVector, convexBound, 0);

        protected override void ThingsChangedUpdateState()
        {
            RealValue = ConvexBound.Compute(LocalVector);
        }
        public static (NodeServer<ValueNode>, SingleResult) ResolveNodes(NodeServer<ValueNode> server, ValueNode[] nodes, Random rnd)
        {
            var convexBound = nodes[0].ConvexBound;
            var violatedNodesIndices = nodes.IndicesWhere(n => !convexBound.IsInBound(n.ConvexValue));
            if (violatedNodesIndices.Count == 0)
                return (server, server.NoBandwidthResult());

            var initiallyViolated = violatedNodesIndices.Count;
            var nodesIndicesToPollNext = new Stack<int>(Enumerable.Range(0, nodes.Length).Except(violatedNodesIndices).ToArray().Shuffle(rnd));
            while (nodesIndicesToPollNext.Count > 0)
            {
                violatedNodesIndices.Add(nodesIndicesToPollNext.Pop());
                var averageValue = violatedNodesIndices.Average(i => nodes[i].ConvexValue);
                if (convexBound.IsInBound(averageValue))
                {
                    foreach (var nodeIndex in violatedNodesIndices)
                        nodes[nodeIndex].SlackValue = averageValue - nodes[nodeIndex].RealValue;
                    var numOfChannels = violatedNodesIndices.Count;
                    var numOfMessages = violatedNodesIndices.Count * 3 - initiallyViolated;
                    var bandwidth = numOfMessages;
                    var result = new SingleResult(bandwidth, numOfMessages, numOfChannels, false, server.FunctionValue, server.UpperBound, server.LowerBound, server.NodesFunctionValues);
                    return (server, result);
                }
            }

            var fullyResolvedServer = server.ReCreate(server.NodesVectors);
            return (fullyResolvedServer, fullyResolvedServer.FullResolutionBandwidthResult());
        }
    }
}
