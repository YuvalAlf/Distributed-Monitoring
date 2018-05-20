using MathNet.Numerics.LinearAlgebra;
using Utils.TypeUtils;

namespace Monitoring.GeometricMonitoring.VectorType
{
    public sealed class AverageVectorType : GlobalVectorType
    {
        public override Vector<double> GetValue(Vector<double>[] vectors) => vectors.AverageVector();
        public override int MulBy(int numOfNodes) => 1;
    }
}
