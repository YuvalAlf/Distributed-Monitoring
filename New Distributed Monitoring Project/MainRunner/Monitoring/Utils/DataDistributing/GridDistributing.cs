using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using Utils.TypeUtils;

namespace Monitoring.Utils.DataDistributing
{
    public sealed class GridDistributing : GeographicalDistributing
    {
        public double ChunckSize { get; }
        public int SqrtNumOfNodes { get; }
        public override string Name => "Grid";

        public GridDistributing(int minValue, int maxValue, int numOfNodes) : base(minValue, maxValue, numOfNodes)
        {
            SqrtNumOfNodes = Convert.ToInt32(Math.Sqrt(NumOfNodes));
            Debug.Assert(SqrtNumOfNodes * SqrtNumOfNodes == NumOfNodes, "Num of nodes has to be a squarred number");
            ChunckSize = (maxValue - minValue + 1) / (double)SqrtNumOfNodes;
        }

        public override int NodeOf(int value)
        {
            value -= MinValue;
            var row = (int) (value / ChunckSize);
            var col = (int)((value / SqrtNumOfNodes) % SqrtNumOfNodes);
            var node = (int) (row * SqrtNumOfNodes + col);
            return ((int)node).ToRange(0, NumOfNodes - 1);
        }
    }
}
