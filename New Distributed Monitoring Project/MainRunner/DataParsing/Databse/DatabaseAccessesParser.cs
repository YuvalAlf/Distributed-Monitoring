using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MoreLinq;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace DataParsing.Databse
{
    public sealed partial class DatabaseAccessesParser : IDisposable
    {
        private IEnumerator<(TimedDatabaseAccess, TimedDatabaseAccess)> Accesses { get; }
        private Queue<Vector[]> DataWindow { get; }
        private Func<int, int> HashUser { get; }
        public int DidntChangeIndex { get; }
        public int VectorLength { get; }
        public int NumOfNodes { get; }

        public DatabaseAccessesParser(IEnumerator<(TimedDatabaseAccess, TimedDatabaseAccess)> accesses, Queue<Vector[]> dataWindow, Func<int, int> hashUser, int didntChangeIndex, int vectorLength, int numOfNodes)
        {
            Accesses = accesses;
            DataWindow = dataWindow;
            HashUser = hashUser;
            DidntChangeIndex = didntChangeIndex;
            VectorLength = vectorLength;
            NumOfNodes = numOfNodes;
        }

        public Vector[] CurrentData()
        {
            Vector[] data = Vector.Init(NumOfNodes);
            foreach (var vectorArray in DataWindow)
                for (int i = 0; i < NumOfNodes; i++)
                    data[i].AddInPlace(vectorArray[i]);
            data.ForEach(v => v.DivideInPlace(data.Length));
            return data;
        }

        public static IEnumerable<TimedDatabaseAccess> CsvToAccesses(string csvPath, int maxVectorLength)
        {
            return
                File.ReadLines(csvPath)
                    .Select(TimedDatabaseAccess.TryParse)
                    .Where(a => a.HasValue)
                    .Select(a => a.Value)
                    .Where(a => a.DatabaseAccess.TableId < maxVectorLength);
        }

        public static DatabaseAccessesParser Init(string csvPath, int maxVectorLength, int window, int numOfNodes, Func<int, int> hashUser)
        {
            var vectorLength = CsvToAccesses(csvPath, maxVectorLength).Max(access => access.DatabaseAccess.TableId) + 2;
            var didntChangeIndex = vectorLength - 1;
            var accesses = CsvToAccesses(csvPath, maxVectorLength).Pairs();
            var dataWindow = new Queue<Vector[]>(ArrayUtils.Init(window, _ => NextVectors(hashUser, vectorLength, numOfNodes, didntChangeIndex, accesses, out bool __)));

            return new DatabaseAccessesParser(accesses, dataWindow, hashUser, didntChangeIndex, vectorLength, numOfNodes);
        }

        private static Vector[] NextVectors(Func<int, int>                                hashUser,
                                            int                                           vectorLength,
                                            int                                           numOfNodes,
                                            int                                           didntChangeIndex,
                                            IEnumerator<(TimedDatabaseAccess, TimedDatabaseAccess)> accesses,
                                            out bool                                      didEnd)
        {
            didEnd = false;
            Vector[] newVectors = Vector.Init(numOfNodes);
            bool[]   didChange  = new bool[numOfNodes];
            while (true)
            {
                if (!accesses.MoveNext())
                {
                    didEnd = true;
                    break;
                }

                var (currentAccess, nextAccess) = accesses.Current;
                var node = hashUser(currentAccess.DatabaseAccess.UserId);
                didChange[node]                         =  true;
                newVectors[node][currentAccess.DatabaseAccess.TableId] += 1;

                if (currentAccess.Timestamp != nextAccess.Timestamp)
                    break;
            }

            for (int node = 0; node < numOfNodes; node++)
                if (didChange[node] == false)
                    newVectors[node][didntChangeIndex] = 1.0;

            Debug.Assert(newVectors.All(v => v.IndexedValues.Count > 0));
            return newVectors;
        }


        public Vector[] TakeStep(out bool didEnd)
        {
            var newVectors = NextVectors(HashUser, VectorLength, NumOfNodes, DidntChangeIndex, Accesses, out didEnd);

            DataWindow.Enqueue(newVectors);

            var lastToDequeue = DataWindow.Dequeue();

            return newVectors.Zip(lastToDequeue, (v1, v2) => v1 - v2).ToArray();
        }

        public void Dispose() => Accesses?.Dispose();
    }
}
