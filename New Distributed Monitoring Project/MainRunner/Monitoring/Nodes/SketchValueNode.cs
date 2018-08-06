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
using Utils.TypeUtils;
using SketchFunction = Utils.MathUtils.Sketches.SketchFunction;

namespace Monitoring.Nodes
{
    public sealed class SketchValueNode : ValueNode
    {
        public SketchFunction Sketch { get; }

        public SketchValueNode(Vector<double> referencePoint, ConvexBound convexBound, double slackValue,
                               SketchFunction sketchFunction) :
            base(referencePoint, convexBound, slackValue)
        {
            Sketch = sketchFunction;
        }

        public new static Func<Vector<double>, ConvexBound, SketchValueNode> Create(SketchFunction sketchFunction)
            => (initialVector, convexBound) => new SketchValueNode(initialVector, convexBound, 0.0, sketchFunction);


        private StrongBox<int> SketchIndex { get; } = new StrongBox<int>(0);

        public static Either<(NodeServer<SketchValueNode>, Communication), Communication> ResolveNodes
            (NodeServer<SketchValueNode> server, SketchValueNode[] nodes, Random rnd)
        {
            var messages       = 0;
            var bandwidth      = 0;
            var sketchFunction = nodes[0].Sketch;
            var convexBound    = nodes[0].ConvexBound;

            for (int dimension = 2; dimension <= nodes[0].ReferencePoint.Count / 4; dimension *= 2)
            {
                var valueSchemeResolution = ValueNode.ResolveNodes(server, nodes, rnd);
                if (valueSchemeResolution.IsChoice1)
                {
                    var (newServer, communication) = valueSchemeResolution.GetChoice1;
                    return (newServer, communication.Add(new Communication(bandwidth, messages)));
                }

                messages                 += valueSchemeResolution.GetChoice2.Messages;
                bandwidth                += valueSchemeResolution.GetChoice2.Bandwidth;
                messages                 += 2             * nodes.Length;
                bandwidth                += 2 * dimension * nodes.Length;
                var (sketches, epsilons) =  nodes.Select(n => sketchFunction.Sketch(n.ChangeVector, dimension, n.SketchIndex)).UnZip();
                var averageChangeSketch = sketches.AverageVector();
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].Reset(nodes[i].ReferencePoint + averageChangeSketch, epsilons[i]);
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

        public override Communication FullSyncAdditionalCost(int numOfNodes, int vectorLength)
        {
            var lastDimensionUsed = (vectorLength / 2).ClosestPowerOf2FromBelow();
            var dimensionsLeft = vectorLength - lastDimensionUsed;
            return new Communication(2 * numOfNodes * dimensionsLeft, numOfNodes * 3);
        }
    }
}
