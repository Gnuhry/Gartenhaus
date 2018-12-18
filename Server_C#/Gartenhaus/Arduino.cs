using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace Gartenhaus
{
    /// <summary>
    /// Class for the Arduino database
    /// </summary>
    public class Arduino : DatabaseCommunication
    {
        /// <summary>
        /// Get all Arduino Ids from Database
        /// </summary>
        public static int[] GetIDs()
        {
            List<int> erg = new List<int>();
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT Id FROM Arduino";
                reader = cmd.ExecuteReader();
                for (int f = 0; f < reader.FieldCount; f++)
                {
                    reader.Read();
                    erg.Add(Convert.ToInt32(reader["Id"]));
                }
                reader.Close();
            }
            return erg.ToArray();
        }
        /// <summary>
        /// Create a new colum in Arduino database
        /// </summary>
        /// <param name="ArduinoIP">
        /// IP-Adress of the Arduino
        /// </param>
        public static int New(string ArduinoIP)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT * FROM Arduino WHERE ArduinoIP=@ArduinoIP";
                cmd.Parameters.Add("@ArduinoIP", SqlDbType.NVarChar).Value = ArduinoIP;
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Delete(Convert.ToInt32(reader["Id"]));
                }
                reader.Close();
                OpenConnection();
                cmd.CommandText = "INSERT INTO Arduino (ArduinoIP) VALUES (@ArduinoIP)";
                cmd.Parameters.Add("@ArduinoIP", SqlDbType.NVarChar).Value = ArduinoIP;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
                return GetIDs().Length - 1;
            }
        }
        /// <summary>
        /// Methode, when Arduino reconect to this Server
        /// </summary>
        /// <param name="id">
        /// Arduino ID
        /// </param>
        public static void Reconect(int id)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT * FROM Arduino WHERE Id=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                reader = cmd.ExecuteReader();
                if (reader.FieldCount > 0)
                {
                    reader.Read();
                    if (!reader["PlantID"].ToString().Equals("0"))
                    {
                        Sendint = id;
                        Sendstring = Convert.ToInt32(reader["PlantID"]);
                        new Thread(Client1).Start();
                    }
                    else
                    {
                        Sendint = id;
                        Sendstring = 0;
                        new Thread(Client1).Start();
                    }
                }
                reader.Close();
            }
        }
        /// <param name="Id">
        /// ArduinoID
        /// </param>
        public static void SetPlantID(int Id, int PlantID)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "Update Arduino SET PlantID = @PlantID WHERE ID=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                cmd.Parameters.Add("@PlantID", SqlDbType.Int).Value = PlantID;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
            }
            Sendint = Id;
            Sendstring = PlantID;

            new Thread(Client1).Start();

        }
        static int Sendint;
        static int Sendstring;
        private static void Client1()
        {
            if (Sendstring == 0)
            {
                Client.Arduino_Send(Sendint, "ID_"+Sendstring+";");
                return;
            }
            string[] PlantData = Plant.GetAll(Sendstring);
            string send=Sendstring+ ";" + PlantData[1]+ ";" + PlantData[2]+";" + PlantData[3]
            +";" + PlantData[4]+";" + PlantData[5]+";" + PlantData[6]+ ";" + PlantData[7]+";" + PlantData[8];
            Client.Arduino_Send(Sendint, send);
        }

        /// <param name="Id">
        /// ArduinoID
        /// </param>
        public static void SetArduinoIP(int Id, string ArduinoIP)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "Update Arduino SET ArduinoIP = @ArduinoIP WHERE ID=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                cmd.Parameters.Add("@ArduinoIP", SqlDbType.NVarChar).Value = ArduinoIP;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
            }
        }
        /// <summary>
        /// Get the ID of the Plant
        /// </summary>
        /// <param name="Id">
        /// ArduinoID
        /// </param>
        public static int GetPlantId(int Id)
        {
            return Convert.ToInt32(Get(Id, "Id"));
        }
        /// <summary>
        /// Get ArduinoIP and Plant name
        /// </summary>
        /// <param name="Id">
        /// ArduinoID
        /// </param>
        public static string[] GetAll(int Id)
        {
            string[] erg = new string[3];
            erg[0] = Get(Id, "ArduinoIP");
            erg[1] = GetPlantName(Id);
            if (erg[1] is null)
            {
                erg[1] = "keine";
            }
            return erg;
        }
        /// <summary>
        /// Get Data from Database
        /// </summary>
        /// <param name="Id">
        /// ArduinoID
        /// </param>
        /// <param name="Search">
        /// rowname "PlantID", "ArduinoIP"
        /// </param>
        private static string Get(int Id, string Search)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT " + Search + " From Arduino WHERE Id=@ID";
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = Id;
                using (reader)
                {
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        if (reader[Search] == null)
                        {
                            return "Error";
                        }
                    }
                    return reader[Search].ToString();
                }
            }
        }
        /// <summary>
        /// Set the PlantID to null
        /// </summary>
        /// <param name="id">
        /// ArduinoID
        /// </param>
        public static void RemovePlantID(int id)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "Update Arduino SET PlantID = '' WHERE ID=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
            }
            Sendint = id;
            Sendstring = 0;
            new Thread(Client1).Start();
        }
        /// <summary>
        /// Get the Name of the Plant in the Arduino
        /// </summary>
        /// <param name="Id">
        /// ArduinoID
        /// </param>
        /// <returns></returns>
        private static string GetPlantName(int Id)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT Plant.Name From Plant INNER JOIN Arduino ON Plant.Id=Arduino.PlantID WHERE Arduino.Id=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                using (reader)
                {
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        return reader["Name"].ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        /// <summary>
        /// Remove Arduino from database
        /// </summary>
        /// <param name="Id">
        /// ArduinoID
        /// </param>
        public static void Delete(int Id)
        {
            if (!IsRealID(Id))
            {
                return;
            }
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "DELETE FROM Arduino WHERE ID=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
            }
        }
        /// <summary>
        /// Check if the ID can be found in the database
        /// </summary>
        /// <param name="id">
        /// ArduinoID
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
        /// <summary>
        /// Set Data from now
        /// </summary>
        /// <param name="arduinoID">
        /// ArduinoID
        /// </param>
        /// <param name="temperatur">
        /// Temperatur
        /// </param>
        /// <param name="humid">
        /// Humid (Air)
        /// </param>
        /// <param name="groundhumid">
        /// Humid (Ground)
        /// </param>
        /// <param name="uv">
        /// UV
        /// </param>
        public static void SetData(int arduinoID, float temperatur, float humid, float groundhumid, float uv)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "INSERT INTO Data (time,arduinoId,Temperatur,Humid,GroundHumid,UV) VALUES (@Day,@Id,@temperatur,@humid,@groundhumid,@uv)";
                cmd.Parameters.Add("@Day", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters.Add("@temperatur", SqlDbType.Float).Value = temperatur;
                cmd.Parameters.Add("@humid", SqlDbType.Float).Value = humid;
                cmd.Parameters.Add("@groundhumid", SqlDbType.Float).Value = groundhumid;
                cmd.Parameters.Add("@uv", SqlDbType.Float).Value = uv;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = arduinoID;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
            }
        }
        /// <summary>
        /// Get All Data which is store in the database
        /// </summary>
        /// <param name="arduinoID">
        /// ArduinoID
        /// </param>
        /// <returns>Time_Temperatur_Humid_GroundHumid|Time_ [...]</returns>
        public static string GetAllData(int arduinoID)
        {
            using (con)
            {
                string erg = "";
                OpenConnection();
                cmd.CommandText = "SELLECT * FROM Data WHERE arduinoId=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = arduinoID;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    erg += reader["time"] + "_" + reader["temperatur"] + "_" + reader["humid"] + "_" + reader["groundhumid"] + "_" + reader["uv"] + "|";
                }
                reader.Close();
                return erg;
            }
        }
    }
}
