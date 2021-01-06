using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.Servers;
using Utils.AiderTypes;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    public sealed class OracleVectorNode : AbstractNode
    {
        public ConvexBound ConvexBound { get; }
        public Func<Vector, double> MonitoredFunction => ConvexBound.MonitoredFunction;
        public double FunctionValue { get; private set; }

        public OracleVectorNode(Vector referencePoint, ConvexBound convexBound, int nodeId, int vectorLength) 
            : base(referencePoint, nodeId, vectorLength)
        {
            ConvexBound = convexBound;
            ThingsChangedUpdateState();
        }

        public static OracleVectorNode Create(Vector initialVector, ConvexBound convexBound, int nodeId, int vectorLength)
            => new OracleVectorNode(initialVector, convexBound, nodeId, vectorLength);

        protected override void ThingsChangedUpdateState()
        {
            FunctionValue = MonitoredFunction(LocalVector);
        }

        public static Either<(NodeServer<OracleVectorNode>, Communication), Communication> ResolveNodes
            (NodeServer<OracleVectorNode> server, OracleVectorNode[] nodes, Random rnd)
        {
            var convexFunction = nodes[0].ConvexBound;
            var monitoredFunction = nodes[0].MonitoredFunction;
            var violatedNodesIndices = nodes.IndicesWhere(n => !n.ConvexBound.IsInBound(n.FunctionValue));
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
                var (controlMessages, controlBandwidth, controlLatency) =  TupleUtils.PointwiseAdd(Communication.ControlMessage(0), Communication.DataMessageVectorSize(nodes[nextViolatedNode].VectorLength));
                udpBandwidth                            += controlBandwidth;
                udpMessages                             += controlMessages;
                latency                                 += controlLatency;
                bandwidth += nodes[nextViolatedNode].VectorLength + 1;
                messages += 2;
                violatedNodesIndices.Add(nextViolatedNode);
                var averageChangeVector = Vector.AverageVector(violatedNodesIndices.Map(i => nodes[i].ChangeVector));
                if (convexFunction.IsInBound(monitoredFunction(referenceVector + averageChangeVector)))
                {
                    foreach (var nodeIndex in violatedNodesIndices)
                        nodes[nodeIndex].ChangeChangeVector(averageChangeVector.Clone());

                    var (mes, band, lat) =  Communication.DataMessageVectorSize(nodes[nextViolatedNode].VectorLength);
                    latency += lat;
                    udpBandwidth    += band * violatedNodesIndices.Count;
                    udpMessages     += mes  * violatedNodesIndices.Count;
                    messages += violatedNodesIndices.Count;
                    //bandwidth += violatedNodesIndices.Count * averageChangeVector.CountNonZero();
                    bandwidth += violatedNodesIndices.Count * nodes[0].VectorLength;
                    return (server, new Communication(bandwidth, messages, udpBandwidth, udpMessages, latency));
                }
            }

            return new Communication(bandwidth, messages, udpBandwidth, udpMessages, latency);
        }

        public static Communication FullSyncAdditionalCost(OracleVectorNode[] nodes)
        {
            var (udpMessages, udpBandwidth, latency) = nodes.Select(n => Communication.DataMessageVectorSize(n.VectorLength))
                                                   .Aggregate(TupleUtils.PointwiseAddKeepLast);
            return new Communication(nodes.Sum(n => n.VectorLength), nodes.Length, udpBandwidth, udpMessages, latency);
        }

    }
}
