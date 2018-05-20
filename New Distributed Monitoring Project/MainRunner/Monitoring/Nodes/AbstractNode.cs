using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    public abstract class AbstractNode
    {
        public Vector<double> ReferencePoint { get; protected set; }
        public Vector<double> ChangeVector { get; protected set; }
        public Vector<double> LocalVector => ReferencePoint + ChangeVector;

        protected AbstractNode(Vector<double> referencePoint)
        {
            ReferencePoint = referencePoint;
            ChangeVector = Enumerable.Repeat(0.0, referencePoint.Count).ToVector(); ;
        }

        public void Change(Vector<double> change)
        {
            for (int index = 0; index < ChangeVector.Count; index++)
                ChangeVector[index] += change[index];
            ThingsChangedUpdateState();
        }

        protected abstract void ThingsChangedUpdateState();
    }
}
