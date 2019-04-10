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
    public class DistanceNode : AbstractNode
    {
        public int Norm { get; }
        public ConvexBound ConvexBound { get; }
        public double RealDistance { get; protected set; }
        public double SlackDistance { get; protected set; }
        public double UsedDistance => RealDistance + SlackDistance;
        //public Vector<double> ResidualVector { get; protected set; }

        public DistanceNode(Vector<double> referencePoint, ConvexBound convexBound, double slackDistance, int norm, int nodeId) : base(referencePoint, nodeId)
        {
            ConvexBound = convexBound;
            SlackDistance = slackDistance;
            Norm = norm;
            ThingsChangedUpdateState();

        }
        public static Func<Vector<double>, ConvexBound, int, DistanceNode> CreateNorm(int norm) 
            => (initialVector, convexBound, nodeId) => new DistanceNode(initialVector, convexBound, 0.0, norm, nodeId);

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
            var nodesIndicesToPollNext = new Stack<int>(Enumerable.Range(0, nodes.Length).Except(violatedNodesIndices).ToArray().ShuffleInPlace(rnd));
            while (nodesIndicesToPollNext.Count > 0)
            {
                bandwidth += 1;
                messages += 2;
                violatedNodesIndices.Add(nodesIndicesToPollNext.Pop());
                var averageDistance = violatedNodesIndices.Average(i => nodes[i].UsedDistance);
                if (averageDistance <= 0.0)
                {
                    foreach (var nodeIndex in violatedNodesIndices)
                        nodes[nodeIndex].SlackDistance = averageDistance - nodes[nodeIndex].RealDistance;
                    bandwidth += violatedNodesIndices.Count;
                    messages += violatedNodesIndices.Count;
                    return (server, new Communication(bandwidth, messages));
                }
            }
            
            return new Communication(bandwidth, messages);
        }

        public static Communication FullSyncAdditionalCost(DistanceNode[] nodes)
            //  => new Communication(nodes.Sum(n => n.ChangeVector.CountNonZero()) + nodes.Length * nodes.Map(n => n.ChangeVector).AverageVector().CountNonZero(), nodes.Length * 3);
            => new Communication(nodes.Sum(n => n.ChangeVector.Count) * 2, nodes.Length * 3);
    }
}
