using MathNet.Numerics.LinearAlgebra;

namespace Monitoring.GeometricMonitoring.VectorType
{
    public abstract class GlobalVectorType
    {
        public abstract Vector<double> GetValue(Vector<double>[] vectors);

        public abstract int MulBy(int numOfNodes);

        public static GlobalVectorType Average = new AverageVectorType();
        public static GlobalVectorType Sum = new SumVectorType();
    }
}
