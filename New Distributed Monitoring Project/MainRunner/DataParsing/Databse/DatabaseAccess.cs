using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParsing.Databse
{
    public struct DatabaseAccess
    {
        public int UserId  { get; }
        public int TableId { get; }

        public DatabaseAccess(int userId, int tableId)
        {
            UserId = userId;
            TableId = tableId;
        }
    }
}
