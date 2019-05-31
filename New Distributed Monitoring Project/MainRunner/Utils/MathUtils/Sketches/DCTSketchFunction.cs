using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MathNet.Numerics.LinearAlgebra;
using MoreLinq.Extensions;
using Utils.TypeUtils;

namespace Utils.MathUtils.Sketches
{
    /* internal sealed class DCTSketchFunction : SketchFunction
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

         public override (Vector<double> sketch, Vector<double> approximation, InvokedIndices indices) Sketch(Vector<double> vector, int dimension)
         {
             var indices = new HashSet<int>();
             var dct = DCT(vector);
             var dctSorted = dct.Index().PartialSortBy(dimension, pair => -Math.Abs(pair.Value));
             var sketchedDct = VectorUtils.CreateZeroVector(vector.Count);
             foreach (var indexValuePair in dctSorted)
             {
                 if (Math.Abs(indexValuePair.Value) >= 0.000000000001)
                 {
                     indices.Add(indexValuePair.Key);
                     sketchedDct[indexValuePair.Key] = indexValuePair.Value;
                 }
             }

             var sketch  = IDCT(sketchedDct);
             var approximation = vector - sketch;
             return (sketch, approximation, new InvokedIndices(indices));
         }
     }*/
}
