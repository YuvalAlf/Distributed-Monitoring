using MathNet.Numerics.LinearAlgebra;

namespace Utils.MathUtils.Sketches
{
    public abstract class SketchFunction
    {
        public abstract (Vector<double> sketch, Vector<double> epsilon) Sketch(Vector<double> vector, int dimension);

        public static SketchFunction DCTSketch => dct;
        private static readonly DCTSketchFunction dct = new DCTSketchFunction();
    }
}
