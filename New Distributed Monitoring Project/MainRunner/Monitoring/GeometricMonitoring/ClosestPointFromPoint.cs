using Utils.AiderTypes;
using Utils.SparseTypes;

namespace Monitoring.GeometricMonitoring
{
    public delegate Either<Vector, double> ClosestPointFromPoint(Vector givenPoint, int node);
}