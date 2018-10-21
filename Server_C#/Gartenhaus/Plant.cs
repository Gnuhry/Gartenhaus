using System;
using System.Collections.Generic;
using System.Data;

namespace Gartenhaus
{
    public class Plant : DatabaseCommunication
    {
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
                Console.WriteLine(cmd.CommandText);
                Console.WriteLine("Changed: " + cmd.ExecuteNonQuery());
                return GetIDs().Length - 1;
            }
        }

        public static void Set(int id, string name, float minTemp, float maxTemp, float minGroundHumid, float maxGroundHumid, float minHumid, float maxHumid, float minUV, float maxUV)
        {
            if (!IsRealID(id))
            {
                return;
            }
            //TODO in Arduino suchen, ob Pflanze registriert
            /*
            if (minTemp.Equals(Get(id, "MinTemp")))
            {
                Client.Arduino_Send(id, "MinTemp_" + minTemp);
            }
            if (minTemp.Equals(Get(id, "MaxTemp")))
            {
                Client.Arduino_Send(id, "MaxTemp_" + maxTemp);
            }
            if (minTemp.Equals(Get(id, "MinHumid")))
            {
                Client.Arduino_Send(id, "MinHumid_" + minHumid);
            }
            if (minTemp.Equals(Get(id, "MaxHumid")))
            {
                Client.Arduino_Send(id, "MaxHumid_" + maxHumid);
            }
            if (minTemp.Equals(Get(id, "MinGroundHumid")))
            {
                Client.Arduino_Send(id, "MinGroundHumid_" + minGroundHumid);
            }
            if (minTemp.Equals(Get(id, "MaxGroundHumid")))
            {
                Client.Arduino_Send(id, "MaxGroundHumid_" + maxGroundHumid);
            }
            if (minTemp.Equals(Get(id, "MinUV")))
            {
                Client.Arduino_Send(id, "MinUV_" + minUV);
            }
            if (minTemp.Equals(Get(id, "MaxUV")))
            {
                Client.Arduino_Send(id, "MaxUV_" + maxUV);
            }*/
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
                    erg=Math.Round(Convert.ToSingle(erg),2).ToString();
                    erg = erg.Replace(',', '.');
                }
                reader.Close();
                return erg;
            }
        }

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
            Console.WriteLine(erg[1]);
            Console.WriteLine(erg[2]);
            Console.WriteLine(erg[3]);
            Console.WriteLine(erg[4]);
            return erg;
        }
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
