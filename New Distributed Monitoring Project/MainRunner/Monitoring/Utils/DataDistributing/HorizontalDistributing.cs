using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.TypeUtils;

namespace Monitoring.Utils.DataDistributing
{
    public sealed class HorizontalDistributing : GeographicalDistributing
    {
        private double ChunckSize { get; }

        public override string Name => "Horizontal";

        public HorizontalDistributing(int minValue, int maxValue, int numOfNodes) : base(minValue, maxValue, numOfNodes)
        {
            ChunckSize = (maxValue - minValue + 1) / (double)numOfNodes;
        }
        
        public override int NodeOf(int value)
        {
            var node = (value - MinValue) / ChunckSize;
            return ((int)node).ToRange(0, NumOfNodes - 1);
        }
    }
}
