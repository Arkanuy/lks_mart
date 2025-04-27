using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace test
{
    class Connector
    {
        public SqlConnection getCon()
        {
            SqlConnection Con = new SqlConnection();

            Con.ConnectionString = "Data Source = Arkan; Initial Catalog = lks_mart; Integrated Security = true";

            return Con; 
        }
    }
}
