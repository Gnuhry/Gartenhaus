using System;
using System.Data.SqlClient;

namespace Gartenhaus
{

    public class DatabaseCommunication
    {
        protected static string connectionString;
        protected static SqlConnection con;
        protected static SqlCommand cmd;
        protected static SqlDataReader reader;
        public DatabaseCommunication(string connectionString_)
        {
            connectionString = connectionString_;
            con = new SqlConnection(connectionString);
            cmd = con.CreateCommand();
        }

        protected static void OpenConnection()
        {
            try
            {
                con.Open();
            }
            catch (Exception)
            {
                try
                {
                    con.ConnectionString = connectionString;
                    con.Open();
                }
                catch (Exception) { }
            }
        }
    }
}
