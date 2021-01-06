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
    public sealed class VectorNode : AbstractNode
    {
        public ConvexBound ConvexBound { get; }
        public double ConvexValue { get; private set; }

        public VectorNode(Vector referencePoint, ConvexBound convexBound, int nodeId, int vectorLength) : base(referencePoint, nodeId, vectorLength)
        {
            ConvexBound = convexBound;
            ThingsChangedUpdateState();
        }

        public static VectorNode Create(Vector initialVector, ConvexBound convexBound, int nodeId, int vectorLength) 
            => new VectorNode(initialVector, convexBound, nodeId, vectorLength);

        protected override void ThingsChangedUpdateState()
        {
            ConvexValue = ConvexBound.Compute(LocalVector);
        }

        public static Either<(NodeServer<VectorNode>, Communication), Communication> ResolveNodes
            (NodeServer<VectorNode> server, VectorNode[] nodes, Random rnd)
        {

            var convexFunction = nodes[0].ConvexBound;
            var violatedNodesIndices = nodes.IndicesWhere(n => !convexFunction.IsInBound(n.ConvexValue));
            if (violatedNodesIndices.Count == 0)
                return (server, Communication.Zero);

            var referenceVector = nodes[0].ReferencePoint;
            var nodesIndicesToPollNext = new Stack<int>(Enumerable.Range(0, nodes.Length).Except(violatedNodesIndices).ToArray().ShuffleInPlace(rnd));
            var messages = violatedNodesIndices.Count;
           // var bandwidth = violatedNodesIndices.Sum(i => nodes[i].ChangeVector.CountNonZero());
            var bandwidth = violatedNodesIndices.Sum(i => nodes[i].VectorLength);
            var (udpMessages, udpBandwidth, latency) = violatedNodesIndices.Select(i => Communication.DataMessageVectorSize(nodes[i].VectorLength)).Aggregate(TupleUtils.Zeros(), TupleUtils.PointwiseAddKeepLast);
            while (nodesIndicesToPollNext.Count > 0)
            {
                var nextViolatedNode = nodesIndicesToPollNext.Pop();
                //bandwidth += nodes[nextViolatedNode].ChangeVector.CountNonZero();
                var (controlMessages, controlBandwidth, controlLatency) = TupleUtils.PointwiseAdd(Communication.ControlMessage(0), Communication.DataMessageVectorSize(nodes[nextViolatedNode].VectorLength));
                udpBandwidth += controlBandwidth;
                udpMessages += controlMessages;
                latency += controlLatency;
                bandwidth += nodes[nextViolatedNode].VectorLength + 1;
                messages += 2;
                violatedNodesIndices.Add(nextViolatedNode);
                var averageChangeVector = Vector.AverageVector(violatedNodesIndices.Map(i => nodes[i].ChangeVector));
                if (convexFunction.IsInBound(convexFunction.Compute(referenceVector + averageChangeVector)))
                {
                    foreach (var nodeIndex in violatedNodesIndices)
                        nodes[nodeIndex].ChangeChangeVector(averageChangeVector.Clone());

                    messages  += violatedNodesIndices.Count;
                    var (mes, band, lat) = Communication.DataMessageVectorSize(nodes[0].VectorLength);
                    latency += lat;
                    udpBandwidth += band * violatedNodesIndices.Count;
                    udpMessages += mes * violatedNodesIndices.Count;

                    //bandwidth += violatedNodesIndices.Count * averageChangeVector.CountNonZero();
                    bandwidth += violatedNodesIndices.Count * nodes[0].VectorLength;
                    return (server, new Communication(bandwidth, messages, udpBandwidth, udpMessages, latency));
                }
            }

            return new Communication(bandwidth, messages, udpBandwidth, udpMessages, latency);
        }

        public static Communication FullSyncAdditionalCost(VectorNode[] nodes)
        {
            var (udpMessages, udpBandwidth, latency) = nodes.Select(n => Communication.DataMessageVectorSize(n.VectorLength))
                                                   .Aggregate(TupleUtils.PointwiseAddKeepLast);
            return new Communication(nodes.Sum(n => n.VectorLength), nodes.Length, udpBandwidth, udpMessages, latency);
        }
    }
}
