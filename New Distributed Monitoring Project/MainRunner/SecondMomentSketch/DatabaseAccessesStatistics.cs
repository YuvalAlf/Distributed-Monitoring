using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parsing;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace SecondMomentSketch
{
    public sealed class DatabaseAccessesStatistics : IDisposable
    {
        private WindowedStatistics Window { get; }
        private IEnumerator<Vector[]> VectorCountsEnumerator { get; }

        public DatabaseAccessesStatistics(WindowedStatistics window, IEnumerator<Vector[]> vectorCountsEnumerator)
        {
            Window = window;
            VectorCountsEnumerator = vectorCountsEnumerator;
        }

        public static DatabaseAccessesStatistics Init(string databaseAccessesPath, int numOfNodes, int windowSize, Func<int, int, int, int> distributeFunc)
        {
            var vectorCountsEnumerator =
                Parsing.TimedDataAccess.createVectorCountsSequence(distributeFunc, numOfNodes, databaseAccessesPath)
                       .GetEnumerator();
            var window = WindowedStatistics.Init(vectorCountsEnumerator.Take(windowSize));
            return new DatabaseAccessesStatistics(window, vectorCountsEnumerator);
        }

        public Vector[] InitCountVectors() => Window.CurrentNodesCountVectors();

        public bool TakeStep()
        {
            if (!VectorCountsEnumerator.MoveNext())
                return false;
            var next = VectorCountsEnumerator.Current;
            Window.Move(next);
            return true;
        }

        public Vector[] GetChangeCountVectors() => Window.GetChangeCountVectors();

        public void Dispose()
        {
            VectorCountsEnumerator?.Dispose();
        }

        public Vector[] InitProbabilityVectors() => Window.CurrentNodesProbabilityVectors();

        public Vector[] GetChangeProbabilityVectors() => Window.GetChangeProbabilityVectors();
    }
}
