using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    public abstract class AbstractNode
    {
        public int NodeId { get; }
        public Vector<double> ReferencePoint { get; private set; }
        public Vector<double> ChangeVector { get; private set; }
        public Vector<double> LocalVector => ReferencePoint + ChangeVector;

        protected AbstractNode(Vector<double> referencePoint, int nodeId)
        {
            ReferencePoint = referencePoint;
            NodeId = nodeId;
            ChangeVector = Enumerable.Repeat(0.0, referencePoint.Count).ToVector(); ;
        }

        public void Change(Vector<double> change)
        {
            for (int index = 0; index < ChangeVector.Count; index++)
                ChangeVector[index] += change[index];
            ThingsChangedUpdateState();
        }

        protected void ChangeChangeVector(Vector<double> newChangeVector)
        {
            ChangeVector = newChangeVector;
            ThingsChangedUpdateState();
        }

        protected void Reset(Vector<double> referencePoint, Vector<double> changeVector)
        {
            ReferencePoint = referencePoint;
            ChangeVector = changeVector;
            ThingsChangedUpdateState();
        }

        protected abstract void ThingsChangedUpdateState();
    }
}
