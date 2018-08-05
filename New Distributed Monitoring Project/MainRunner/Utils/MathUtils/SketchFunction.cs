using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Utils.TypeUtils;

namespace Utils.MathUtils
{
    public abstract class SketchFunction
    {
        public abstract (Vector<double> sketch, Vector<double> epsilon) Sketch(Vector<double> vector, int dimension);

        public static SketchFunction DCTSketch => dct;
        private static DCTSketchFunction dct = new DCTSketchFunction();

        private sealed class DCTSketchFunction : SketchFunction
        {
            public DCTSketchFunction()
            {}


            private static Vector<double> DCT(Vector<double> @this)
            {
                var N = @this.Count;
                double CreateK(int k) => Enumerable.Range(0, N).Sum(n => @this[n] * Math.Cos(Math.PI * (n + 0.5) * k / N));
                return Enumerable.Range(0, N).Select(CreateK).ToVector();
            }
            private static Vector<double> IDCT(Vector<double> @this)
            {
                var N = @this.Count;
                double CreateK(int k) => 0.5 * @this[0] + Enumerable.Range(1, N - 1).Sum(n => @this[n] * Math.Cos(Math.PI * (k + 0.5) * n / N));
                return Enumerable.Range(0, N).Select(CreateK).Select(x => x * 2 / N).ToVector();
            }

            public override (Vector<double> sketch, Vector<double> epsilon) Sketch(Vector<double> vector, int dimension)
            {
                var dct = DCT(vector);
                for (int i = dimension; i < vector.Count; i++)
                    dct[i] = 0;
                var sketch = IDCT(dct);
                var epsilon = vector - sketch;
                return (sketch, epsilon);
            }
        }
    }
}
