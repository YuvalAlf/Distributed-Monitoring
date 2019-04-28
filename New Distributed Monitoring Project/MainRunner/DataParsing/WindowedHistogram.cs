using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.SparseTypes;

namespace DataParsing
{
    /*public sealed class WindowedHistogram
    {
        private Queue<Vector[]> WindowVectors { get; }

        public static WindowedHistogram Init(Vector[] vectors)
        {

        }

        public Vector[] TakeStepReturnChange(Vector[] newVectors)
        {
            var lastVectors = WindowVectors.Dequeue();
            WindowVectors.Enqueue(newVectors);
            return newVectors.Zip(lastVectors, (v1, v2) => v1 - v2).ToArray();
        }

        public Vector[] ToStatisticsVectors()
    }*/
}
