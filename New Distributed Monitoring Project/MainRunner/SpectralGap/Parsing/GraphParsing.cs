using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectralGap.Parsing
{
    public static class GraphParsing
    {
        public static IEnumerable<GraphOperation> TransformGraph(string inputFilePath)
        {
            var timestampToEdges = new Dictionary<int, HashSet<(int, int)>>();
            foreach (var line in File.ReadLines(inputFilePath))
            {
                var tokens = line.Split(' ');
                if (tokens[0].StartsWith("%"))
                    if (Int32.TryParse(tokens.Last(), out int numOfNodes))
                        yield return new GraphOperation.InitGraph(numOfNodes);
                    else
                        continue;
                else
                {
                    var node1 = int.Parse(tokens[0]);
                    var node2 = int.Parse(tokens[1]);
                    var timestamp = int.Parse(tokens[3]);
                    if (!timestampToEdges.ContainsKey(timestamp))
                        timestampToEdges.Add(timestamp, new HashSet<(int, int)>());
                    if (timestampToEdges.Values.Any(s => s.Contains((node1, node2))))
                        throw new ArgumentException();
                    timestampToEdges[timestamp].Add((node1, node2));
                }
            }

            foreach (var timestamp in timestampToEdges.Keys.OrderBy(x => x))
            {
                yield return new GraphOperation.NewTimestampOperation();
                foreach (var (node1, node2) in timestampToEdges[timestamp])
                    yield return new GraphOperation.EdgeOperation.AddEdge(node1, node2);
            }
        }

        public static void WriteGraph(string outputFilePath, IEnumerable<GraphOperation> operations)
        {
            using (var file = File.CreateText(outputFilePath))
                foreach (var graphOperation in operations)
                    file.WriteLine(graphOperation.ToText());
        }

        public static IEnumerable<GraphOperation> ReadGraph(string inputFilePath)
        {
            return File.ReadLines(inputFilePath).Select(GraphOperation.Parse);
        }
    }
}
