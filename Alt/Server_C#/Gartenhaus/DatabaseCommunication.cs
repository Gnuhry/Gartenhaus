using System;
using System.Data.SqlClient;

namespace Gartenhaus
{
    /// <summary>
    /// Ground class for Database communication
    /// </summary>
    public class DatabaseCommunication
    {
        protected static string connectionString = Program.connectionString;
        protected static SqlConnection con;
        protected static SqlCommand cmd;
        protected static SqlDataReader reader;
        /// <summary>
        /// Open the Connection to the Database
        /// </summary>
        protected static void OpenConnection()
        {
            if (reader != null)
            {
                reader.Close();
            }
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
                catch (Exception) { }
            }
        }
    }
}
