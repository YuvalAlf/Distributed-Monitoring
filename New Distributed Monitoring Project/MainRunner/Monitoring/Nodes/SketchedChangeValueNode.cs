using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.Servers;
using MoreLinq;
using Utils.MathUtils;
using Utils.MathUtils.Sketches;
using Utils.TypeUtils;
using SketchFunction = Utils.MathUtils.Sketches.SketchFunction;

namespace Monitoring.Nodes
{
    public sealed class SketchedChangeValueNode : ValueNode
    {
        public SketchFunction Sketch { get; }

        public SketchedChangeValueNode(Vector<double> referencePoint, ConvexBound convexBound, double slackValue,SketchFunction sketchFunction)
            : base(referencePoint, convexBound, slackValue)
        {
            Sketch = sketchFunction;
        }

        public static Func<Vector<double>, ConvexBound, SketchedChangeValueNode> Create(SketchFunction sketchFunction)
            => (initialVector, convexBound) => new SketchedChangeValueNode(initialVector, convexBound, 0.0, sketchFunction);

        public static Either<(NodeServer<SketchedChangeValueNode>, Communication), Communication> ResolveNodes
            (NodeServer<SketchedChangeValueNode> server, SketchedChangeValueNode[] nodes, Random rnd)
        {
            var messages       = 0;
            var bandwidth      = 0;
            var sketchFunction = nodes[0].Sketch;
            var convexBound    = nodes[0].ConvexBound;

            for (int dimension = 4; dimension <= nodes[0].ReferencePoint.Count / 2; dimension *= 2)
            {
                var valueSchemeResolution = ValueNode.ResolveNodes(server, nodes, rnd);
                if (valueSchemeResolution.IsChoice1)
                {
                    var (newServer, communication) = valueSchemeResolution.GetChoice1;
                    return (newServer, communication.Add(new Communication(bandwidth, messages)));
                }

                messages  += valueSchemeResolution.GetChoice2.Messages;
                bandwidth += valueSchemeResolution.GetChoice2.Bandwidth;
                var currentDimension = dimension;
                var (sketches, epsilons, invokedIndices) =  nodes.Select(n => sketchFunction.Sketch(n.ChangeVector, currentDimension)).UnZip();
                messages  += nodes.Length * 2;
                bandwidth += invokedIndices.Sum(i => i.Dimension);
                var averageChangeSketch = sketches.AverageVector();
                messages  += nodes.Length;
                bandwidth += InvokedIndices.Combine(invokedIndices).Dimension * nodes.Length;
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].Reset(nodes[i].ReferencePoint + averageChangeSketch, epsilons[i]);
                    nodes[i].SlackValue = 0;
                    nodes[i].ThingsChangedUpdateState();
                }
                if (nodes.All(n => convexBound.IsInBound(n.ConvexValue)))
                    return (server, new Communication(bandwidth, messages));
            }

            return new Communication(bandwidth, messages);
        }

        public static Communication FullSyncAdditionalCost(SketchedChangeValueNode[] nodes)
        {
            var sketchFunction = nodes[0].Sketch;
            var vectorLength = nodes[0].LocalVector.Count;
            var numOfNodes = nodes.Length;
            var (sketches, epsilons, invokedIndices) = nodes.Select(n => sketchFunction.Sketch(n.ChangeVector, vectorLength * 2)).UnZip();
            return new Communication(invokedIndices.Sum(i => i.Dimension) + InvokedIndices.Combine(invokedIndices).Dimension * numOfNodes, numOfNodes * 3);
        }
    }
}
