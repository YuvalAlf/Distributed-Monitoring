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
using Utils.MathUtils.Sketches;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    [Obsolete("Should use Change-Data-Sketch instead", true)]
    public sealed class SketchedDataValueNode : ValueNode
    {
        public SketchFunction Sketch { get; }

        public SketchedDataValueNode(Vector<double> referencePoint, ConvexBound convexBound, double slackValue,
                                     SketchFunction sketchFunction) :
            base(referencePoint, convexBound, slackValue)
        {
            Sketch = sketchFunction;
        }

        public static Func<Vector<double>, ConvexBound, SketchedDataValueNode> Create(SketchFunction sketchFunction)
            => (initialVector, convexBound) =>
                   new SketchedDataValueNode(initialVector, convexBound, 0.0, sketchFunction);

        public static Either<(NodeServer<SketchedDataValueNode>, Communication), Communication> ResolveNodes
            (NodeServer<SketchedDataValueNode> server, SketchedDataValueNode[] nodes, Random rnd)
        {
            var messages       = 0;
            var bandwidth      = 0;
            var sketchFunction = nodes[0].Sketch;
            var convexBound    = nodes[0].ConvexBound;

            for (int dimension = 2; dimension <= nodes[0].ReferencePoint.Count / 2; dimension *= 2)
            {
                var valueSchemeResolution = ValueNode.ResolveNodes(server, nodes, rnd);
                if (valueSchemeResolution.IsChoice1)
                {
                    var (newServer, communication) = valueSchemeResolution.GetChoice1;
                    return (newServer, communication.Add(new Communication(bandwidth, messages)));
                }

                messages  += valueSchemeResolution.GetChoice2.Messages;
                bandwidth += valueSchemeResolution.GetChoice2.Bandwidth;
                messages  += 2             * nodes.Length;
                bandwidth += dimension * nodes.Length;
                var (sketches, epsilons, invokedIndices) = nodes.Select(n => sketchFunction.Sketch(n.LocalVector, dimension)).UnZip();
                bandwidth += InvokedIndices.Combine(invokedIndices).Dimension * nodes.Length;
                var averageDataSketch = sketches.AverageVector();
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].Reset(averageDataSketch, epsilons[i]);
                    nodes[i].SlackValue = 0;
                    nodes[i].ThingsChangedUpdateState();
                }

                messages  += nodes.Length;
                bandwidth += dimension * nodes.Length;
                if (nodes.All(n => convexBound.IsInBound(n.ConvexValue)))
                    return (server, new Communication(bandwidth, messages));
            }

            return new Communication(bandwidth, messages);
        }

       /* public override Communication FullSyncAdditionalCost(int numOfNodes, int vectorLength)
        {
            var lastDimensionUsed = (vectorLength / 2).ClosestPowerOf2FromBelow();
            var dimensionsLeft    = vectorLength - lastDimensionUsed;
            return new Communication(2 * numOfNodes * dimensionsLeft, numOfNodes * 3);
        }*/
    }
}
