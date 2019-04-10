using System;
using System.Runtime.CompilerServices;
using MathNet.Numerics.LinearAlgebra;

namespace Utils.MathUtils.Sketches
{
    [Serializable]
    public abstract class SketchFunction
    {
        public abstract (Vector<double> sketch, Vector<double> epsilon, InvokedIndices indices) Sketch(Vector<double> vector, int dimension);

        public static SketchFunction DCTSketch => dct;
        private static readonly DCTSketchFunction dct = new DCTSketchFunction();

        public static SketchFunction StandardBaseSketch => standardBase;
        private static readonly StandardBasisSketchFunction standardBase = new StandardBasisSketchFunction();
    }
}
