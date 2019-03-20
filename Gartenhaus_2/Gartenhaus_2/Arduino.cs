using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Gartenhaus_2
{
    //Klassen zum Bearbeiten der Datenbank-Tabelle Arduino
    public class Arduino
    {
        public static void SendToClient(int plantId, object[] data)
        {
            foreach (string ip in GetAllIPs(plantId))
            {
                new Client().StartClient(ip.Split('_')[1], ip.Split('_')[0] + "a" + data[0] + "b" + data[1] + "c" + data[2] + "d" + data[3] + "e" + data[4] + "f" + data[5] + "g" + data[6]);
            }
        }

        public static void SendToClient(int plantId, int arduinoId)
        {
            if (plantId > 0)
            {
                object[] plant = Plant.Get(plantId);
                new Client().StartClient(Get(arduinoId)[1].ToString(), arduinoId + "a" + plant[2] + "b" + plant[3] + "c" + plant[4] + "d" + plant[5] + "e" + plant[6] + "f" + plant[7] + "g" + plant[8]);
            }
            else
            {
                new Client().StartClient(Get(arduinoId)[1].ToString(), arduinoId + "a-100b-100c-100d-100e-100f-100g-100");
            }
        }

        public static void New(string ArduinoIP)
        {
            DeleteAll(ArduinoIP);
            Database database = new Database();
            database.OpenConnection();
            Console.WriteLine("Changed: " + database.Write("INSERT INTO Arduino (ArduinoIP) VALUES (@ArduinoIP)", new string[] { "@ArduinoIP" }, new object[] { ArduinoIP }, new System.Data.SqlDbType[] { System.Data.SqlDbType.NVarChar }));
            database.CloseConnection();
            object[] IDS = Get("ID");
            SendToClient(0, Convert.ToInt32(IDS[IDS.Length - 1]));
        }

        public static void Reconect(int ArduinoId, string ArduinoIP)
        {
            if (!IsRealID(ArduinoId))
            {
                New(ArduinoIP);
                return;
            }
            object[] help = Get(ArduinoId);
            if (!help[1].Equals(ArduinoIP))
            {
                Update(ArduinoId, Convert.ToInt32(help[2]), ArduinoIP);
            }
            SendToClient(Convert.ToInt32(help[2]), ArduinoId);
        }

        public static void Update(int ArduinoId, int PlantId)
        {
            if (!IsRealID(ArduinoId))
            {
                return;
            }
            if (!Plant.IsRealID(PlantId) && PlantId != 0)
            {
                return;
            }
            Database database = new Database();
            database.OpenConnection();
            Console.WriteLine("Changed: " + database.Write("UPDATE Arduino SET PlantId = @PlantId WHERE Id=@Id",
                new string[] { "@PlantId", "@Id" },
                new object[] { PlantId, ArduinoId },
                new System.Data.SqlDbType[] { System.Data.SqlDbType.Int, System.Data.SqlDbType.Int })
            );
            database.CloseConnection();

            SendToClient(PlantId, ArduinoId);
        }

        public static void Update(int ArduinoId, int PlantId, string ArduinoIP)
        {
            if (!IsRealID(ArduinoId))
            {
                return;
            }
            if (!Plant.IsRealID(PlantId) && PlantId != 0)
            {
                return;
            }
            Database database = new Database();
            database.OpenConnection();
            Console.WriteLine("Changed: " + database.Write("UPDATE Arduino SET ArduinoIP=@ArduinoIP, PlantId = @PlantId WHERE Id=@Id",
                new string[] { "@ArduinoIP", "@PlantId", "@Id" },
                new object[] { ArduinoIP, PlantId, ArduinoId },
                new System.Data.SqlDbType[] { System.Data.SqlDbType.NVarChar, System.Data.SqlDbType.Int, System.Data.SqlDbType.Int })
            );
            database.CloseConnection();
            SendToClient(PlantId, ArduinoId);
        }

        public static void Delete(int ArduinoId)
        {
            if (!IsRealID(ArduinoId))
            {
                return;
            }
            Database database = new Database();
            database.OpenConnection();
            Console.WriteLine("Changed: " + database.Write("DELETE FROM Arduino WHERE Id=@Id",
                new string[] { "@Id" },
                new object[] { ArduinoId },
                new System.Data.SqlDbType[] { System.Data.SqlDbType.Int })
            );
            database.CloseConnection();
        }

        public static void DeleteAll(int PlantId)
        {
            if (!Plant.IsRealID(PlantId))
            {
                return;
            }
            Database database = new Database();
            database.OpenConnection();
            Console.WriteLine("Changed: " + database.Write("DELETE FROM Arduino WHERE PlantId=@Id",
                new string[] { "@Id" },
                new object[] { PlantId },
                new System.Data.SqlDbType[] { System.Data.SqlDbType.Int })
            );
            database.CloseConnection();
        }

        public static void DeleteAll(string ArduinoIP)
        {
            Database database = new Database();
            database.OpenConnection();
            Console.WriteLine("Changed: " + database.Write("DELETE FROM Arduino WHERE ArduinoIP=@IP",
                new string[] { "@IP" },
                new object[] { ArduinoIP },
                new System.Data.SqlDbType[] { System.Data.SqlDbType.NVarChar })
            );
            database.CloseConnection();
        }

        public static object[] Get()
        {
            List<object> erg = new List<object>();
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Arduino", new string[] { }, new object[] { }, new System.Data.SqlDbType[] { });
            while (sqlDataReader.Read())
            {
                erg.Add(sqlDataReader[0] + "_" + sqlDataReader[1] + "_" + sqlDataReader[2]);
            }
            database.CloseConnection();
            return erg.ToArray();
        }

        public static object[] Get(int ArduinoId)
        {
            if (!IsRealID(ArduinoId))
            {
                return new object[] { "Error" };
            }
            object[] erg = new object[3];
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Arduino WHERE Id=@Id", new string[] { "@Id" }, new object[] { ArduinoId }, new System.Data.SqlDbType[] { System.Data.SqlDbType.Int });
            if (sqlDataReader.Read())
            {
                for (int f = 0; f < 3; f++)
                {
                    erg[f] = sqlDataReader[f];
                }
                if (erg[2].ToString().Trim().Equals(null) || erg[2].ToString().Trim().Equals(""))
                {
                    erg[2] = "0";
                }
            }
            database.CloseConnection();
            return erg;
        }

        public static object Get(int ArduinoId, string Search)
        {
            if (!IsRealID(ArduinoId))
            {
                return "Error";
            }
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Arduino WHERE Id=@Id", new string[] { "@Id" }, new object[] { ArduinoId }, new System.Data.SqlDbType[] { System.Data.SqlDbType.Int });
            object erg = "";
            if (sqlDataReader.Read())
            {
                erg = sqlDataReader[Search];
            }
            database.CloseConnection();
            return erg;
        }

        public static object[] Get(string Search)
        {
            List<object> erg = new List<object>();
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Arduino", new string[] { }, new object[] { }, new System.Data.SqlDbType[] { });
            while (sqlDataReader.Read())
            {
                erg.Add(sqlDataReader[Search]);
            }
            database.CloseConnection();
            return erg.ToArray();
        }

        public static object[] GetAllIPs(int PlantId)
        {
            if (!Plant.IsRealID(PlantId))
            {
                return new object[] { "Error" };
            }
            List<object> erg = new List<object>();
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Arduino WHERE PlantId=@Id", new string[] { "@Id" }, new object[] { PlantId }, new System.Data.SqlDbType[] { System.Data.SqlDbType.Int });
            if (sqlDataReader.Read())
            {
                for (int f = 0; f < 4; f++)
                {
                    erg.Add(sqlDataReader[0] + "_" + sqlDataReader[1]);
                }
            }
            database.CloseConnection();
            return erg.ToArray();
        }

        public static object[] GetDisplay()
        {
            object[] help = Get();
            for (int f = 0; f < help.Length; f++)
            {
                object[] help2 = help[f].ToString().Split('_');
                if (help2[2].ToString().Trim() != null && help2[2].ToString().Trim() != "" && help2[2].ToString().Trim() != "0")
                {
                    help2[2] = Plant.Get(Convert.ToInt32(help[f].ToString().Split('_')[2]), "Name");
                }
                else
                {
                    help2[2] = "noch keine";
                }
                object help3 = help2[0] + "_" + help2[2] + "_" + help2[1];
                help[f] = help3;
            }
            return help;
        }

        public static void Live(string IP, int Id)
        {
            if (!IsRealID(Id))
            {
                return;
            }
            new Client().StartClient(Get(Id, "ArduinoIP").ToString(), "_" + IP);
        }

        private static bool IsRealID(int ArduinoId)
        {
            bool realID = false;
            foreach (int Id in Get("Id"))
            {
                if (ArduinoId.Equals(Id))
                {
                    realID = true;
                }
            }
            return realID;
        }

        public static void SetData(int arduinoID, float temperatur, float humid, float groundhumid, int light)
        {
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("INSERT INTO Data (time,ArduinoId,Temperatur,Humid,GroundHumid,Light) VALUES (@Day,@Id,@temperatur,@humid,@groundhumid,@light)",
                new string[] { "@Day", "@Id", "@temperatur", "@humid", "@groundhumid", "@light" },
                new object[] { DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), arduinoID, temperatur, humid, groundhumid, light },
                new System.Data.SqlDbType[] { System.Data.SqlDbType.DateTime, System.Data.SqlDbType.Int, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.Float, System.Data.SqlDbType.Int });
            database.CloseConnection();
        }

        public static object[] GetDataAll()
        {
            List<object> erg = new List<object>();
            List<object> erg2 = new List<object>();
            List<int> Plantids = new List<int>();
            Database database = new Database();
            database.OpenConnection();
            SqlDataReader sqlDataReader = database.Read("SELECT * FROM Data,Arduino WHERE Data.ArduinoId=Arduino.Id", new string[] { }, new object[] { }, new System.Data.SqlDbType[] { });
            while (sqlDataReader.Read())
            {
                erg.Add(sqlDataReader["ArduinoId"] + "_" + sqlDataReader["ArduinoIP"] + "_" + sqlDataReader["time"] + "_" + sqlDataReader["Temperatur"] + "_" + sqlDataReader["Humid"] + "_" + sqlDataReader["GroundHumid"] + "_" + sqlDataReader["Light"]);
                try
                {
                    Plantids.Add(Convert.ToInt32(sqlDataReader["PlantId"]));
                }
                catch (Exception)
                {
                    Plantids.Add(0);
                }
            }
            database.CloseConnection();
            for (int f = 0; f < erg.Count; f++)
            {
                string he = ":" + erg[f].ToString().Split('_')[0] + "-" + erg[f].ToString().Split('_')[1];
                bool ac = false;
                for (int g = 0; g < erg2.Count && !ac; g++)
                {
                    ac = erg2[g].ToString().Split('-')[0].Substring(1) == he.Split('-')[0].Substring(1);
                }
                if (!ac)
                {
                    try
                    {
                        object[] plant = Plant.Get(Plantids[f]);
                        erg2.Add(he + "_" + plant[2] + "_" + plant[3] + "_" + plant[4] + "_" + plant[5] + "_" + plant[6] + "_" + plant[7] + "_1000");
                    }
                    catch (Exception es)
                    {
                        erg2.Add(he);
                    }
                }
            }
            erg2.AddRange(erg);
            return erg2.ToArray();
        }
    }
}
