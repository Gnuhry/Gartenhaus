using System;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server2
{
    //----------------------------------------- Main -----------------------------------------------

    class Program
    {
        public static int port = 5000;
        static void Main(string[] args)
        {
             Server.StartListening();
        }
    }


    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Buffergröße initalisieren
        public const int BufferSize = 1024;
        // Buffer initalisieren
        public byte[] buffer = new byte[BufferSize];
        // Empfangede Daten String initalisieren
        public StringBuilder sb = new StringBuilder();
    }

    //------------------------------------ SQL Kommunikation ---------------------------------------

    public class SQLCommunication
    {
        //Connection String von der Datenbank
        private static string connectionstring = "";
        //SQL-Kommunikation initalisieren
        private static SqlConnection con = new SqlConnection(connectionstring);
        //SQL Befehlbuilder initalisieren
        private static SqlCommand cmd = con.CreateCommand();

        private static void OpenConnection()//SQL Kommunikation öffnen
        {
            try
            {
                con.Open();
            }
            catch (Exception)
            {
                con.ConnectionString = connectionstring;
                con.Open();
            }
        }

        //------------Datenbank Pflanzen -----------------------------------

        //    Tabellenname: "Verzeichnis"
        //         -Name
        //         -ArduinoIDs
        //         -MinTemp
        //         -MaxTemp
        //         -MinFeucht
        //         -MaxFeucht
        //         -MinHumid
        //         -MaxHumid
        //         -MinUV
        //         -MaxUV
        public static int New(string Name, float MinTemp_, float MaxTemp_, float MinFeucht_, float MaxFeucht_, float MinHumid_, float MaxHumid_, float MinUV_, float MaxUV_) //In Datenbank neue Pflanze ansetzen
        {
            string MinTemp = (MinTemp_ + "").Replace(',', '.');
            string MaxTemp = (MaxTemp_ + "").Replace(',', '.');
            string MinFeucht = (MinFeucht_ + "").Replace(',', '.');
            string MaxFeucht = (MaxFeucht_ + "").Replace(',', '.');
            string MinHumid = (MinHumid_ + "").Replace(',', '.');
            string MaxHumid = (MaxHumid_ + "").Replace(',', '.');
            string MinUV = (MinUV_ + "").Replace(',', '.');
            string MaxUV = (MaxUV_ + "").Replace(',', '.');

            using (con)
            {
                OpenConnection(); //Kommunikation öffnen
                cmd.CommandText = "INSERT INTO Verzeichnis (Name,MinTemp,MaxTemp,MinFeucht,MaxFeucht,MinHumid,MaxHumid,MinUV,MaxUV) VALUES ('" + Name + "'," + MinTemp + "," + MaxTemp + "," + MinFeucht + "," + MaxFeucht + "," + MinHumid + "," + MaxHumid + "," + MinUV + "," + MaxUV + ")"; //Befehl neue Spalte initalisieren
                Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                cmd.CommandText = "SELECT * FROM Verzeichnis WHERE Name = '" + Name + "'"; //Befehl ID finden initalisieren
                SqlDataReader reader = cmd.ExecuteReader(); //Befehl ausführen
                reader.Read();
                string temp = "0";
                try
                {
                    temp = reader["ID"].ToString();
                }
                catch (Exception) { }
                reader.Close();
                // con.Close();
                return Convert.ToInt32(temp); //ID returnen
            }
        }
        public static string Delete(int ID)//In der DAtenbank Pflanze löschen
        {
            using (con)
            {
                OpenConnection(); //Kommunikation öffnen
                cmd.CommandText = "DELETE FROM Verzeichnis WHERE ID = " + ID; //Löschen Befehl initalisieren
                try
                {
                    Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                    return "Sucess";
                }
                catch (Exception) { return "Error"; }
                // con.Close();
            }
        }
        public static string Auslesen(string Search, int ID) //Daten aus Datenbank lesen
        {
            using (con)
            {
                OpenConnection(); //Kommunikation öffnen
                cmd.CommandText = "SELECT * FROM Verzeichnis WHERE Id = " + ID; //Auswahl Befehl initalisieren
                SqlDataReader reader;
                try
                {
                    reader = cmd.ExecuteReader(); //Befehl ausführen
                }
                catch (Exception) { return "Error"; }
                reader.Read();
                string temp;
                try
                {
                    temp = reader[Search].ToString(); //Daten auslesen
                }
                catch (Exception) { return "Error"; }
                //  con.Close();
                reader.Close();
                return temp;//Daten returnen
            }
        }
        public static string Eingabe(string Zeile, object Wert, int ID) //Daten eingeben
        {
            using (con)
            {
                OpenConnection();//Kommunikation öffnen
                cmd.CommandText = "UPDATE Verzeichnis SET " + Zeile + " =" + Wert + " WHERE ID = " + ID; //Eingabe Befehl initalisieren
                try
                {
                    Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                    Client.Arduino_Send(Zeile, ID, Wert);
                    return "Sucess";
                }
                catch (Exception) { return "Error"; }
                // con.Close();
            }
        }
        public static string GetIDs() {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT * FROM Verzeichnis";
                SqlDataReader reader;
                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (Exception) { return "Error"; }
                string temp = "";
                while (reader.Read())
                {
                    try
                    {
                        temp += reader["ID"].ToString() + "_";
                    }
                    catch (Exception) { return "Error"; }

                }
                reader.Close();
                return temp;
            }
        }
        //----------------Datenbank Arduino----------------------------------

        //   Name: "Arduino"
        //      -ArduinoIP
        //      -IDPflanze
        //      -DataSend
        //      -Tabelle

        public static string GetIDsArduino()
        {
            using (con)
            {
                OpenConnection();
                cmd.CommandText = "SELECT * FROM Arduino";
                SqlDataReader reader;
                try
                {
                    reader = cmd.ExecuteReader();
                }
                catch (Exception) { return "Error"; }
                string temp = "";
                while (reader.Read())
                {
                    try
                    {
                        temp += reader["ID"].ToString() + "_";
                    }
                    catch (Exception) { return "Error"; }

                }
                reader.Close();
                return temp;
            }
        }
        public static string SetArduino( int IDPflanze, int Id)
        {
            using (con)
            {
                OpenConnection();//Kommunikation öffnen
                cmd.CommandText = "UPDATE Arduino SET IDPflanze =" + IDPflanze + " WHERE Id = " + Id; //Eingabe Befehl initalisieren
                try
                {
                    Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                    return "Sucess";
                }
                catch (Exception) { return "Error"; }
            }
        }
        public static string SetArduino(string Zeile, int Id, object Wert)
        {
            using (con)
            {
                OpenConnection();//Kommunikation öffnen
                cmd.CommandText = "UPDATE Arduino SET "+Zeile+" =" + Wert + " WHERE Id = " + Id; //Eingabe Befehl initalisieren
                try
                {
                    Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                    return "Sucess";
                }
                catch (Exception) { return "Error"; }
            }
        }
        public static string DeleteArduino(string Id) {
            using (con)
            {
                OpenConnection(); //Kommunikation öffnen
                cmd.CommandText = "DELETE FROM Arduino WHERE ID = " + Id; //Löschen Befehl initalisieren
                try
                {
                    Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                    return "Sucess";
                }
                catch (Exception) { return "Error"; }
                // con.Close();
            }
        }
        public static string GetArduino(string Zeile, int Id)
        {
            using (con)
            {
                OpenConnection();//Kommunikation öffnen
                cmd.CommandText = "SELECT * FROM Arduino WHERE Id="+Id; //Eingabe Befehl initalisieren
                SqlDataReader reader;
                try
                {
                    reader = cmd.ExecuteReader(); //Befehl ausführen
                }
                catch (Exception) { return "Error"; }
                reader.Read();
                string temp;
                try
                {
                    temp = reader[Zeile].ToString(); //Daten auslesen
                }
                catch (Exception) { return "Error"; }
                //  con.Close();
                reader.Close();
                return temp;//Daten returnen
            }
        }

        public static string NewArduino(string[] help, StateObject state)
        {
            Socket handler = state.workSocket;
            if (help[1].ToCharArray().Length == 0)
            {
                using (con)
                {
                    OpenConnection(); //Kommunikation öffnen
                    string ip = (handler.RemoteEndPoint as IPEndPoint).Address.MapToIPv4().ToString();
                    cmd.CommandText = "INSERT INTO Arduino ArduinoIP VALUES " + ip; //Befehl neue Spalte initalisieren
                    Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                    cmd.CommandText = "SELECT * FROM Verzeichnis WHERE AndroidIP = " + ip; //Befehl ID finden initalisieren
                    SqlDataReader reader = cmd.ExecuteReader(); //Befehl ausführen
                    reader.Read();
                    string temp = "";
                    try
                    {
                        temp= reader["Id"].ToString();
                    }
                    catch (Exception) { return "Error"; }
                    reader.Close();
                    cmd.CommandText = "CREATE TABLE Data" + temp + " (Temperatur float, Feuchtigkeit float, Humid float, UV float)"; //Befehl neue Tabelle initalisieren
                    Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                    cmd.CommandText = "UPDATE Arduino SET Tabelle = Data"+temp+" WHERE Id=" + temp; //Befehl TabellenID speichern initalisieren
                    Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                    return temp;
                    // con.Close();
                }
            }
            else
            {
                using (con)
                {
                    //TODO inArduni Arduino id
                    OpenConnection(); //Kommunikation öffnen
                    cmd.CommandText = "SELECT * FROM Arduino WHERE Id = " + help[1]; //Auswahl Befehl initalisieren
                    SqlDataReader reader = null;
                    try
                    {
                        reader = cmd.ExecuteReader(); //Befehl ausführen
                    }
                    catch (Exception) { }
                    reader.Read();
                    string ip = "", senden = null;
                    try
                    {
                        ip = reader["ArduinoIP"].ToString(); //Daten auslesen
                    }
                    catch (Exception) { }
                    try
                    {
                        senden = reader["DataSend"].ToString(); //Daten auslesen
                    }
                    catch (Exception) { }
                    //  con.Close();
                    reader.Close();
                    if (ip != (handler.RemoteEndPoint as IPEndPoint).Address.MapToIPv4().ToString())
                    {
                        OpenConnection();//Kommunikation öffnen
                        cmd.CommandText = "UPDATE Arduino SET ArduinoIP =" + (handler.RemoteEndPoint as IPEndPoint).Address.MapToIPv4().ToString() + " WHERE Id = " + help[1]; //Eingabe Befehl initalisieren
                        try
                        {
                            Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                        }
                        catch (Exception) { }
                        // con.Close();
                    }
                    if (senden != null)
                    {
                        string[] help2 = senden.Split('|');
                        bool a = false, b = false, c = false, d = false, e = false, f = false, g = false, h = false, i = false;//a-mintemp,b-maxtemp,cd feucht, ef humid, gh uv ,i name
                        for (int ff = help2.Length - 1; (ff >= 0) && !(a && b && c && d && e && f && g && h && i); ff--)
                        {
                            switch (help2[ff].Split('_')[0])
                            {
                                case "1": if (!i) { Client.Arduino_Send(help2[ff], Convert.ToInt32(help[1])); i = true; } break;
                                case "2": if (!a) { Client.Arduino_Send(help2[ff], Convert.ToInt32(help[1])); a = true; } break;
                                case "3": if (!b) { Client.Arduino_Send(help2[ff], Convert.ToInt32(help[1])); b = true; } break;
                                case "4": if (!c) { Client.Arduino_Send(help2[ff], Convert.ToInt32(help[1])); c = true; } break;
                                case "5": if (!d) { Client.Arduino_Send(help2[ff], Convert.ToInt32(help[1])); d = true; } break;
                                case "6": if (!e) { Client.Arduino_Send(help2[ff], Convert.ToInt32(help[1])); e = true; } break;
                                case "7": if (!f) { Client.Arduino_Send(help2[ff], Convert.ToInt32(help[1])); f = true; } break;
                                case "8": if (!g) { Client.Arduino_Send(help2[ff], Convert.ToInt32(help[1])); g = true; } break;
                                case "9": if (!h) { Client.Arduino_Send(help2[ff], Convert.ToInt32(help[1])); h = true; } break;
                            }
                        }

                    }
                    return "Sucess";
                }
            }
        }
        public static string SetData(float Temp,float Feucht, float Humid, float UV,string time, int ID)
        {
            string temp;
            using (con)
            {
                OpenConnection(); //Kommunikation öffnen
                cmd.CommandText = "SELECT * FROM Arduino WHERE Id = " + ID; //Auswahl Befehl initalisieren
                SqlDataReader reader;
                try
                {
                    reader = cmd.ExecuteReader(); //Befehl ausführen
                }
                catch (Exception) { return "Error"; }
                reader.Read();
                try
                {
                    temp = reader["Tabelle"].ToString(); //Daten auslesen
                }
                catch (Exception) { return "Error"; }
                //  con.Close();
                reader.Close();

                OpenConnection();//Kommunikation öffnen
                cmd.CommandText = "INSERT INTO "+ temp+ " (Temperatur,Feuchtigkeit,Humid,UV,temp) VALUES ("+Temp+","+Feucht+","+Humid+","+UV+","+time+")"; //Eingabe Befehl initalisieren
                try
                {
                    Console.WriteLine("Datensätze geändert: " + cmd.ExecuteNonQuery()); //Befehl ausführen
                    return "Sucess";
                }
                catch (Exception) { return "Error"; }
            }
        }
        public static string GetLength(int ID)
        {
            string temp = "";
            using (con)
            {
                OpenConnection(); //Kommunikation öffnen
                cmd.CommandText = "SELECT * FROM Arduino WHERE Id = " + ID; //Auswahl Befehl initalisieren
                SqlDataReader reader;
                try
                {
                    reader = cmd.ExecuteReader(); //Befehl ausführen
                }
                catch (Exception) { return "Error"; }
                reader.Read();
                try
                {
                    temp = reader["Tabelle"].ToString(); //Daten auslesen
                }
                catch (Exception) { return "Error"; }
                //  con.Close();
                reader.Close();
                OpenConnection();//Kommunikatio öffnen
                cmd.CommandText = "SELECT COUNT (*) FROM "+temp;
                return (string)cmd.ExecuteScalar();
            }
        }

        public static string GetData(int ID, int dataID)
        {
            string temp;
            using (con)
            {
                OpenConnection(); //Kommunikation öffnen
                cmd.CommandText = "SELECT * FROM Arduino WHERE Id = " + ID; //Auswahl Befehl initalisieren
                SqlDataReader reader;
                try
                {
                    reader = cmd.ExecuteReader(); //Befehl ausführen
                }
                catch (Exception) { return "Error"; }
                reader.Read();
                try
                {
                    temp = reader["Tabelle"].ToString(); //Daten auslesen
                }
                catch (Exception) { return "Error"; }
                //  con.Close();
                reader.Close();
                string erg = "";
                    OpenConnection(); //Kommunikation öffnen
                    cmd.CommandText = "SELECT * FROM " + temp + " WHERE Id = " + dataID; //Auswahl Befehl initalisieren
                    try
                    {
                        reader = cmd.ExecuteReader(); //Befehl ausführen
                    }
                    catch (Exception) { return "Error"; }
                    reader.Read();
                    try
                    {
                        erg+=reader["temp"]+"#"+ reader["Temperatur"].ToString() + "-" + reader["Feuchtigkeit"].ToString() + "-" + reader["Humid"] + "-" + reader["UV"]+"|";
                    }
                    catch (Exception) { return "Error"; }
                //  con.Close();
                reader.Close();
                return erg;
            }
        }
    }


    //---------------------------------- TCP - IP Kommunikation ------------------------------------

    public class Server{
        // Thread Signal
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        public static string GetLocalIPAddress() //Locale Addresse bekommen
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName()); //Host aus DNS-Liste initalisieren
            foreach (IPAddress ip in host.AddressList) //Nach IP-Addresse suchen, die im lokalen Netzwerk ist
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();//IP zurückgeben
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static void StartListening()
        {
            // Lokalen Schlußpunkt initalisieren
            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress());
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Program.port);

            // TCP/IP socket initalisiern
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            //Socket zum lokalen Endpunkt binden und für neue Verbindungen warten
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(Program.port);

                while (true)
                {
                    // Event zu kein Signal Status setzen
                    allDone.Reset();

                    // Asynchronen Socket für kommende Kommunikationen initalisieren
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    //Auf Kommunikation warten und dann weitermachen
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            //Dem Haupt Thread signalisieren fortzusetzen
            allDone.Set();

            //Socket mit dem Client bekommen
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            //StateObject initalisieren
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            //StatObject und Socket bekommen
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Daten vom Socket lesen
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                //Daten speichern, falls weitere DAten kommen
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Nach Ende-Flagge suchen, wenn nicht gefunden, weiterlesen
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    //Fertig gelesen
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);

                    string reponse = null;
                    string[] help = content.Split('_');
                    string txt = help[0];
                    switch (txt.ToLower())
                    {
                        case "get time": reponse = DateTime.Now.ToLongDateString(); break;
                        case "get name": reponse = SQLCommunication.Auslesen("Name", Convert.ToInt32(help[1])); break; //get Name [ID:int]
                        case "get maxhumid": reponse = SQLCommunication.Auslesen("MaxHumid", Convert.ToInt32(help[1])); break; //get maxhumid [ID:int]
                        case "get minhumid": reponse = SQLCommunication.Auslesen("MinHumid", Convert.ToInt32(help[1])); break; //get minhumid [ID:int]
                        case "get maxtemp": reponse = SQLCommunication.Auslesen("MaxTemp", Convert.ToInt32(help[1])); break; //get maxtemp [ID:int]
                        case "get mintemp": reponse = SQLCommunication.Auslesen("MinTemp", Convert.ToInt32(help[1])); break; //get mintemp [ID:int]
                        case "get maxuv": reponse = SQLCommunication.Auslesen("MaxUV", Convert.ToInt32(help[1])); break; //get maxuv [ID:int]
                        case "get minuv": reponse = SQLCommunication.Auslesen("MinUV", Convert.ToInt32(help[1])); break; //get minuv [ID:int]
                        case "get maxfeucht": reponse = SQLCommunication.Auslesen("MaxFeucht", Convert.ToInt32(help[1])); break; //get maxfeucht [ID:int]
                        case "get minfeucht": reponse = SQLCommunication.Auslesen("MinFeucht", Convert.ToInt32(help[1])); break; //get minfeucht [ID:int]
                        case "get ids": reponse = SQLCommunication.GetIDs(); break; //get ids
                        case "get all":
                            reponse =
                                           SQLCommunication.Auslesen("Name", Convert.ToInt32(help[1])) + "_" +
                                           SQLCommunication.Auslesen("MinTemp", Convert.ToInt32(help[1])) + "_" +
                                           SQLCommunication.Auslesen("MaxTemp", Convert.ToInt32(help[1])) + "_" +
                                           SQLCommunication.Auslesen("MinFeucht", Convert.ToInt32(help[1])) + "_" +
                                           SQLCommunication.Auslesen("MaxFeucht", Convert.ToInt32(help[1])) + "_" +
                                           SQLCommunication.Auslesen("MinHumid", Convert.ToInt32(help[1])) + "_" +
                                           SQLCommunication.Auslesen("MaxHumid", Convert.ToInt32(help[1])) + "_" +
                                           SQLCommunication.Auslesen("MinUV", Convert.ToInt32(help[1])) + "_" +
                                           SQLCommunication.Auslesen("MaxUV", Convert.ToInt32(help[1])); break; //get all [ID:int] 


                        case "set arduino": reponse = SQLCommunication.SetArduino("ArduinoID", Convert.ToInt32(help[2]) , Convert.ToInt32(help[1])); break; //set arduino [ID:int] [ArduinoID:int]
                        case "set name": reponse = SQLCommunication.Eingabe("Name", "'" + help[2] + "'", Convert.ToInt32(help[1])); break; //set Name [ID:int] [Name:string]
                        case "set maxhumid": reponse = SQLCommunication.Eingabe("MaxHumid", Convert.ToSingle(help[2]), Convert.ToInt32(help[1])); break; //set maxhumid [ID:int] [MaxHumid:float]
                        case "set minhumid": reponse = SQLCommunication.Eingabe("MinHumid", Convert.ToSingle(help[2]), Convert.ToInt32(help[1])); break; //set minhumid [ID:int] [MinHumid:float]
                        case "set maxtemp": reponse = SQLCommunication.Eingabe("MaxTemp", Convert.ToSingle(help[2]), Convert.ToInt32(help[1])); break; //set maxtemp [ID:int] [MaxTemp:float]
                        case "set mintemp": reponse = SQLCommunication.Eingabe("MinTemp", Convert.ToSingle(help[2]), Convert.ToInt32(help[1])); break; //set mintemp [ID:int] [MinTemp:float]
                        case "set maxuv": reponse = SQLCommunication.Eingabe("MaxUV", Convert.ToSingle(help[2]), Convert.ToInt32(help[1])); break; //set maxuv [ID:int] [MaxUV:float]
                        case "set minuv": reponse = SQLCommunication.Eingabe("MinUV", Convert.ToSingle(help[2]), Convert.ToInt32(help[1])); break; //set minuv [ID:int] [MinUV:float]
                        case "set maxfeucht": reponse = SQLCommunication.Eingabe("MaxFeucht", Convert.ToSingle(help[2]), Convert.ToInt32(help[1])); break; //set maxfeucht [ID:int] [MaxFeucht:float]
                        case "set minfeucht": reponse = SQLCommunication.Eingabe("MinFeucht", Convert.ToSingle(help[2]), Convert.ToInt32(help[1])); break; //set minfeucht [ID:int] [MinFeucht:float]
                        case "set data": reponse = SQLCommunication.SetData(Convert.ToSingle(help[2]), Convert.ToSingle(help[3]), Convert.ToSingle(help[4]), Convert.ToSingle(help[5]), DateTime.Now.ToShortTimeString()+"_"+DateTime.Now.ToShortDateString(), Convert.ToInt32(help[1])); break; //set data [ID:int] [Temperatur:float] [Feuchtigkeit:float] [Humid:float] [UV:float]

                        case "new": reponse = "ID: " + SQLCommunication.New(help[1], Convert.ToSingle(help[2]), Convert.ToSingle(help[3]), Convert.ToSingle(help[4]), Convert.ToSingle(help[5]), Convert.ToSingle(help[6]), Convert.ToSingle(help[7]), Convert.ToSingle(help[8]), Convert.ToSingle(help[9])); break; //new [Name:string] [MaxTemp:float] [...]
                        case "delete": reponse = SQLCommunication.Delete(Convert.ToInt32(help[1])); break; //delete [ID:int]
                        case "new arduino": reponse = SQLCommunication.NewArduino(help, state); break; //new arduino [ArduinoID:int_null]
                        case "delete arduino": reponse = SQLCommunication.DeleteArduino(help[1]); break; //delete arduino [ArduinoID:int]
                        case "get arduinoip": reponse = SQLCommunication.GetArduino("ArduinoIP", Convert.ToInt32(help[1])); break; //get arduinoip [ArduinoID:int]
                        case "get arduino data": reponse = SQLCommunication.GetArduino("DataSend", Convert.ToInt32(help[1])); if (reponse == null) reponse = "Nothing"; break; //get arduinodata [ArduinoID:int]
                        case "get arduinoidpflanze": reponse = SQLCommunication.GetArduino("IDPflanze", Convert.ToInt32(help[1])); break; //get arduinoidpflanze [ArduinoID:int]
                        case "get arduinoids": reponse = SQLCommunication.GetIDsArduino(); break; //get arduinoids
                        case "get data": reponse = SQLCommunication.GetData(Convert.ToInt32(help[1]), Convert.ToInt32(help[2])); break; //get data [ID:int] [DataID:int]
                        case "get length": reponse = SQLCommunication.GetLength(Convert.ToInt32(help[1])); break;//get length [ID:int]
                        case "set arduinoip": reponse= SQLCommunication.SetArduino("IDPflanze",Convert.ToInt32(help[1]),help[2]); break;
                    }
                    //Antwort senden
                    if (reponse == "") reponse = "Error";
                    Send(handler, reponse);
                }
                else
                {
                    // Wenn nicht alle Daten gesendet, weiter suchen
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }


        private static void Send(Socket handler, String data)
        {
            //String mit ASCII verschlüßeln
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            //Start des Sendevorgang
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Socket bekommen
                Socket handler = (Socket)ar.AsyncState;

                // Senden abschließen
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    //------------------------------------- Client Arduino -----------------------------------------

    public class Client
    {
        //Initalisieren der Events welche Warten wenn etwas gesendet oder empfangen wird
        private static ManualResetEvent connectDone =
       new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        private static string response;


        public static void Arduino_Send(string Zeile, int ID, object Wert)//Methode zum Senden an Arduino
        {
            int temp;
            switch (Zeile)
            {
                case "Name": temp = 1; break;
                case "MaxTemp": temp = 3; break;
                case "MinTemp": temp = 2; break;
                case "MaxFeucht": temp = 5; break;
                case "MinFeucht": temp = 4; break;
                case "MaxHumid": temp = 7; break;
                case "MinHumid": temp = 6; break;
                case "MaxUV": temp = 9; break;
                case "MinUV": temp = 8; break;
                default: return;
            }
            StartClient(ID, temp + "_" + Wert);

        }
        public static void Arduino_Send(string Message, int ID)//Methode zum Senden an Arduino
        {
            StartClient(ID, Message);

        }
        private static string GetArduinoIPAddress(int arduinoID)=>SQLCommunication.GetArduino("ArduinoIP", arduinoID); //Methode zum Auslesen der gespeicherten IP-Adresse des Arduino
        private static void StartClient(int ArduinoID,string message) //Methode, welche zuständig für die Übermittlung ist
        {
            try
            {
                //End Addresse festlegen
                IPAddress ipAddress = IPAddress.Parse(GetArduinoIPAddress(ArduinoID));
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Program.port);

                // TCP/IP socket erstellen
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // Verbinden
                    client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                    connectDone.WaitOne();


                // Daten senden
                Send(client, message+"|");
                sendDone.WaitOne();

                // Daten empfangen
                Receive(client);
                receiveDone.WaitOne();
                }
                catch (Exception)
                {
                    //Wenn senden nicht möglich, dann Befehl speichern
                    string temp = SQLCommunication.GetArduino("DataSend", ArduinoID);
                    temp += "|";
                    temp += message;
                    Console.WriteLine("Communication fehlgeschlagen");
                    SQLCommunication.SetArduino("DataSend", ArduinoID, temp);
                    return;
                }
                Console.WriteLine("Response received : {0}", response);

                //Socket schließen
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }



        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Socket auslesen
                Socket client = (Socket)ar.AsyncState;

                // Connection beenden
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                // Programm Signal geben, dass Verbindung besteht
                connectDone.Set();
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // State object initalisieren
                StateObject state = new StateObject();
                state.workSocket = client;

                // Daten empfangen
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                //State Object und Socket auslesen
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Daten auslesen
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // Zwischenspeichern, falls mehr Daten kommen
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Restliche Daten auslesen
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // Daten fertig ausgelesen
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    //Programm Signaldas alle Daten angekommen sind
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            // In ASCII übersetzen
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Senden starten
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Socket auslesen
                Socket client = (Socket)ar.AsyncState;

                // Senden abschließen
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                //Programm Signal das alles gesendet wurde
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }



    }
}
