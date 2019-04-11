using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Utils.MathUtils.Sketches
{
    public sealed class StandardBasisSketchFunction : SketchFunction
    {
        public override (Vector sketch, Vector epsilon, InvokedIndices indices) Sketch(Vector vector, int dimension)
        {
            var indices    = new HashSet<int>();
            var sketch     = new Vector();
            var sketchData = vector.IndexedValues.PartialSortBy(dimension / 2, pair => -Math.Abs(pair.Value));
            foreach (var indexValuePair in sketchData)
            {
                if (Math.Abs(indexValuePair.Value) >= 0.000000000001)
                {
                    indices.Add(indexValuePair.Key);
                    sketch[indexValuePair.Key] = indexValuePair.Value;
                }
            }

            var epsilon = vector - sketch;
            return (sketch, epsilon, new InvokedIndices(indices));
        }
    }
}
