using MathNet.Numerics.LinearAlgebra;
using Utils.TypeUtils;

namespace Monitoring.GeometricMonitoring.VectorType
{
    public sealed class SumVectorType : GlobalVectorType
    {
        public override Vector<double> GetValue(Vector<double>[] vectors)
        {
            var length = vectors[0].Count;
            var sum = new double[length];
            foreach (var vector in vectors)
                for (int i = 0; i < length; i++)
                    sum[i] += vector[i];
            return sum.ToVector();
        }

        public override int MulBy(int numOfNodes) => numOfNodes;
    }
}
