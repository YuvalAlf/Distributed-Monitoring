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
        public long Latency      { get; }

        public const int OneWayLatencyMs = 4;
        public const int PayloadDoublesLimit = 283;
        public const int UdpHeaderSize = 40;

        public static (int udpMessages, int udpBandwidth) DataMessage(int vectorSize)
        {
            var numFullPackets = vectorSize / PayloadDoublesLimit;
            var reminder = vectorSize % PayloadDoublesLimit;
            var numMessages = numFullPackets + (reminder > 0 ? 1 : 0);

            return (numMessages, vectorSize + UdpHeaderSize * numMessages);
        }

        public static (int udpMessages, int UdpBandwidth) ControlMessage()
        {
            return (1, UdpHeaderSize);
        }

        public Communication(long bandwidth, long messages, long udpBandwidth, long udpMessages, long latency)
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