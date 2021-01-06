using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Monitoring.Data;
using Monitoring.GeometricMonitoring;
using Monitoring.Servers;
using MoreLinq;
using Utils.AiderTypes;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    public class FunctionNode : AbstractNode
    {
        public ConvexBound ConvexBound { get; }
        public ConvexBound.Type BoundType => ConvexBound.ConvexBoundType;
        public double Threshold => ConvexBound.Threshold;
        public double InitialConvexValue { get; protected set; }
        public double SlackValue => RealValue - InitialConvexValue;
        public double RealValue { get; protected set; }
        public int LocalCounter { get; protected set; } = 0;


        public FunctionNode(Vector referencePoint, ConvexBound convexBound, int nodeId, int vectorLength)
            : base(referencePoint, nodeId, vectorLength)
        {
            ConvexBound = convexBound;
            ThingsChangedUpdateState();
            InitialConvexValue = RealValue;
        }

        public static FunctionNode Create(Vector initialVector, ConvexBound convexBound, int nodeId, int vectorLength)
            => new FunctionNode(initialVector, convexBound, nodeId, vectorLength);

        protected override void ThingsChangedUpdateState()
        {
            RealValue = ConvexBound.Compute(LocalVector);
        }

        public int QuantsOfSlackValue(double quant)
        {
            var quants =
                BoundType == ConvexBound.Type.UpperBound
                    ? Math.Floor(SlackValue  / quant)
                    : Math.Ceiling(-SlackValue / quant);
            return Convert.ToInt32(Math.Max(0, quants));
        }

        public static Either<(NodeServer<TFunctionNode>, Communication), Communication> ResolveNodes<TFunctionNode>
            (NodeServer<TFunctionNode> server, TFunctionNode[] nodes, Random rnd)
        where TFunctionNode : FunctionNode
        {
            var bandwidth = new StrongBox<int>(0);
            var messages = new StrongBox<int>(0);
            var udpMessagesAndBandwidth = new StrongBox<(int, int, double)>((0, 0, 0.0));

            var threshold = nodes[0].ConvexBound.Threshold;
            var convexType = nodes[0].ConvexBound.ConvexBoundType;
            double quant = Math.Abs((threshold - nodes[0].InitialConvexValue) / 2);
            int[] a; 

            nodes.Where(n => n.QuantsOfSlackValue(quant) > n.LocalCounter)
                     .SideEffect(_ => bandwidth.Value += 1)
                     .SideEffect(_ => messages.Value += 1)
                     .SideEffect(_ => udpMessagesAndBandwidth.Value = TupleUtils.PointwiseAddKeepLast(udpMessagesAndBandwidth.Value, Communication.ControlMessage(4)))
                     .ForEach(n => n.LocalCounter = n.QuantsOfSlackValue(quant));

            var counterSum = nodes.Sum(n => n.LocalCounter);

            if (counterSum > nodes.Length)
            {
                var averageSlack = nodes.Average(n => n.SlackValue);
                var initialConvex = nodes[0].InitialConvexValue;
                messages.Value  += 2 * nodes.Length; //2 * nodes.Count(n => n.QuantsOfSlackValue(quant) <= n.LocalCounter);
                bandwidth.Value += 2 * nodes.Length; //2 * nodes.Count(n => n.QuantsOfSlackValue(quant) <= n.LocalCounter);

                var additive = TupleUtils.PointwiseAdd(Communication.ControlMessage(0), Communication.ControlMessage(4)); // quantum
                additive = TupleUtils.PointwiseMulKeepLast(nodes.Length, additive);
                udpMessagesAndBandwidth.Value = TupleUtils.PointwiseAdd(udpMessagesAndBandwidth.Value, additive);

                if ((convexType == ConvexBound.Type.UpperBound && initialConvex + averageSlack > threshold) ||
                    (convexType == ConvexBound.Type.LoweBound && initialConvex + averageSlack < threshold))
                    return new Communication(bandwidth.Value, messages.Value, udpMessagesAndBandwidth.Value.Item2, udpMessagesAndBandwidth.Value.Item1, udpMessagesAndBandwidth.Value.Item3);
                messages.Value  += nodes.Length;
                bandwidth.Value += nodes.Length;
                var newAdditive = TupleUtils.PointwiseMulKeepLast(nodes.Length, Communication.ControlMessage(8));  // average of phi(v_i)
                udpMessagesAndBandwidth.Value = TupleUtils.PointwiseAdd(udpMessagesAndBandwidth.Value, newAdditive);
                nodes
                   .SideEffect(n => n.LocalCounter = 0)
                   .SideEffect(n => n.InitialConvexValue += averageSlack)
                   .ForEach(n => n.RealValue = n.InitialConvexValue);

            }

            return (server, new Communication(bandwidth.Value, messages.Value, udpMessagesAndBandwidth.Value.Item2, udpMessagesAndBandwidth.Value.Item1, udpMessagesAndBandwidth.Value.Item3));
        }

        public static Communication FullSyncAdditionalCost(FunctionNode[] nodes)
        {
            var bandwidth = nodes.Sum(n => n.VectorLength) * 2;
            var messages  = nodes.Length                   * 3;
            var (dataUdpMessages, dataUdpBandwidth, dataLatency) = nodes.Select(n => Communication.DataMessageVectorSize(n.VectorLength))
                                                           .Aggregate(TupleUtils.PointwiseAddKeepLast);

            var (controlUdpMessages, controlUdpBandwidth, controlLatency) = nodes.Select(n => Communication.ControlMessage(0))
                                                                 .Aggregate(TupleUtils.PointwiseAddKeepLast);
            //  => new Communication(nodes.Sum(n => n.ChangeVector.CountNonZero()) + nodes.Length * nodes.Map(n => n.ChangeVector).AverageVector().CountNonZero(), nodes.Length * 3);
            return new Communication(bandwidth, messages, dataUdpBandwidth * 2 + controlUdpBandwidth, dataUdpMessages * 2 + controlUdpMessages, dataLatency * 2 + controlLatency);
        }
    }
}
