using System.Data;
using System.Data.SqlClient;

namespace Gartenhaus_2
{
    //Klasse zum Bearbeiten der Datenbank
    public class Database
    {
        private SqlConnection sqlConnection;
        public void OpenConnection()
        {
            sqlConnection = new SqlConnection(HelpObject.connectionString);
            sqlConnection.Open();
        }
        public void CloseConnection()
        {
            sqlConnection.Close();
        }

        public SqlDataReader Read(string cmd, string[] Parameter, object[] value, SqlDbType[] type)
        {
            SqlCommand sqlcmd = sqlConnection.CreateCommand();
            sqlcmd.CommandText = cmd;
            for (int f = 0; f < Parameter.Length && f < value.Length && f < type.Length; f++)
            {
                sqlcmd.Parameters.Add(Parameter[f], type[f]).Value = value[f];
            }
            return sqlcmd.ExecuteReader();
        }
        public int Write(string cmd, string[] Parameter, object[] value, SqlDbType[] type)
        {
            SqlCommand sqlcmd = sqlConnection.CreateCommand();
            sqlcmd.CommandText = cmd;
            for (int f = 0; f < Parameter.Length && f < value.Length && f < type.Length; f++)
            {
                sqlcmd.Parameters.Add(Parameter[f], type[f]).Value = value[f];
            }
            return sqlcmd.ExecuteNonQuery();
        }
    }
}
