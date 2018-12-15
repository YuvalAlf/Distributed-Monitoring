using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.Servers;
using Utils.MathUtils.Sketches;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    public sealed class SketchedChangeDistanceNode : DistanceNode
    {
        public SketchFunction Sketch { get; }
        public SketchedChangeDistanceNode(Vector referencePoint, ConvexBound convexBound, double slackValue, SketchFunction sketchFunction, int norm, int nodeId, int vectorLength)
            : base(referencePoint, convexBound, slackValue, norm, nodeId, vectorLength)
        {
            Sketch = sketchFunction;
        }

        public static Func<Vector, ConvexBound, int, int, SketchedChangeDistanceNode> Create(SketchFunction sketchFunction, int norm)
            => (initialVector, convexBound, nodeId, vectorLength) => new SketchedChangeDistanceNode(initialVector, convexBound, 0.0, sketchFunction, norm, nodeId, vectorLength);

        public static Either<(NodeServer<SketchedChangeDistanceNode>, Communication), Communication> ResolveNodes
            (NodeServer<SketchedChangeDistanceNode> server, SketchedChangeDistanceNode[] nodes, Random rnd)
        {
            var messages = 0;
            var bandwidth = 0;
            var sketchFunction = nodes[0].Sketch;
            var convexBound = nodes[0].ConvexBound;

            for (int dimension = 4; dimension <= nodes[0].VectorLength / 4; dimension *= 2)
            {
                var distanceSchemeResolution = DistanceNode.ResolveNodes(server, nodes, rnd);
                if (distanceSchemeResolution.IsChoice1)
                {
                    var (newServer, communication) = distanceSchemeResolution.GetChoice1;
                    return (newServer, communication.Add(new Communication(bandwidth, messages)));
                }

                messages += distanceSchemeResolution.GetChoice2.Messages;
                bandwidth += distanceSchemeResolution.GetChoice2.Bandwidth;
                var currentDimension = dimension;
                var (sketches, epsilons, invokedIndices) = nodes.Select(n => sketchFunction.Sketch(n.ChangeVector, currentDimension)).UnZip();
                messages += nodes.Length * 2;
                bandwidth += invokedIndices.Sum(i => i.Dimension) * 2;
                var averageChangeSketch = Vector.AverageVector(sketches);
                messages += nodes.Length;
                bandwidth += InvokedIndices.Combine(invokedIndices).Dimension * 2 * nodes.Length;
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].Reset(nodes[i].ReferencePoint.Add(averageChangeSketch), epsilons[i]);
                    nodes[i].SlackDistance = 0;
                    nodes[i].ThingsChangedUpdateState();
                }
                if (nodes.All(n => n.UsedDistance <= 0))
                    return (server, new Communication(bandwidth, messages));
            }

            return new Communication(bandwidth, messages);
        }

        public static Communication FullSyncAdditionalCost(SketchedChangeDistanceNode[] nodes)
        {
            var sketchFunction = nodes[0].Sketch;
            var vectorLength   = nodes[0].VectorLength;
            var numOfNodes     = nodes.Length;
            var (sketches, epsilons, invokedIndices) = nodes.Select(n => sketchFunction.Sketch(n.ChangeVector, vectorLength * 2)).UnZip();
            return new Communication(2 * (invokedIndices.Sum(i => i.Dimension) + InvokedIndices.Combine(invokedIndices).Dimension * numOfNodes), numOfNodes * 3);
        }
    }
}
