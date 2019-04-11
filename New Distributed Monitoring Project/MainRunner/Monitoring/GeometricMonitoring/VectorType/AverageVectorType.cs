using MathNet.Numerics.LinearAlgebra;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.GeometricMonitoring.VectorType
{
    public sealed class AverageVectorType : GlobalVectorType
    {
        public override Vector GetValue(Vector[] vectors) => Vector.AverageVector(vectors);
        public override int MulBy(int numOfNodes) => 1;
    }
}
