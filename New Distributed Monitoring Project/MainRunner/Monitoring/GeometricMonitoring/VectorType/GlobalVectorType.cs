using MathNet.Numerics.LinearAlgebra;
using Utils.SparseTypes;

namespace Monitoring.GeometricMonitoring.VectorType
{
    public abstract class GlobalVectorType
    {
        public abstract Vector GetValue(Vector[] vectors);

        public abstract int MulBy(int numOfNodes);

        public static GlobalVectorType Average = new AverageVectorType();
        public static GlobalVectorType Sum = new SumVectorType();
    }
}
