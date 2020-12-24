using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.Servers;
using Utils.AiderTypes;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    public class ValueNode : AbstractNode
    {
        public ConvexBound ConvexBound { get; }
        public double RealValue { get; protected set; }
        public double SlackValue { get; protected set; }
        public double ConvexValue => RealValue + SlackValue;

        public ValueNode(Vector referencePoint, ConvexBound convexBound, double slackValue, int nodeId, int vectorLength) 
            : base(referencePoint, nodeId, vectorLength)
        {
            ConvexBound = convexBound;
            SlackValue = slackValue;
            ThingsChangedUpdateState();
        }

        public static ValueNode Create(Vector initialVector, ConvexBound convexBound, int nodeId, int vectorLength)
            => new ValueNode(initialVector, convexBound, 0, nodeId, vectorLength);

        protected override void ThingsChangedUpdateState()
        {
            RealValue = ConvexBound.Compute(LocalVector);
        }

        public static Either<(NodeServer<TValueNode>, Communication), Communication> ResolveNodes<TValueNode>
            (NodeServer<TValueNode> server, TValueNode[] nodes, Random rnd)
        where TValueNode : ValueNode
        {
            var convexBound = nodes[0].ConvexBound;
            var violatedNodesIndices = nodes.IndicesWhere(n => !convexBound.IsInBound(n.ConvexValue));
            if (violatedNodesIndices.Count == 0)
                return (server, Communication.Zero);


            var initiallyViolated = violatedNodesIndices.Count;
            var nodesIndicesToPollNext = new Stack<int>(Enumerable.Range(0, nodes.Length).Except(violatedNodesIndices).ToArray().ShuffleInPlace(rnd));


            var bandwidth = violatedNodesIndices.Count;
            var messages  = violatedNodesIndices.Count;
            var latency = violatedNodesIndices.Count > 0 ? Communication.OneWayLatencyMs : 0;
            var (udpMessages, udpBandwidth) = violatedNodesIndices.Select(i => Communication.DataMessage(8)).Aggregate(TupleUtils.Zeros(), TupleUtils.PointwiseAdd);

            while (nodesIndicesToPollNext.Count > 0)
            {
                latency += 2 * Communication.OneWayLatencyMs;
                var (controlMessage, controlBandwidth) =  Communication.DataMessage(8);
                udpBandwidth                            += 2 * controlBandwidth;
                udpMessages                             += 2 * controlMessage;

                bandwidth += 2;
                messages  += 2;
                violatedNodesIndices.Add(nodesIndicesToPollNext.Pop());
                var averageValue = violatedNodesIndices.Average(i => nodes[i].ConvexValue);
                if (convexBound.IsInBound(averageValue))
                {
                    foreach (var nodeIndex in violatedNodesIndices)
                        nodes[nodeIndex].SlackValue = averageValue - nodes[nodeIndex].RealValue;
                    latency += Communication.OneWayLatencyMs;
                    udpBandwidth += violatedNodesIndices.Count * controlBandwidth;
                    udpMessages += violatedNodesIndices.Count * controlMessage;

                    messages  += violatedNodesIndices.Count;
                    bandwidth += violatedNodesIndices.Count;
                    return (server, new Communication(bandwidth, messages, udpBandwidth, udpMessages, latency));
                }
            }

            return new Communication(bandwidth, messages, udpBandwidth, udpMessages, latency);
        }

        public static Communication FullSyncAdditionalCost(ValueNode[] nodes)
        {
            var bandwidth = nodes.Sum(n => n.VectorLength) * 2;
            var messages = nodes.Length * 3;
            var (dataUdpMessages, dataUdpBandwidth) = nodes.Select(n => Communication.DataMessage(n.VectorLength))
                                                   .Aggregate(TupleUtils.PointwiseAdd);

            var (controlUdpMessages, controlUdpBandwidth) = nodes.Select(n => Communication.ControlMessage())
                                                            .Aggregate(TupleUtils.PointwiseAdd);
            var latency = Communication.OneWayLatencyMs * 3;
            //  => new Communication(nodes.Sum(n => n.ChangeVector.CountNonZero()) + nodes.Length * nodes.Map(n => n.ChangeVector).AverageVector().CountNonZero(), nodes.Length * 3);
            return new Communication(bandwidth, messages, dataUdpBandwidth * 2 + controlUdpBandwidth, dataUdpMessages * 2 + controlUdpMessages, latency);
        }
    }
}
