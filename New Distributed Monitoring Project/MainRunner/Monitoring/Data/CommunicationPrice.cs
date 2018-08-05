using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Data
{
    public sealed class CommunicationPrice
    {
        public int Bandwidth { get; }
        public int Messages { get; }

        public CommunicationPrice(int bandwidth, int messages)
        {
            Bandwidth = bandwidth;
            Messages = messages;
        }

        public static CommunicationPrice Zero => new CommunicationPrice(0, 0);

        public CommunicationPrice Add(CommunicationPrice priceToAdd)
            => new CommunicationPrice(Bandwidth + priceToAdd.Bandwidth, Messages + priceToAdd.Messages);
    }
}
