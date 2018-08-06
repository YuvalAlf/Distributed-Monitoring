﻿using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.Servers;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    public class ValueNode : AbstractNode
    {
        public ConvexBound ConvexBound { get; }
        public double RealValue { get; protected set; }
        public double SlackValue { get; protected set; }
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
            while (nodesIndicesToPollNext.Count > 0)
            {
                bandwidth += 1;
                messages  += 2;
                violatedNodesIndices.Add(nodesIndicesToPollNext.Pop());
                var averageValue = violatedNodesIndices.Average(i => nodes[i].ConvexValue);
                if (convexBound.IsInBound(averageValue))
                {
                    foreach (var nodeIndex in violatedNodesIndices)
                        nodes[nodeIndex].SlackValue = averageValue - nodes[nodeIndex].RealValue;
                    messages  += violatedNodesIndices.Count;
                    bandwidth += violatedNodesIndices.Count;
                    return (server, new Communication(bandwidth, messages));
                }
            }

            return new Communication(bandwidth, messages);
        }

        public override Communication FullSyncAdditionalCost(int numOfNodes, int vectorLength)
            => new Communication(2 * numOfNodes * vectorLength, numOfNodes * 3);
    }
}
