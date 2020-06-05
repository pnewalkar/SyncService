using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Maintel.Icon.Portal.SyncService.Dod.Helpers
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;
        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void ClearTable(string tablename)
        {
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                    conn.Open();
                    conn.Execute($"TRUNCATE TABLE {tablename}");
                    conn.Close();
            }
        }

        public int NumberOfRecords(string sqlStatement)
        {
            IEnumerable<int> returnVals = new List<int>() { 0 };
            using (IDbConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                returnVals = conn.Query<int>(sqlStatement);
                conn.Close();
            }
            return returnVals.First();
        }


    }
}
