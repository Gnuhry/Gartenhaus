using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Gartenhaus_2
{
    public class Client
    {
        private string IP,me;
        public void StartClient(string IPAddress_, string message)
        {
            IP = IPAddress_; me = message;
            IPAddress address = IPAddress.Parse(IPAddress_);
            IPEndPoint endPoint = new IPEndPoint(address, HelpObject.arduinoport);
            Socket client;
            try
            {
                client = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                HelpObject help = new HelpObject { socket = client };
                help.sb.Append(message);
                client.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), help);
            }
            catch (Exception) { }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                HelpObject help = (HelpObject)ar.AsyncState;
                Socket client = help.socket;
                client.EndConnect(ar);
                Console.WriteLine("Socket connected to " + client.RemoteEndPoint.ToString());
                Send(help.socket, help.sb.ToString());
            }
            catch (Exception) { }
        }
        private void Send(Socket client, string message)
        {
            message += "|";
            byte[] byteData = Encoding.ASCII.GetBytes(message);
            client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int length = client.EndReceive(ar);
                Console.WriteLine("Sent " + length + " bytes to server.");
                Receive(client);
            }
            catch (Exception) { }
        }

        private void Receive(Socket client)
        {
            try
            {
                HelpObject help = new HelpObject { socket = client };
                client.BeginReceive(help.buffer, 0, HelpObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), help);
            }
            catch (Exception) { }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                HelpObject help = (HelpObject)ar.AsyncState;
                Socket client = help.socket;
                int length = client.EndReceive(ar);
                if (length > 0)
                {
                    help.sb.Append(Encoding.ASCII.GetString(help.buffer, 0, length));
                    string message = help.sb.ToString();
                    if (message.IndexOf("<EOF>") < 0)
                    {
                        client.BeginReceive(help.buffer, 0, HelpObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), help);
                    }
                    else
                    {
                        Console.WriteLine("Reponse: " + message);
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                    }
                }
            }
            catch (Exception) { new Client().StartClient(IP, me); }
        }
    }
}
