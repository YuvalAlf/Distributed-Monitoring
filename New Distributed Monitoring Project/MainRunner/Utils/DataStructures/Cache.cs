using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;

namespace Utils.DataStructures
{
    public sealed class Cache<T>
    {
        private int usageTimeStamp = 0;
        public int MaxSize { get; }
        public Dictionary<int, (int timestamp, T data)> Data { get; }
        public Func<int, T> Generate { get; }

        public Cache(int maxSize, Func<int, T> generate)
        {
            MaxSize = maxSize;
            Generate = generate;
            Data = new Dictionary<int, (int, T)>(MaxSize + 1);
        }

        public T this[int index]
        {
            get
            {
                if (Data.ContainsKey(index))
                    Data[index] = (usageTimeStamp++, Data[index].data);
                else
                {
                    Data[index] = (usageTimeStamp++, Generate(index));
                    CleanCache();
                }

                return Data[index].data;
            }
        }

        private void CleanCache()
        {
            if (Data.Count < MaxSize)
                return;
            var medianTimstamp = Data.Values.Select(pair => (double)pair.timestamp).Median();
            var indices = Data.Keys.ToArray();
            foreach (var index in indices)
                if (Data[index].timestamp < medianTimstamp)
                    Data.Remove(index);
        }
    }
}
