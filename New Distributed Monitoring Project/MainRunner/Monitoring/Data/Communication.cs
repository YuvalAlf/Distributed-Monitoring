using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Data
{
    public sealed class Communication
    {
        public long Bandwidth    { get; }
        public long Messages     { get; }
        public long UdpBandwidth { get; }
        public long UdpMessages  { get; }
        public double Latency      { get; }

        public const double ConstantOneWayBandwidthMs = 4;

        // WIFI
        public const int PayloadDoublesLimit = 283;
        public const int UdpHeaderSize       = 40;
        public const double NetworkBandwidthBytePerMs    = 7077.888;

        // Zigbee
        //public const int PayloadDoublesLimit = 11;
        //public const int UdpHeaderSize       = 36;
        //public const double NetworkBandwidthBytePerMs    = 32;


        public static (int udpMessages, int udpBandwidth, double latency) DataMessageVectorSize(int vectorSize)
        {
            var numFullPackets = vectorSize / PayloadDoublesLimit;
            var reminder = vectorSize % PayloadDoublesLimit;
            var numMessages = numFullPackets + (reminder > 0 ? 1 : 0);
            var totalBandwidth = vectorSize * 8 + UdpHeaderSize * numMessages;
            var latency = ConstantOneWayBandwidthMs + totalBandwidth / NetworkBandwidthBytePerMs;
            return (numMessages, totalBandwidth, latency);
        }

        public static (int udpMessages, int UdpBandwidth, double latency) ControlMessage(int additionalBytes)
        {
            var totalBandwidth = UdpHeaderSize + additionalBytes;
            var latency = ConstantOneWayBandwidthMs + totalBandwidth / NetworkBandwidthBytePerMs;
            return (1, totalBandwidth, latency);
        }

        public Communication(long bandwidth, long messages, long udpBandwidth, long udpMessages, double latency)
        {
            Bandwidth    = bandwidth;
            Messages     = messages;
            UdpBandwidth = udpBandwidth;
            UdpMessages  = udpMessages;
            Latency      = latency;
        }

        public static Communication Zero => new Communication(0, 0, 0, 0, 0);

        public Communication Add(Communication priceToAdd, bool setLatency)
        {
            var newLatency = setLatency ? priceToAdd.Latency : Latency + priceToAdd.Latency;
            return new Communication(Bandwidth   + priceToAdd.Bandwidth, Messages  + priceToAdd.Messages, UdpBandwidth + priceToAdd.UdpBandwidth,
                              UdpMessages + priceToAdd.UdpMessages, newLatency);
        }
    }
}