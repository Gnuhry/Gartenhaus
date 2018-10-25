using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Gartenhaus
{
    /// <summary>
    /// Class for Server
    /// </summary>
    public class Server
    {
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        /// <summary>
        /// Get Local IP Adress
        /// </summary>
        public static string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        /// <summary>
        /// Start Server listening
        /// </summary>
        public static void StartListening()
        {
            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress());
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Program.loaclPort);

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(Program.loaclPort);

                while (true)
                {
                    allDone.Reset();
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);
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
        /// <summary>
        /// Accept Callback
        /// </summary>
        private static void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject
            {
                workSocket = handler
            };
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }
        /// <summary>
        /// Read Callback
        /// </summary>
        private static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    //Read done
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);

                    string reponse = "";
                    string[] help = content.Split('_');
                    string txt = help[0];
                    //Analyze message
                    switch (txt.ToLower())
                    {
                        case "get time": //get time
                            reponse = DateTime.Now.ToLongDateString();
                            break;
                        case "get plant name": //get plant name_[Name:string]
                            reponse = Plant.Get(Convert.ToInt32(help[1]), "Name");
                            break;
                        case "get plant all"://get plant all_[PlantID:int]
                            foreach (string text in Plant.GetAll(Convert.ToInt32(help[1])))
                                reponse += text + "_";
                            break;
                        case "get plant ids"://get plant ids
                            foreach (int text in Plant.GetIDs())
                                reponse += text + "_";
                            break;
                        case "get plant names"://get plant names
                            reponse = "";
                            foreach (int id in Plant.GetIDs())
                            {
                                reponse += Plant.Get(id, "Name") + "_";
                            }
                            break;

                        case "set plant"://set plant_[PlantID:int]_[Name:string]_[MinTemp:float]_[MaxTemp:float]_[MinGroundHumid:float]_[MaxGroundHumid:float]_
                            //_[MinHumid:float]_[MaxHumid:float]_[MinUV:float]_[MaxUV:float]
                            Plant.Set(Convert.ToInt32(help[1]), help[2], Convert.ToSingle(help[3]), Convert.ToSingle(help[4]),
                                Convert.ToSingle(help[5]), Convert.ToSingle(help[6]), Convert.ToSingle(help[7]),
                                Convert.ToSingle(help[8]), Convert.ToSingle(help[9]), Convert.ToSingle(help[10]));
                            reponse = "Success";
                            break;
                        case "new plant"://new plant_[Name:string]_[MinTemp:float]_[MaxTemp:float]_[MinGroundHumid:float]_[MaxGroundHumid:float]_
                            //_[MinHumid:float]_[MaxHumid:float]_[MinUV:float]_[MaxUV:float]
                            Plant.New(help[1], Convert.ToSingle(help[2]), Convert.ToSingle(help[3]),
                                Convert.ToSingle(help[4]), Convert.ToSingle(help[5]), Convert.ToSingle(help[6]),
                                Convert.ToSingle(help[7]), Convert.ToSingle(help[8]), Convert.ToSingle(help[9]));
                            reponse = "Success";
                            break;
                        case "delete plant"://delete plant_[PlantID:int]
                            Plant.Delete(Convert.ToInt32(help[1]));
                            reponse = "Success";
                            break;


                        case "get arduino all"://get arduino all_[ArduinoID:int]
                            foreach (string text in Arduino.GetAll(Convert.ToInt32(help[1])))
                                reponse += text + "_";
                            break;
                        case "get arduino ids"://get arduino ids
                            foreach (int text in Arduino.GetIDs())
                                reponse += text + "_";
                            Console.WriteLine(reponse);
                            break;
                        case "set arduino arduinoip"://set arduino arduinoip_[Arduinoip:int]_[Arduinoip:string]
                            Arduino.SetArduinoIP(Convert.ToInt32(help[1]), help[2]);
                            reponse = "Success";
                            break;
                        case "set arduino plantid"://set arduino plantid_[ArduinoID:int]_[PlantID:int]
                            Arduino.SetPlantID(Convert.ToInt32(help[1]), Convert.ToInt32(help[2]));
                            reponse = "Success";
                            break;
                        case "set arduino"://set arduino_[ArduinoID]_[ArduinoIP:string]_[PlantID:int]
                            Arduino.SetArduinoIP(Convert.ToInt32(help[1]), help[2]);
                            if (Convert.ToInt32(help[3]+1) < 1)
                            {
                                Arduino.RemovePlantID(Convert.ToInt32(help[1]));
                            }
                            else
                            {
                                Arduino.SetPlantID(Convert.ToInt32(help[1]), Plant.GetIDs()[Convert.ToInt32(help[3])]);
                            }
                            reponse = "Success";
                            break;
                        case "new arduino"://new arduino
                            Console.WriteLine("New_" + handler.RemoteEndPoint.ToString().Split(':')[0]);
                            reponse = "" + Arduino.New(handler.RemoteEndPoint.ToString().Split(':')[0]);
                            break;
                        case "reconect arduino"://reconect arduino_[ArduinoID:string]
                            Arduino.Reconect(Convert.ToInt32(help[1]));
                            reponse = "Success";
                            break;
                        case "delete arduino"://delete arduino_[ArduinoID:int]
                            Arduino.Delete(Convert.ToInt32(help[1]));
                            reponse = "Success";
                            break;


                        case "set arduino data"://set arduino data_[ArduinoID:int]_[Temperatur:float]_[Humid:float]_[GroundHumid:float]_[UV:float]
                            Arduino.SetData(Convert.ToInt32(help[1]), Convert.ToSingle(help[2]), Convert.ToSingle(help[3]), Convert.ToSingle(help[4]), Convert.ToSingle(help[5]));
                            reponse = "Success";
                            break;
                        case "get arduino data"://get arduino data
                            reponse = Arduino.GetAllData(Convert.ToInt32(help[1]));
                            break;
                    }
                    //send 
                    if (reponse == "") reponse = "Error";
                    Send(handler, reponse);
                }
                else
                {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        /// <summary>
        /// Send
        /// </summary>
        private static void Send(Socket handler, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }
        /// <summary>
        /// Send Callback
        /// </summary>
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;

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
