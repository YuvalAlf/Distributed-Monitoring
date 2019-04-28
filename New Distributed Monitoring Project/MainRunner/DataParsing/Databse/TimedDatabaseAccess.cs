using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataParsing.Databse
{
    public struct TimedDatabaseAccess
    {
        public DatabaseAccess DatabaseAccess    { get; }
        public long Timestamp { get; }

        public TimedDatabaseAccess(DatabaseAccess databaseAccess, long timestamp)
        {
            DatabaseAccess = databaseAccess;
            Timestamp = timestamp;
        }

        public static TimedDatabaseAccess? TryParse(string line)
        {
            var tokens = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != 3)
                return null;
            var res1 = int.TryParse(tokens[0], out int userId);
            var res2 = int.TryParse(tokens[1], out int tableId);
            var res3 = long.TryParse(tokens[2], out long timestamp);
            if (!(res1 && res2 && res3))
                return null;
            return new TimedDatabaseAccess(new DatabaseAccess(userId, tableId), timestamp);
        }

        public static IEnumerable<TimedDatabaseAccess> Parse(string csvPath, int maxVectorLength)
        {
            return
                File.ReadLines(csvPath)
                    .Select(TimedDatabaseAccess.TryParse)
                    .Where(a => a.HasValue)
                    .Select(a => a.Value)
                    .Where(a => a.DatabaseAccess.TableId < maxVectorLength);
        }
    }
}
