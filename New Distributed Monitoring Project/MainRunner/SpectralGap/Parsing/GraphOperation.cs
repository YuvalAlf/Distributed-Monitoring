using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralGap.Parsing
{
    public abstract class GraphOperation
    {
        public abstract string TokenName { get; }

        public abstract string ToText();

        public static GraphOperation Parse(string line)
        {
            var data  = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var token = data[0];
            Lazy<int> int1 = new Lazy<int>(() => int.Parse(data[1]));
            Lazy<int> int2 = new Lazy<int>(() => int.Parse(data[2]));

            switch (token)
            {
                case NewTimestampOperation.TOKEN_NAME:
                    return new NewTimestampOperation();

                case InitGraph.TOKEN_NAME:
                    return new InitGraph(int1.Value);

                case EdgeOperation.AddEdge.TOKEN_NAME:
                    return new EdgeOperation.AddEdge(int1.Value, int2.Value);

                case EdgeOperation.RemoveEdge.TOKEN_NAME:
                    return new EdgeOperation.RemoveEdge(int1.Value, int2.Value);

                default:
                    throw new ArgumentException(line);
            }
        }

        public sealed class NewTimestampOperation : GraphOperation
        {
            public const string TOKEN_NAME = "NEW_TIMESTAMP";
            public override string TokenName => TOKEN_NAME;
            public override string ToText() => TOKEN_NAME;
        }

        public sealed class InitGraph : GraphOperation
        {
            public const    string TOKEN_NAME = "INIT_GRAPH";
            public override string TokenName => TOKEN_NAME;
            public override string ToText()  => $"{TOKEN_NAME} {NumOfVertices}";

            public int NumOfVertices { get; }

            public InitGraph(int numOfVertices)
            {
                NumOfVertices = numOfVertices;
            }
        }

        public abstract class EdgeOperation : GraphOperation
        {
            public int Node1 { get; }
            public int Node2 { get; }

            protected EdgeOperation(int node1, int node2)
            {
                Node1 = node1;
                Node2 = node2;
            }
            
            public override string ToText() => $"{TokenName} {Node1} {Node2}";

            public sealed class AddEdge : EdgeOperation
            {
                public const    string TOKEN_NAME = "ADD";
                public override string TokenName => TOKEN_NAME;

                public AddEdge(int node1, int node2) : base(node1, node2)
                {
                }
            }

            public sealed class RemoveEdge : EdgeOperation
            {
                public const    string TOKEN_NAME = "REMOVE";
                public override string TokenName => TOKEN_NAME;

                public RemoveEdge(int node1, int node2) : base(node1, node2)
                {
                }
            }
        }

    }
}
