﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using Utils.TypeUtils;

namespace Utils.DataStructures
{
    public sealed class WindowedHistogram<T>
    {
        public HistogramQueue<T> ItemsRemoved { get; }
        public HistogramQueue<T> ItemsInserted { get; }
        public HistogramQueue<T> ItemsInside { get; }
        public SortedSet<T> OptionalItems { get; }

        public WindowedHistogram(HistogramQueue<T> itemsRemoved, HistogramQueue<T> itemsInserted, HistogramQueue<T> itemsInside, SortedSet<T> optionalItems)
        {
            ItemsRemoved = itemsRemoved;
            ItemsInserted = itemsInserted;
            ItemsInside = itemsInside;
            OptionalItems = optionalItems;
        }

        public static WindowedHistogram<T> Init(IEnumerable<T> firstItems, SortedSet<T> optionalItems)
        {
            var itemsRemoved = HistogramQueue<T>.Create(optionalItems);
            var itemsInserted = HistogramQueue<T>.Create(optionalItems);
            var itemsInside = HistogramQueue<T>.Create(optionalItems);
            itemsInside.EnqueueAll(firstItems);

            return new WindowedHistogram<T>(itemsRemoved, itemsInserted, itemsInside, optionalItems);
        }
        public int MoveWindow(IEnumerable<T> nextItems)
        {
            ItemsRemoved.DequeueAll();
            ItemsInside.EnqueueAll(ItemsInserted.DequeueAll());

            StrongBox<int> amount = new StrongBox<int>(0);
            nextItems.ForEach(newItem =>
            {
                ItemsInserted.Enqueue(newItem);
                ItemsRemoved.Enqueue(ItemsInside.Dequeue());
                amount.Value++;
            });
            return amount.Value;
        }

        public Vector<double> CountVector() 
            => ItemsInside.CountVector() + ItemsInserted.CountVector();

        public Vector<double> ChangedCountVector()
        {
            var vec = ItemsInserted.CountVector() - ItemsRemoved.CountVector();
            Debug.Assert(vec.Sum() == 0);
            return vec;
        }
    }
}
