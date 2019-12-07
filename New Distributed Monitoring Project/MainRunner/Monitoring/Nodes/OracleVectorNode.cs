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
            while (nodesIndicesToPollNext.Count > 0)
            {
                var nextViolatedNode = nodesIndicesToPollNext.Pop();
                //bandwidth += nodes[nextViolatedNode].ChangeVector.CountNonZero();
                bandwidth += nodes[nextViolatedNode].VectorLength;
                messages += 2;
                violatedNodesIndices.Add(nextViolatedNode);
                var averageChangeVector = Vector.AverageVector(violatedNodesIndices.Map(i => nodes[i].ChangeVector));
                if (convexFunction.IsInBound(monitoredFunction(referenceVector + averageChangeVector)))
                {
                    foreach (var nodeIndex in violatedNodesIndices)
                        nodes[nodeIndex].ChangeChangeVector(averageChangeVector.Clone());

                    messages += violatedNodesIndices.Count;
                    //bandwidth += violatedNodesIndices.Count * averageChangeVector.CountNonZero();
                    bandwidth += violatedNodesIndices.Count * nodes[0].VectorLength;
                    return (server, new Communication(bandwidth, messages));
                }
            }

            return new Communication(bandwidth, messages);
        }

        public static Communication FullSyncAdditionalCost(OracleVectorNode[] nodes)
            => new Communication(nodes.Sum(n => n.VectorLength), nodes.Length);

    }
}
