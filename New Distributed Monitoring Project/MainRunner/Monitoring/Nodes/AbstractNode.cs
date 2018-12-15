using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Monitoring.Data;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace Monitoring.Nodes
{
    public abstract class AbstractNode
    {
        public int NodeId { get; }
        public int VectorLength { get; }
        public Vector ReferencePoint { get; private set; }
        public Vector ChangeVector { get; private set; }
        public Vector LocalVector => ReferencePoint.Add(ChangeVector);

        protected AbstractNode(Vector referencePoint, int nodeId, int vectorLength)
        {
            ReferencePoint = referencePoint;
            NodeId = nodeId;
            VectorLength = vectorLength;
            ChangeVector = new Vector(); ;
        }

        public void Change(Vector change)
        {
            ChangeVector.AddInPlace(change);
            ThingsChangedUpdateState();
        }

        protected void ChangeChangeVector(Vector newChangeVector)
        {
            ChangeVector = newChangeVector;
            ThingsChangedUpdateState();
        }

        protected void Reset(Vector referencePoint, Vector changeVector)
        {
            ReferencePoint = referencePoint;
            ChangeVector = changeVector;
            ThingsChangedUpdateState();
        }

        protected abstract void ThingsChangedUpdateState();
    }
}
