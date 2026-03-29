using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ItemManager.Infrastructure.Helpers
{
    public class DbHelper
    {
        private readonly string _connectionString;

        // Constructor
        public DbHelper (string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method / Function
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
