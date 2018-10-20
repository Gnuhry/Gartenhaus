using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Gartenhaus
{
    public class Server
    {
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
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Program.loaclPort);

            // TCP/IP socket initalisiern
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            //Socket zum lokalen Endpunkt binden und für neue Verbindungen warten
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(Program.loaclPort);

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
            StateObject state = new StateObject
            {
                workSocket = handler
            };
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

                    string reponse = "";
                    string[] help = content.Split('_');
                    string txt = help[0];
                    switch (txt.ToLower())
                    {
                        case "get time":
                            reponse = DateTime.Now.ToLongDateString();
                            break;
                        case "get plant name":
                            reponse = Plant.Get(Convert.ToInt32(help[1]), "Name");
                            break;
                        case "get plant all":
                            foreach (string text in Plant.GetAll(Convert.ToInt32(help[1])))
                                reponse += text + "_";
                            break;
                        case "get plant ids":
                            foreach (int text in Plant.GetIDs())
                                reponse += text + "_";
                            break;
                        case "get plant names":
                            reponse = "";
                            foreach (int id in Plant.GetIDs())
                            {
                                reponse += Plant.Get(id, "Name") + "_";
                            }
                            break;

                        case "set plant":
                            Plant.Set(Convert.ToInt32(help[1]), help[2], Convert.ToSingle(help[3]), Convert.ToSingle(help[4]),
                                Convert.ToSingle(help[5]), Convert.ToSingle(help[6]), Convert.ToSingle(help[7]),
                                Convert.ToSingle(help[8]), Convert.ToSingle(help[9]), Convert.ToSingle(help[10]));
                            reponse = "Success";
                            break;
                        case "new plant":
                            Plant.New(help[1], Convert.ToSingle(help[2]), Convert.ToSingle(help[3]),
                                Convert.ToSingle(help[4]), Convert.ToSingle(help[5]), Convert.ToSingle(help[6]),
                                Convert.ToSingle(help[7]), Convert.ToSingle(help[8]), Convert.ToSingle(help[9]));
                            reponse = "Success";
                            break;
                        case "delete plant":
                            Plant.Delete(Convert.ToInt32(help[1]));
                            reponse = "Success";
                            break;


                        case "get arduino all":
                            foreach (string text in Arduino.GetAll(Convert.ToInt32(help[1])))
                                reponse += text + "_";
                            break;
                        case "get arduino ids":
                            foreach (int text in Arduino.GetIDs())
                                reponse += text + "_";
                            break;
                        case "set arduino arduinoIP":
                            Arduino.SetArduinoIP(Convert.ToInt32(help[1]), help[2]);
                            reponse = "Success";
                            break;
                        case "set arduino PlantID":
                            Arduino.SetPlantID(Convert.ToInt32(help[1]), Convert.ToInt32(help[2]));
                            reponse = "Success";
                            break;
                        case "set arduino":
                            Arduino.SetArduinoIP(Convert.ToInt32(help[1]), help[2]);
                            Arduino.SetPlantID(Convert.ToInt32(help[1]), Plant.GetIDs()[Convert.ToInt32(help[3])]);
                            reponse = "Success";
                            break;
                        case "new arduino":
                            reponse = ""+Arduino.New(help[1]); 
                            break;
                        case "reconect arduino":
                            Arduino.Reconect(Convert.ToInt32(help[1]));
                            reponse = "Success";
                            break;
                        case "delete arduino":
                            Arduino.Delete(Convert.ToInt32(help[1]));
                            reponse = "Success";
                            break;


                        case "set arduino data":
                            Arduino.SetData(Convert.ToInt32(help[1]), Convert.ToSingle(help[2]), Convert.ToSingle(help[3]), Convert.ToSingle(help[4]), Convert.ToSingle(help[5]));
                            reponse = "Success";
                            break;
                        case "get arduino data":
                            reponse = Arduino.GetAllData(Convert.ToInt32(help[1]));
                            break;
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
}
