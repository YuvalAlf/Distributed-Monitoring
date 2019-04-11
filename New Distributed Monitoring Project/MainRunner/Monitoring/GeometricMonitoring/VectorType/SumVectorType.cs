using MathNet.Numerics.LinearAlgebra;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.GeometricMonitoring.VectorType
{
    public sealed class SumVectorType : GlobalVectorType
    {
        public override Vector GetValue(Vector[] vectors) => Vector.SumVector(vectors);

        public override int MulBy(int numOfNodes) => numOfNodes;
    }
}
