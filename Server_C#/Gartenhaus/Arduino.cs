using System;
using System.Collections.Generic;
using System.Data;

namespace Gartenhaus
{
    public class Arduino : DatabaseCommunication
    {
        public Arduino(string connectionString_) : base(connectionString_)
        {
        }

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
            }
            return erg.ToArray();
        }
        public static int New(string ArduinoIP)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT * FROM Arduino WHERE ArduinoIP=@ArduinoIP";
                cmd.Parameters.Add("@ArduinoIP", SqlDbType.NVarChar).Value = ArduinoIP;
                reader = cmd.ExecuteReader();
                if (reader.FieldCount > 0)
                {
                    reader.Read();
                    Delete(Convert.ToInt32(reader["Id"]));
                }
                cmd.CommandText = "INSERT INTO Arduino (ArduinoIP) VALUES (@ArduinoIP)";
                cmd.Parameters.Add("@ArduinoIP", SqlDbType.NVarChar).Value = ArduinoIP;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
                return GetIDs().Length - 1;
            }
        }
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
                    if (!(reader["DataSend"].ToString().Equals("") || reader["DataSend"].ToString().Equals(null)))
                    {
                        Client.Arduino_Send(id, "ArduinoID_" + id);
                    }
                }
            }
        }

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
        }
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
        private static void SetDataSend(int Id, string dataSend)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "Update Arduino SET DataSend = @dataSend WHERE ID=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                cmd.Parameters.Add("@dataSend", SqlDbType.NVarChar).Value = dataSend;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
            }
        }
        public static void RemoveDataSend(int Id)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "Update Arduino SET DataSend = ' ' WHERE ID=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
            }
        }
        public static string[] GetAll(int Id)
        {
            string[] erg = new string[3];
            erg[0] = Get(Id, "ArduinoIP");
            erg[1] = GetPlantName(Id);
            erg[2] = Get(Id, "DataSend");
            return erg;
        }
        public static void AddDataSend(int Id, string message)
        {
            if (message.Split('_')[0]=="Name")
            {
                return;
            }
            string[] help = GetAll(Id)[2].Split('|');
            string erg = "";
            for (int f = 0; f < help.Length; f++)
            {
                if (message.Split('_')[0].Equals(help[f].Split('_')[0]))
                {
                    erg += message + "|";
                }
                else
                {
                    erg += help[f] + "|";
                }
            }
            SetDataSend(Id, erg);
        }
        private static string Get(int Id, string Search)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT " + Search + " From Arduino WHERE Id=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                reader = cmd.ExecuteReader();
                reader.Read();
                return reader.ToString();
            }
        }
        private static string GetPlantName(int Id)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT Plant.Name From Plant INNER JOIN Arduino ON Plant.Id=Arduino.PlantID WHERE Arduino.Id=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = Id;
                reader = cmd.ExecuteReader();
                reader.Read();
                return reader.ToString();
            }
        }
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

        public static void SetData(int arduinoID, float temperatur, float humid, float groundhumid, float uv)
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "INSERT INTO Data (time,arduinoId,temperatur,humid,groundhumid,uv) VALUES (@Day,@Id,@temperatur,@humid,@groundhumid,@uv)";
                cmd.Parameters.Add("@Day", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters.Add("@temperatur", SqlDbType.Float).Value = temperatur;
                cmd.Parameters.Add("@humid", SqlDbType.Float).Value = humid;
                cmd.Parameters.Add("@groundhumid", SqlDbType.Float).Value = groundhumid;
                cmd.Parameters.Add("@uv", SqlDbType.Float).Value = uv;
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = arduinoID;
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
            }
        }
        public static string GetAllData(int arduinoID)
        {
            using (con)
            {
                string erg="";
                OpenConnection();
                cmd.CommandText = "SELLECT * FROM Data WHERE arduinoId=@Id";
                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = arduinoID;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    erg += reader["time"] + "_" + reader["temperatur"] + "_" + reader["humid"] + "_" + reader["groundhumid"] + "_" + reader["uv"]+"|";
                }
                return erg;
            }
        }
    }
}
