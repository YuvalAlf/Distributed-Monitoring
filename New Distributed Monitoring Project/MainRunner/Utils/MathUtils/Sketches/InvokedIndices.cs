using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.MathUtils.Sketches
{
    public sealed class InvokedIndices
    {
        private HashSet<int> Indices { get; }

        public InvokedIndices(HashSet<int> invokedIndices) => Indices = invokedIndices;

        public void Add(InvokedIndices other) => Indices.UnionWith(other.Indices);

        public int Dimension => Indices.Count;

        public static InvokedIndices Empty() => new InvokedIndices(new HashSet<int>());

        public static InvokedIndices Combine(InvokedIndices[] indices)
        {
            return indices.Aggregate(Empty(), (acc, item) => {acc.Add(item);
                                                  return acc;});
        }
    }
}
