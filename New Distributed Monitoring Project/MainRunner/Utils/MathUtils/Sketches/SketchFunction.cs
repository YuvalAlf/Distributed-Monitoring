using System;
using System.Runtime.CompilerServices;
using MathNet.Numerics.LinearAlgebra;
using Utils.SparseTypes;

namespace Utils.MathUtils.Sketches
{
    [Serializable]
    public abstract class SketchFunction
    {
        public abstract (Vector sketch, Vector epsilon, InvokedIndices indices) Sketch(Vector vector, int dimension);

      //  public static SketchFunction DCTSketch => dct;
      //  private static readonly DCTSketchFunction dct = new DCTSketchFunction();

        public static SketchFunction StandardBaseSketch => standardBase;
        private static readonly StandardBasisSketchFunction standardBase = new StandardBasisSketchFunction();
    }
}
