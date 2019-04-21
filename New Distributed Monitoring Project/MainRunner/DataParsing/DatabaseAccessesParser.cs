using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;
using MoreLinq;
using Utils.SparseTypes;
using Utils.TypeUtils;

namespace DataParsing
{
    public sealed class DatabaseAccessesParser : IDisposable
    {
        public struct DatabaseAccess
        {
            public int UserId { get; }
            public int TableId { get; }
            public long Timestamp { get; }

            public DatabaseAccess(int userId, int tableId, long timestamp)
            {
                UserId = userId;
                TableId = tableId;
                Timestamp = timestamp;
            }

            public static DatabaseAccess? TryParse(string line)
            {
                var tokens = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length != 3)
                    return null;
                var res1 = int.TryParse(tokens[0], out int userId);
                var res2 = int.TryParse(tokens[1], out int tableId);
                var res3 = long.TryParse(tokens[2], out long timestamp);
                if (!(res1 && res2 && res3))
                    return null;
                return new DatabaseAccess(userId, tableId, timestamp);
            }
        }


        public string CsvPath { get; }
        private IEnumerator<(DatabaseAccess, DatabaseAccess)> Accesses { get; }
        public int VectorLength { get; }
        public long CurrentTimestamp { get; } = -1;
        
        public DatabaseAccessesParser(string csvPath, IEnumerator<(DatabaseAccess, DatabaseAccess)> accesses, int vectorLength)
        {
            CsvPath = csvPath;
            Accesses = accesses;
            VectorLength = vectorLength;
        }

        public static IEnumerable<DatabaseAccess> CsvToAccesses(string csvPath, int maxVectorLength)
        {
            return
                File.ReadLines(csvPath)
                    .Select(DatabaseAccess.TryParse)
                    .Where(a => a.HasValue)
                    .Select(a => a.Value)
                    .Where(a => a.TableId < maxVectorLength);
        }

        public static DatabaseAccessesParser Init(string csvPath, int maxVectorLength)
        {
            var vectorLength = CsvToAccesses(csvPath, maxVectorLength).Max(access => access.TableId) + 1;

            return new DatabaseAccessesParser(csvPath, CsvToAccesses(csvPath, maxVectorLength).Pairs(), vectorLength);
        }

        public void Dispose()
        {
            Accesses?.Dispose();
        }

        public Vector[] TakeStep(int numOfNodes, Func<int, int> hashUser, int didntChangeIndex, out bool didEnd)
        {
            didEnd = false;
            Vector[] newVectors = ArrayUtils.Init(numOfNodes, _ => new Vector());
            bool[] didChange = new bool[numOfNodes];
            while (true)
            {
                if (!Accesses.MoveNext())
                {
                    didEnd = true;
                    break;
                }

                var (currentAccess, nextAccess) = Accesses.Current;
                var node = hashUser(currentAccess.UserId);
                didChange[node] = true;
                newVectors[node][currentAccess.TableId] += 1;

                if (currentAccess.Timestamp != nextAccess.Timestamp)
                    break;
            }
            for (int node = 0; node < numOfNodes; node++)
                if (didChange[node] == false)
                    newVectors[node][didntChangeIndex] = 1.0;

            Debug.Assert(newVectors.All(v => v.IndexedValues.Count > 0));
            return newVectors;
        }
    }
}
