using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using Utils.TypeUtils;

namespace Utils.DataStructures
{
    public sealed class HistogramQueue<T>
    {
        public SortedSet<T> OptionalItems { get; }
        public SortedDictionary<T, int> Counter { get; }
        public Queue<T> Queue { get; }

        private HistogramQueue(SortedSet<T> optionalItems, SortedDictionary<T, int> counter, Queue<T> queue)
        {
            OptionalItems = optionalItems;
            Counter = counter;
            Queue = queue;
        }

        public static HistogramQueue<T> Create(SortedSet<T> optionalItems)
        {
            var dictionary = new SortedDictionary<T, int>(optionalItems.Comparer);
            optionalItems.ForEach(i => dictionary[i] = 0);
            return new HistogramQueue<T>(optionalItems, dictionary, new Queue<T>());
        }

        public void Enqueue(T item)
        {
            Debug.Assert(OptionalItems.Contains(item));
            Queue.Enqueue(item);
            Counter[item]++;
        }
        public T Dequeue()
        {
            var item = Queue.Dequeue();
            Counter[item]--;
            return item;
        }

        public IEnumerable<T> DequeueAll()
        {
            var items = this.Queue.ToArray();
            while (this.Queue.Count > 0)
                this.Dequeue();
            return items;
        }

        public void EnqueueAll(IEnumerable<T> items) => items.ForEach(this.Enqueue);

        public Vector<double> CountVector()
            => OptionalItems.Select(i => (double)Counter[i]).ToVector();
    }
}
