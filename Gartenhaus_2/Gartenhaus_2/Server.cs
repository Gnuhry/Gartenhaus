using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gartenhaus_2
{
    public class Server
    {
        private static ManualResetEvent manuelresetevent = new ManualResetEvent(false);
        private Socket server;
        private string threadhelp;
        private string ArduinoIP;

        public void StartServer()
        {
            IPAddress ipAddress = IPAddress.Parse(GetLocalIPAddress());
            server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Bind(new IPEndPoint(ipAddress, HelpObject.localport));
                server.Listen(HelpObject.localport);
                while (true)
                {
                    manuelresetevent.Reset();
                    Console.WriteLine("Waiting for a connection...");
                    server.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        server);
                    manuelresetevent.WaitOne();
                }
            }
            catch (Exception)
            {

            }


        }

        private void AcceptCallback(IAsyncResult ar)
        {
            manuelresetevent.Set();
            Socket client = ((Socket)ar.AsyncState).EndAccept(ar);
            HelpObject help = new HelpObject { socket = client };
            client.BeginReceive(help.buffer, 0, HelpObject.BufferSize, SocketFlags.None, new AsyncCallback(ReadCallback), help);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                string message;
                HelpObject help = ((HelpObject)ar.AsyncState);
                Socket client = help.socket;
                int length = client.EndReceive(ar);
                if (length > 1)
                {
                    help.sb.Append(Encoding.ASCII.GetString(help.buffer, 0, length));
                    message = help.sb.ToString();

                    if (message.IndexOf("<EOF>") < 0)
                    {
                        client.BeginReceive(help.buffer, 0, HelpObject.BufferSize, SocketFlags.None, new AsyncCallback(ReadCallback), help);
                    }
                    else
                    {
                        ArduinoIP = client.RemoteEndPoint.ToString().Split(':')[0];
                        string reponse = Processing(message.Substring(0, message.IndexOf("<EOF>")));
                        Console.WriteLine("Answer: " + reponse);
                        Send(client, reponse);
                    }
                }
            }
            catch (Exception) { }
        }
        private string Processing(string message)
        {
            Console.WriteLine("Message from client: " + message);
            switch (message.ToLower().Split('_')[0].Split(' ')[0])
            {
                case "get": return GetProcessing(message);
                case "set": case "new": case "delete": case "reconect": case "live": threadhelp = message; new Thread(Process).Start(); return "Succes";
                default: return "Error";
            }
        }
        private void Process()
        {
            string[] tile = threadhelp.Split('_');
            switch (tile[0].ToLower())
            {
                case "set plant": Plant.Update(Convert.ToInt32(tile[1]), tile[2], Convert.ToSingle(tile[3]), Convert.ToSingle(tile[4]), Convert.ToSingle(tile[5]), Convert.ToSingle(tile[6]), Convert.ToSingle(tile[7]), Convert.ToSingle(tile[8]), Convert.ToInt32(tile[9])); break;//set plant_[PlantID:int]_[Name:string]_[MinTemp:float]_[MaxTemp:float]_[MinGroundHumid:float]_[MaxGroundHumid:float]_[MinHumid:float]_[MaxHumid:float]_[Light:float]
                case "new plant": Plant.New(tile[1], Convert.ToSingle(tile[2]), Convert.ToSingle(tile[3]), Convert.ToSingle(tile[4]), Convert.ToSingle(tile[5]), Convert.ToSingle(tile[6]), Convert.ToSingle(tile[7]), Convert.ToInt32(tile[8])); break;//new plant_[Name:string]_[MinTemp:float]_[MaxTemp:float]_[MinGroundHumid:float]_[MaxGroundHumid:float]_[MinHumid:float]_[MaxHumid:float]_[Light:float]
                case "delete plant": Plant.Delete(Convert.ToInt32(tile[1])); break;//delete plant_[PlantID:int]
                case "set arduino plantid": Arduino.Update(Convert.ToInt32(tile[1]), Convert.ToInt32(tile[2])); break;//set arduino plantid_[ArduinoID:int]_[PlantID:int]
                case "set arduino": Arduino.Update(Convert.ToInt32(tile[1]), Convert.ToInt32(tile[3]), tile[2]); break;//set arduino_[ArduinoID:int]_[ArduinoIP:string]_[PlantID:int]
                case "new arduino": Arduino.New(ArduinoIP); break;//new arduino
                case "reconect arduino": Arduino.Reconect(Convert.ToInt32(tile[1]), ArduinoIP); break;//reconect arduino_[ArduinoID:int]
                case "delete arduino": Arduino.Delete(Convert.ToInt32(tile[1])); break;//delete arduino_[ArduinoID:int]
                case "set arduino data": try { Arduino.SetData(Convert.ToInt32(tile[1]), Convert.ToSingle(tile[2]), Convert.ToSingle(tile[3]), Convert.ToSingle(tile[4]), Convert.ToInt32(tile[5])); } catch (FormatException) { } break;//set arduino data_[ArduinoID:int]_[Temperatur:float]_[Humid:float]_[GroundHumid:float]_[Light:int]
                case "live": Arduino.Live(ArduinoIP, Convert.ToInt32(tile[1])); break;
            }
        }

        private string GetProcessing(string message)
        {
            StringBuilder erg = new StringBuilder();
            string[] tile = message.Split('_');
            switch (tile[0].ToLower())
            {
                case "get time": return DateTime.Now.ToLongDateString(); //get time
                case "get plant name": return Plant.Get(Convert.ToInt32(tile[1]), "Name").ToString(); //get plant name_[Name:string]
                case "get plant all": foreach (object o in Plant.Get(Convert.ToInt32(tile[1]))) { erg.Append(o + "_"); } return erg.ToString(); //get plant all_[PlantID:int]
                case "get plant ids": foreach (object o in Plant.Get("Id")) { erg.Append(o + "_"); } return erg.ToString();//get plant ids
                case "get plant names": foreach (object o in Plant.Get("Name")) { erg.Append(o + "_"); } return erg.ToString();//get plant names
                case "get plant display": foreach (object o in Plant.GetDisplay()) { erg.Append(o + ";"); } return erg.ToString();//get plant display
                case "get arduino all": foreach (object o in Arduino.Get(Convert.ToInt32(tile[1]))) { erg.Append(o + "_"); } return erg.ToString(); //get arduino all_[ArduinoId:int]
                case "get arduino ids": foreach (object o in Arduino.Get("Id")) { erg.Append(o + "_"); } return erg.ToString();//get arduino ids
                case "get arduino data": foreach (object o in Arduino.GetDataAll()) { erg.Append(o + ";"); } return erg.ToString(); //get arduino data
                case "get arduino display": foreach (object o in Arduino.GetDisplay()) { erg.Append(o + ";"); } return erg.ToString();//get arduino display
                default: return "Error";
            }
        }

        private void Send(Socket client, string data)
        {
            if (data.Trim().Equals("") || data.Trim().Equals(null))
            {
                data = "Error";
            }
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int lentgh = client.EndSend(ar);
                Console.WriteLine("Sent " + lentgh + " bytes to client");
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (Exception) { }
        }

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
    }
}
