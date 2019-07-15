using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monitoring.Data
{
    public sealed class Communication
    {
        public int Bandwidth { get; }
        public int Messages { get; }

        public Communication(int bandwidth, int messages)
        {
            Bandwidth = bandwidth;
            Messages = messages;
        }

        public static Communication Zero => new Communication(0, 0);

        public Communication Add(Communication priceToAdd)
            => new Communication(Bandwidth + priceToAdd.Bandwidth, Messages + priceToAdd.Messages);
    }
}
