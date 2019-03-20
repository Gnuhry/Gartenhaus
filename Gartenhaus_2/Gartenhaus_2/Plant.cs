using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Gartenhaus_2
{
    //Klasse zum Bearbeiten der Datenbank-Tabelle Plant
    public class Plant
    {

        /// <summary>
        /// Create a new column in database
        /// </summary>
        /// <param name="name">
        /// Name of the Plant
        /// </param>
        /// <param name="minTemp">
        /// The lowest working Temperatur
        /// </param>
        /// <param name="maxTemp">
        /// The highes working Temperatur
        /// </param>
        /// <param name="minGroundHumid">
        /// The lowest working GroundHumid
        /// </param>
        /// <param name="maxGroundHumid">
        /// The highest working GroundHumid
        /// </param>
        /// <param name="minHumid">
        /// The lowest working AirHumid
        /// </param>
        /// <param name="maxHumid">
        /// The highest working AirHumid
        /// </param>
        /// <param name="light">
        /// The light state
        /// </param>
        public static void New(string name, float minTemp, float maxTemp, float minGroundHumid, float maxGroundHumid, float minHumid, float maxHumid, int light)
        {
            Database database = new Database();
            database.OpenConnection();
            Console.WriteLine("Changed: " + database.Write("INSERT INTO Plant (Name,MinTemp,MaxTemp,MinGroundHumid,MaxGroundHumid,MinHumid,MaxHumid,Light) VALUES (@Name,@MinTemp,@MaxTemp,@MinGroundHumid,@MaxGroundHumid,@MinHumid,@MaxHumid,@Light)",
                new string[] { "@Name", "@MinTemp", "@MaxTemp", "@MinGroundHumid", "@MaxGroundHumid", "@MinHumid", "@MaxHumid", "@Light" },
                new object[] { name, minTemp, maxTemp, minGroundHumid, maxGroundHumid, minHumid, maxHumid, light },
                new System.Data.SqlDbType[] { System.Data.SqlDbType.NVarChar, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.TinyInt })
            );
            database.CloseConnection();
        }
        public static void Update(int plantId, string name, float minTemp, float maxTemp, float minGroundHumid, float maxGroundHumid, float minHumid, float maxHumid, int light)
        {
            if (!IsRealID(plantId))
            {
                return;
            }
            object[] data = Get(plantId);
            object[] data2 = new object[] { minTemp, maxTemp, minGroundHumid, maxGroundHumid, minHumid, maxHumid, light };
            for (int f = 0; f < 7; f++)
            {
                if (!data[f + 2].Equals(data2[f]))
                {
                    Arduino.SendToClient(plantId, data2);
                    break;
                }
            }
            Database database = new Database();
            database.OpenConnection();
            Console.WriteLine("Changed: " + database.Write("UPDATE Plant SET Name = @Name, MinTemp = @MinTemp, MaxTemp = @MaxTemp, MinGroundHumid = @MinGroundHumid, " +
                    "MaxGroundHumid=@MaxGroundHumid,MinHumid=@MinHumid,MaxHumid=@MaxHumid,Light=@Light WHERE Id=@Id",
                new string[] { "@Name", "@MinTemp", "@MaxTemp", "@MinGroundHumid", "@MaxGroundHumid", "@MinHumid", "@MaxHumid", "@Light", "@Id" },
                new object[] { name, minTemp, maxTemp, minGroundHumid, maxGroundHumid, minHumid, maxHumid, light, plantId },
                new System.Data.SqlDbType[] { System.Data.SqlDbType.NVarChar, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.TinyInt, System.Data.SqlDbType.Int })
            );
            database.CloseConnection();
        }
        public static void Delete(int plantId)
        {
            if (!IsRealID(plantId))
            {
                return;
            }
            Database database = new Database();
            database.OpenConnection();
            Console.WriteLine("Changed: " + database.Write("DELETE FROM Plant WHERE Id=@Id",
                new string[] { "@Id" },
                new object[] { plantId },
                new System.Data.SqlDbType[] { System.Data.SqlDbType.Int })
            );
            database.CloseConnection();
        }
        public static object[] Get()
        {
            List<object> erg = new List<object>();
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Plant", new string[] { }, new object[] { }, new System.Data.SqlDbType[] { });
            while (sqlDataReader.Read())
            {
                erg.Add(sqlDataReader[0] + "_" + sqlDataReader[1] + "_" + sqlDataReader[2] + "_" + sqlDataReader[3] + "_" + sqlDataReader[4] + "_" + sqlDataReader[5] + "_" + sqlDataReader[6] + "_" + sqlDataReader[7] + "_" + sqlDataReader[8]);
            }
            database.CloseConnection();
            return erg.ToArray();
        }
        public static object[] Get(int plantId)
        {
            if (!IsRealID(plantId))
            {
                return new object[] { "Error" };
            }
            object[] erg = new object[9];
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Plant WHERE Id=@Id", new string[] { "@Id" }, new object[] { plantId }, new System.Data.SqlDbType[] { System.Data.SqlDbType.Int });
            if (sqlDataReader.Read())
            {
                for (int f = 0; f < 9; f++)
                {
                    erg[f] = sqlDataReader[f];
                }
            }
            database.CloseConnection();
            return erg;
        }
        public static object[] Get(string Search)
        {
            List<object> erg = new List<object>();
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Plant", new string[] { }, new object[] { }, new System.Data.SqlDbType[] { });
            while (sqlDataReader.Read())
            {
                erg.Add(sqlDataReader[Search]);
            }
            database.CloseConnection();
            return erg.ToArray();
        }
        public static object Get(int plantId, string Search)
        {
            if (!IsRealID(plantId))
            {
                return "Error";
            }
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Plant WHERE Id=@Id", new string[] { "@Id" }, new object[] { plantId }, new System.Data.SqlDbType[] { System.Data.SqlDbType.Int });
            object erg = "Fehler";
            if (sqlDataReader.Read())
            {
                erg = sqlDataReader[Search];
            }
            database.CloseConnection();
            return erg;
        }
        public static object[] GetDisplay()
        {
            List<object> erg = new List<object>();
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Plant", new string[] { }, new object[] { }, new System.Data.SqlDbType[] { });
            while (sqlDataReader.Read())
            {
                erg.Add(sqlDataReader[0] + "_" + sqlDataReader[1]);
            }
            database.CloseConnection();
            return erg.ToArray();
        }
        public static bool IsRealID(int plantId)
        {
            bool realID = false;
            foreach (int Id in Get("Id"))
            {
                if (plantId.Equals(Id))
                {
                    realID = true;
                }
            }
            return realID;
        }
    }
}
