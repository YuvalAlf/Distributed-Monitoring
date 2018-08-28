using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq;
using Utils.TypeUtils;

namespace Utils.MathUtils.Sketches
{
    public sealed class MostDominantElementSketchFunction : SketchFunction
    {
        public override (Vector<double> sketch, Vector<double> epsilon, InvokedIndices indices) Sketch(
            Vector<double> vector, int dimension, StrongBox<int> startIndex)
        {
            var indices = new HashSet<int>();
            var sketch  = Enumerable.Repeat(0.0, vector.Count).ToVector();
            var sketchData  = vector.Index().PartialSortBy(dimension / 2, pair => -Math.Abs(pair.Value));
            foreach (var indexValuePair in sketchData)
            {
                indices.Add(indexValuePair.Key);
                sketch[indexValuePair.Key] = indexValuePair.Value;
            }

            var epsilon = vector - sketch;
            return (sketch, epsilon, new InvokedIndices(indices));
        }
    }
}
