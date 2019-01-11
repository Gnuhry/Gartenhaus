using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace Gartenhaus
{
    /// <summary>
    /// Class for Plant database
    /// </summary>
    public class Plant : DatabaseCommunication
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
        /// <param name="minUV">
        /// The lowest working UV
        /// </param>
        /// <param name="maxUV">
        /// The highest working UV
        /// </param>
        public static int New(string name, float minTemp, float maxTemp, float minGroundHumid, float maxGroundHumid, float minHumid, float maxHumid, float minUV, float maxUV)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "INSERT INTO Plant (Name,MinTemp,MaxTemp,MinGroundHumid,MaxGroundHumid,MinHumid,MaxHumid,MinUV,MaxUV) VALUES (@Name,@MinTemp,@MaxTemp,@MinGroundHumid,@MaxGroundHumid,@MinHumid,@MaxHumid,@MinUV ,@MaxUV)";
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@MinTemp", SqlDbType.Float).Value = minTemp;
                cmd.Parameters.Add("@MaxTemp", SqlDbType.Float).Value = maxTemp;
                cmd.Parameters.Add("@MinGroundHumid", SqlDbType.Float).Value = minGroundHumid;
                cmd.Parameters.Add("@MaxGroundHumid", SqlDbType.Float).Value = maxGroundHumid;
                cmd.Parameters.Add("@MinHumid", SqlDbType.Float).Value = minHumid;
                cmd.Parameters.Add("@MaxHumid", SqlDbType.Float).Value = maxHumid;
                cmd.Parameters.Add("@MinUV", SqlDbType.Float).Value = minUV;
                cmd.Parameters.Add("@MaxUV", SqlDbType.Float).Value = maxUV;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
                return GetIDs().Length - 1;
            }
        }
        static int Sendint;
        static string Sendstring;
        private static void Client1()
        {
            Client.Arduino_Send(Sendint, Sendstring);
        }

        /// <summary>
        /// Change data of a column in database
        /// </summary>
        /// /// <param name="id">
        /// PlantID
        /// </param>
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
        /// <param name="minUV">
        /// The lowest working UV
        /// </param>
        /// <param name="maxUV">
        /// The highest working UV
        /// </param>
        public static void Set(int id, string name, float minTemp, float maxTemp, float minGroundHumid, float maxGroundHumid, float minHumid, float maxHumid, float minUV, float maxUV)
        {
            if (!IsRealID(id))
            {
                return;
            }
            //Serach if the PlantID is register in a arduino and send the changes to it
            int[] ids = Arduino.GetIDs();
            foreach (int iD in ids)
            {
                if (Arduino.GetPlantId(iD) == id)
                {
                    if (minTemp.Equals(Get(iD, "MinTemp")))
                    {
                        Sendint = iD;
                        Sendstring = "MinTemp_" + minTemp;
                        new Thread(Client1).Start();
                    }
                    if (minTemp.Equals(Get(iD, "MaxTemp")))
                    {
                        Sendint = iD;
                        Sendstring = "MaxTemp_" + maxTemp;
                        new Thread(Client1).Start();
                    }
                    if (minTemp.Equals(Get(iD, "MinHumid")))
                    {
                        Sendint = iD;
                        Sendstring = "MinHumid_" + minHumid;
                        new Thread(Client1).Start();
                    }
                    if (minTemp.Equals(Get(iD, "MaxHumid")))
                    {
                        Sendint = iD;
                        Sendstring = "MaxHumid_" + maxHumid;
                        new Thread(Client1).Start();
                    }
                    if (minTemp.Equals(Get(iD, "MinGroundHumid")))
                    {
                        Sendint = iD;
                        Sendstring = "MinGroundHumid_" + minGroundHumid;
                        new Thread(Client1).Start();
                    }
                    if (minTemp.Equals(Get(iD, "MaxGroundHumid")))
                    {
                        Sendint = iD;
                        Sendstring = "MaxGroundHumid_" + maxGroundHumid;
                        new Thread(Client1).Start();
                    }
                    if (minTemp.Equals(Get(iD, "MinUV")))
                    {
                        Sendint = iD;
                        Sendstring = "MinUV_" + minUV;
                        new Thread(Client1).Start();
                    }
                    if (minTemp.Equals(Get(iD, "MaxUV")))
                    {
                        Sendint = iD;
                        Sendstring = "MaxUV_" + maxUV;
                        new Thread(Client1).Start();
                    }
                }
            }
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "UPDATE Plant SET Name=@Name,MinTemp=@MinTemp,MaxTemp=@MaxTemp,MinGroundHumid=@MinGroundHumid," +
                    "MaxGroundHumid=@MaxGroundHumid,MinHumid=@MinHumid,MaxHumid=@MaxHumid,MinUV=@MinUV,MaxUV=@MaxUV WHERE Id=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@MinTemp", SqlDbType.Float).Value = minTemp;
                cmd.Parameters.Add("@MaxTemp", SqlDbType.Float).Value = maxTemp;
                cmd.Parameters.Add("@MinGroundHumid", SqlDbType.Float).Value = minGroundHumid;
                cmd.Parameters.Add("@MaxGroundHumid", SqlDbType.Float).Value = maxGroundHumid;
                cmd.Parameters.Add("@MinHumid", SqlDbType.Float).Value = minHumid;
                cmd.Parameters.Add("@MaxHumid", SqlDbType.Float).Value = maxHumid;
                cmd.Parameters.Add("@MinUV", SqlDbType.Float).Value = minUV;
                cmd.Parameters.Add("@MaxUV", SqlDbType.Float).Value = maxUV;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
            }
        }
        /// <summary>
        /// Get Data from Database
        /// </summary>
        /// <param name="id">
        /// PlantID
        /// </param>
        /// <param name="search">
        /// rowname like "Name","MinTemp","MaxTemp","MinFeucht","MaxFeucht","MinGroundHumid","MaxGroundHumid","MinUV","MaxUV"
        /// </param>
        public static string Get(int id, string search)
        {
            if (!IsRealID(id))
            {
                return "Error";
            }
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT " + search + " FROM Plant WHERE Id=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                reader = cmd.ExecuteReader();
                string erg = "";
                while (reader.Read())
                {
                    erg += reader[search];
                }
                if (search != "Name")
                {
                    erg = Math.Round(Convert.ToSingle(erg), 2).ToString();
                    erg = erg.Replace(',', '.');
                }
                reader.Close();
                return erg;
            }
        }
        /// <summary>
        /// Get All Data from one column
        /// </summary>
        /// <param name="id">
        /// PlantID
        /// </param>
        /// <returns>
        /// Array of Name,MinTemp,MaxTemp,MinGroundHumid,MaxGroundHumid,MinHumid,MaxHumid,MinUV,MaxUV
        /// </returns>
        public static string[] GetAll(int id)
        {
            string[] erg = new string[9];
            erg[0] = Get(id, "Name");
            erg[1] = Get(id, "MinTemp");
            erg[2] = Get(id, "MaxTemp");
            erg[3] = Get(id, "MinGroundHumid");
            erg[4] = Get(id, "MaxGroundHumid");
            erg[5] = Get(id, "MinHumid");
            erg[6] = Get(id, "MaxHumid");
            erg[7] = Get(id, "MinUV");
            erg[8] = Get(id, "MaxUV");
            return erg;
        }
        /// <summary>
        /// Delete a column in database
        /// </summary>
        /// <param name="id">
        /// PlantID
        /// </param>
        public static void Delete(int id)
        {
            if (!IsRealID(id))
            {
                return;
            }
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "DELETE FROM Plant WHERE ID=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
            }
        }
        /// <summary>
        /// Get All IDs from database
        /// </summary>
        public static int[] GetIDs()
        {
            List<int> erg = new List<int>();
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT Id FROM Plant";
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    erg.Add(Convert.ToInt32(reader["Id"]));
                }
            }
            reader.Close();
            return erg.ToArray();
        }
        /// <summary>
        /// Check if ID can be found in database
        /// </summary>
        /// <param name="id">
        /// PlantID
        /// </param>
        /// <returns>
        /// Is the ID in the database
        /// </returns>
        private static bool IsRealID(int id)
        {
            bool realID = false;
            foreach (int Id in GetIDs())
            {
                if (id.Equals(Id))
                {
                    realID = true;
                }
            }
            return realID;
        }
    }
}
