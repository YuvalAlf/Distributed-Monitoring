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
    public class DistanceNode : AbstractNode
    {
        public int Norm { get; }
        public ConvexBound ConvexBound { get; }
        public double RealDistance { get; protected set; }
        public double SlackDistance { get; protected set; }
        public double UsedDistance => RealDistance + SlackDistance;
        //public Vector<double> ResidualVector { get; protected set; }

        public DistanceNode(Vector referencePoint, ConvexBound convexBound, double slackDistance, int norm, int nodeId, int vectorLength) : base(referencePoint, nodeId, vectorLength)
        {
            ConvexBound = convexBound;
            SlackDistance = slackDistance;
            Norm = norm;
            ThingsChangedUpdateState();

        }
        public static Func<Vector, ConvexBound, int, int, DistanceNode> CreateNorm(int norm) 
            => (initialVector, convexBound, nodeId, vectorLength) => new DistanceNode(initialVector, convexBound, 0.0, norm, nodeId, vectorLength);

        protected override void ThingsChangedUpdateState()
        {
           // (RealDistance, ResidualVector) = ConvexBound.ComputeDistance(Norm, LocalVector);
            RealDistance = ConvexBound.ComputeDistance(Norm, LocalVector, this.NodeId);
        }
        
        public static Either<(NodeServer<TNode>, Communication), Communication> ResolveNodes<TNode>
            (NodeServer<TNode> server, TNode[] nodes, Random rnd)
        where TNode : DistanceNode
        {
            var violatedNodesIndices = nodes.IndicesWhere(n => n.UsedDistance > 0);
            if (violatedNodesIndices.Count == 0)
                return (server, Communication.Zero);

            var bandwidth = violatedNodesIndices.Count;
            var messages = violatedNodesIndices.Count;
            var (udpMessages, udpBandwidth, latency) = violatedNodesIndices.Select(i => Communication.ControlMessage(8)).Aggregate(TupleUtils.Zeros(), TupleUtils.PointwiseAddKeepLast);


            var nodesIndicesToPollNext = new Stack<int>(Enumerable.Range(0, nodes.Length).Except(violatedNodesIndices).ToArray().ShuffleInPlace(rnd));
            while (nodesIndicesToPollNext.Count > 0)
            {
                var (controlMessage, controlBandwidth, controlLatency) =  Communication.ControlMessage(8);
                latency                                += 2 * controlLatency;
                udpBandwidth                           += 2 * controlBandwidth;
                udpMessages                            += 2 * controlMessage;

                bandwidth += 2;
                messages += 2;
                violatedNodesIndices.Add(nodesIndicesToPollNext.Pop());
                var averageDistance = violatedNodesIndices.Average(i => nodes[i].UsedDistance);
                if (averageDistance <= 0.0)
                {
                    foreach (var nodeIndex in violatedNodesIndices)
                        nodes[nodeIndex].SlackDistance = averageDistance - nodes[nodeIndex].RealDistance;

                    latency      += controlLatency;
                    udpBandwidth += violatedNodesIndices.Count * controlBandwidth;
                    udpMessages  += violatedNodesIndices.Count * controlMessage;

                    bandwidth += violatedNodesIndices.Count;
                    messages += violatedNodesIndices.Count;
                    return (server, new Communication(bandwidth, messages, udpBandwidth, udpMessages, latency));
                }
            }

            return new Communication(bandwidth, messages, udpBandwidth, udpMessages, latency);
        }

        public static Communication FullSyncAdditionalCost(DistanceNode[] nodes)
        {
            var bandwidth = nodes.Sum(n => n.VectorLength) * 2;
            var messages  = nodes.Length                   * 3;
            var (dataUdpMessages, dataUdpBandwidth, dataLatency) = nodes.Select(n => Communication.DataMessageVectorSize(n.VectorLength))
                                                           .Aggregate(TupleUtils.PointwiseAddKeepLast);

            var (controlUdpMessages, controlUdpBandwidth, controlLatency) = nodes.Select(n => Communication.ControlMessage(0))
                                                                 .Aggregate(TupleUtils.PointwiseAddKeepLast);
            var latency = controlLatency + 2 * dataLatency;
            //  => new Communication(nodes.Sum(n => n.ChangeVector.CountNonZero()) + nodes.Length * nodes.Map(n => n.ChangeVector).AverageVector().CountNonZero(), nodes.Length * 3);
            return new Communication(bandwidth, messages, dataUdpBandwidth * 2 + controlUdpBandwidth, dataUdpMessages * 2 + controlUdpMessages, latency);
        }
    }
}
