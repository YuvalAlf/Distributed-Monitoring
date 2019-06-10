using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.TypeUtils;

namespace TaxiTripsDataParsing
{
    public sealed class CachedMinWriter<T> : IDisposable
        where T : IComparable<T>
    {
        public SortedSet<T> Items { get; }
        public Action<T> WriteItem { get; }
        public int CacheSize { get; }

        public CachedMinWriter(SortedSet<T> items, Action<T> writeItem, int cacheSize)
        {
            Items = items;
            WriteItem = writeItem;
            CacheSize = cacheSize;
        }

        public static CachedMinWriter<T> Create(int cacheSize, Action<T> writeItem) 
            => new CachedMinWriter<T>(new SortedSet<T>(), writeItem, cacheSize);

        public void AddItem(T newItem)
        {
            Items.Add(newItem);
            if (Items.Count > 0)
                WriteItem(Items.ExtractMin());
        }

        public void Dispose()
        {
            while (Items.Count > 0)
               WriteItem(Items.ExtractMin());
        }
    }
}
