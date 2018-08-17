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

        public static Vector<double> DCT(Vector<double> @this)
        {
            var dct = @this.ToArray();
            Accord.Math.CosineTransform.DCT(dct);
            return dct.ToVector();
        }
        public static Vector<double> IDCT(Vector<double> @this)
        {
            var idct = @this.ToArray();
            Accord.Math.CosineTransform.IDCT(idct);
            return idct.ToVector();
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
