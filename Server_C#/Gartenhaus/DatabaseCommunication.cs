using System;
using System.Data.SqlClient;

namespace Gartenhaus
{

    public class DatabaseCommunication
    {
        protected static string connectionString=Program.connectionString;
        protected static SqlConnection con;
        protected static SqlCommand cmd;
        protected static SqlDataReader reader;

        protected static void OpenConnection()
        {
            if (con == null)
            {
                con = new SqlConnection(connectionString);
            }
                cmd = con.CreateCommand();
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
                catch (Exception) { /*OpenConnection();*/ }
            }
        }
    }
}
