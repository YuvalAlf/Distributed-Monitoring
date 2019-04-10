using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using Utils.TypeUtils;

namespace Utils.DataStructures
{
    public sealed class DataChunck<T>
    {
        public Vector<double> ZeroVector { get; }
        public Dictionary<T, int> Counts { get; }
        public int NumOfNodes { get; }
        public int Uid { get; }
        public bool XVector => Uid % 2 == 0;


        public DataChunck(Dictionary<T, int> counts, int uid, Vector<double> zeroVector, int numOfNodes)
        {
            Counts = counts;
            Uid = uid;
            ZeroVector = zeroVector;
            NumOfNodes = numOfNodes;
        }


        // BEWARE: changing the state of the input..
        public static DataChunck<T> Init(Vector<double> zeroVector, int numOfNodes, int uid, Dictionary<T, int> counts, SortedSet<T> optionalItems)
        {
            foreach (var item in counts.Keys.ToArray())
                if (!optionalItems.Contains(item))
                    counts.Remove(item);
            return new DataChunck<T>(counts, uid, zeroVector, numOfNodes);
        }

        public Vector<double> CountVector(Dictionary<T, int> indexOfItem)
        {
            var counts = new double[indexOfItem.Count];
            Counts.Keys.ForEach(item => counts[indexOfItem[item]] = 1 / (double)NumOfNodes);
            var result = XVector ? counts.Concat(ZeroVector).ToVector() : ZeroVector.Concat(counts).ToVector();
            return result;
        }
    }
}
