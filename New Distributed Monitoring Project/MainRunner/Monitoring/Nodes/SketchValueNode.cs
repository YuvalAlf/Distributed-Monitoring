using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.Servers;
using MoreLinq;
using Utils.MathUtils;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    public sealed class SketchValueNode : ValueNode
    {
        public SketchFunction Sketch = SketchFunction.DCTSketch;

        public SketchValueNode(Vector<double> referencePoint, ConvexBound convexBound, double slackValue) :
            base(referencePoint, convexBound, slackValue)
        {}

        public new static SketchValueNode Create(Vector<double> initialVector, ConvexBound convexBound)
            => new SketchValueNode(initialVector, convexBound, 0.0);

        public static Either<(NodeServer<SketchValueNode>, CommunicationPrice), CommunicationPrice> ResolveNodes
            (NodeServer<SketchValueNode> server, SketchValueNode[] nodes, Random rnd)
        {
            var messages       = 0;
            var bandwidth      = 0;
            var sketchFunction = nodes[0].Sketch;
            var convexBound    = nodes[0].ConvexBound;

            for (int dimension = 2; dimension <= nodes[0].ReferencePoint.Count / 3; dimension *= 2)
            {
                var valueSchemeResolution = ValueNode.ResolveNodes(server, nodes, rnd);
                if (valueSchemeResolution.IsChoice1)
                {
                    var (newServer, communication) = valueSchemeResolution.GetChoice1;
                    return (newServer, communication.Add(new CommunicationPrice(bandwidth, messages)));
                }

                messages                 += valueSchemeResolution.GetChoice2.Messages;
                bandwidth                += valueSchemeResolution.GetChoice2.Bandwidth;
                messages                 += 2             * nodes.Length;
                bandwidth                += 2 * dimension * nodes.Length;
                var (sketches, epsilons) =  nodes.Select(n => sketchFunction.Sketch(n.LocalVector, dimension)).UnZip();
                var averageSketch = sketches.AverageVector();
                for (int i = 0; i < nodes.Length; i++)
                    nodes[i].Reset(averageSketch.Clone(), epsilons[i].Clone());
                messages  += nodes.Length;
                bandwidth += dimension * nodes.Length;
                if (nodes.All(n => convexBound.IsInBound(n.ConvexValue)))
                    return (server, new CommunicationPrice(bandwidth, messages));
            }

            return new CommunicationPrice(bandwidth, messages);
        }

    }
}
