using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using Utils.TypeUtils;

namespace Utils.DataStructures
{
    public sealed class ChunckedHistogram<T>
    {
        public Vector<double> ZeroVector { get; }
        public Dictionary<T, int> ItemsIndices { get; }
        public DataChunck<T> LastRemovedItem { get; private set; }
        public LinkedList<DataChunck<T>> Queue { get; }
        public SortedSet<T> OptionalItems { get; }
        public int NumOfNodes { get; }
        public int VectorLength => OptionalItems.Count;

        public ChunckedHistogram(DataChunck<T> lastRemovedItem, Dictionary<T, int> itemsIndices, LinkedList<DataChunck<T>> queue, SortedSet<T> optionalItems, Vector<double> zeroVector, int numOfNodes)
        {
            LastRemovedItem = lastRemovedItem;
            ItemsIndices = itemsIndices;
            Queue = queue;
            OptionalItems = optionalItems;
            ZeroVector = zeroVector;
            NumOfNodes = numOfNodes;
        }

        public static ChunckedHistogram<T> Init(SortedSet<T> optionalItems, Dictionary<T, int> itemsIndices, int numOfNodes)
        {
            var queue = new LinkedList<DataChunck<T>>();
            var zeroVector = Enumerable.Repeat(0.0, itemsIndices.Count).ToVector();
            return new ChunckedHistogram<T>(null, itemsIndices, queue, optionalItems, zeroVector, numOfNodes);
        }

        public void AddToEnd(int uid, Dictionary<T, int> items)
        {
            var dataChunck = DataChunck<T>.Init(ZeroVector, NumOfNodes, uid, items, OptionalItems);
            Queue.AddLast(dataChunck);
        }
        public void RemoveLastItem()
        {
            LastRemovedItem = Queue.First.Value;
            Queue.RemoveFirst();
        }

        public void MoveWindow(int uid, Dictionary<T, int> items)
        {
            this.RemoveLastItem();
            this.AddToEnd(uid, items);
        }

        public Vector<double> CountVector()
        {
            var sum = ZeroVector.VConcat(ZeroVector);
            foreach (var dataChunck in Queue)
                sum += dataChunck.CountVector(ItemsIndices);
            return sum;
        }

        public Vector<double> ChangedCountVector()
        {
            return Queue.Last.Value.CountVector(ItemsIndices) - LastRemovedItem.CountVector(ItemsIndices);
        }
    }
}
