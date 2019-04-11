using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using Utils.TypeUtils;

namespace SpectralGap.Parsing
{
    public sealed class GraphCreationState
    {
        private int NumOfNodes { get; }
        private Func<(int, int), int> EdgeToNode { get; }
        private IEnumerator<GraphOperation> Operations { get; }

        public GraphCreationState(IEnumerable<GraphOperation> operations, Func<(int, int), int> edgeToNode, int numOfNodes)
        {
            Operations = operations.GetEnumerator();
            EdgeToNode = edgeToNode;
            NumOfNodes = numOfNodes;
        }

        public (int vectorLength, Vector<double>[] initVectors) InitMatrix()
        {
            throw new NotImplementedException();
        }

        public Vector<double>[] GetNextChange(int vectorLength, out bool ended)
        {
            ended = false;
            var changeVectors = ArrayUtils.Init(NumOfNodes, _ => VectorUtils.CreateZeroVector(vectorLength));
            var nextOperation = Operations.Current;
            while (!ended && !(Operations.Current is GraphOperation.NewTimestampOperation))
            {
                var edgeOperation = (Operations.Current as GraphOperation.EdgeOperation.AddEdge);
                var node1 = edgeOperation.Node1;
                var node2 = edgeOperation.Node2;
                var nodeIndex = EdgeToNode((node1, node2));
                changeVectors[nodeIndex][GetMatrixIndex(vectorLength, node1, node2)] = 1.0;
                if (Operations.MoveNext() == false)
                    ended = true;
            }
            throw new NotImplementedException();
            return changeVectors;
        }

        private int GetMatrixIndex(int vectorLength, int node1, int node2)
        {
            if (node1 == node2)
                throw new ArgumentException("Node1 = Node2");
            if (node1 > node2)
                return GetMatrixIndex(vectorLength, node2, node1);
            var index = (node1 - 1) * (vectorLength - 1) + (node2 - 1);
            return index;
        }
    }
}
