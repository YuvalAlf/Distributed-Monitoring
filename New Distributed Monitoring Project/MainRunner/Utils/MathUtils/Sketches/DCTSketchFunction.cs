using System;
using System.Linq;
using System.Runtime.CompilerServices;
using MathNet.Numerics.LinearAlgebra;
using Utils.TypeUtils;

namespace Utils.MathUtils.Sketches
{
    internal sealed class DCTSketchFunction : SketchFunction
    {
        public DCTSketchFunction()
        { }

        private static Vector<double> DCT(Vector<double> @this)
        {
            var N = @this.Count;
            double CreateK(int k) => Enumerable.Range(0, N).Sum(n => @this[n] * Math.Cos(Math.PI * (n + 0.5) * k / N));
            return Enumerable.Range(0, N).Select(CreateK).ToVector();
        }

        private static Vector<double> IDCT(Vector<double> @this)
        {
            var N = @this.Count;

            double CreateK(int k) => 0.5 * @this[0] +
                                     Enumerable.Range(1, N - 1)
                                               .Sum(n => @this[n] * Math.Cos(Math.PI * (k + 0.5) * n / N));

            return Enumerable.Range(0, N).Select(CreateK).Select(x => x * 2 / N).ToVector();
        }

        public override (Vector<double> sketch, Vector<double> epsilon) Sketch(Vector<double> vector, int dimension, StrongBox<int> startIndex)
        {
            var dct = DCT(vector);
            var sketchedDct = Enumerable.Repeat(0.0, vector.Count).ToVector();
            for (int i = 0; i < dimension; i++)
            {
                sketchedDct[startIndex.Value] = dct[startIndex.Value];
                startIndex.Value = (startIndex.Value + 1) % vector.Count;
            }
                
            var sketch  = IDCT(sketchedDct);
            var epsilon = vector - sketch;
            return (sketch, epsilon);
        }
    }
}
