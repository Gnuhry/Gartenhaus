/*
 * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Gartenhaus_2
{
    public class Client
    {
        private string IP, me;
        public void StartClient(string IPAddress_, string message) //Nachricht an einen Netzwerkteilnehmer senden
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
                client.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), help); //Verbindung versuchen aufzubauen
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
                Send(help.socket, help.sb.ToString()); //Nachricht senden
            }
            catch (Exception) { }
        }
        private void Send(Socket client, string message)
        {
            message += "|";
            byte[] byteData = Encoding.ASCII.GetBytes(message); //String to byte
            Console.WriteLine("Send: " + message);
            client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), client); //Sendevorgang beginnen
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int length = client.EndReceive(ar);
                Console.WriteLine("Sent " + length + " bytes to server.");
                Receive(client); //Auf Nachricht warten
            }
            catch (Exception) { }
        }

        private void Receive(Socket client)
        {
            try
            {
                HelpObject help = new HelpObject { socket = client };
                client.BeginReceive(help.buffer, 0, HelpObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), help); //Empfangen starten
            }
            catch (Exception) { }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                HelpObject help = (HelpObject)ar.AsyncState;
                Socket client = help.socket;
                int length = client.EndReceive(ar); //Empfangen beenden
                if (length > 0)
                {
                    help.sb.Append(Encoding.ASCII.GetString(help.buffer, 0, length));
                    string message = help.sb.ToString();
                    if (message.IndexOf("<EOF>") < 0)
                    {
                        //Wenn Nachricht noch nicht komplett ausgelesen, Vorgang wiederholen
                        client.BeginReceive(help.buffer, 0, HelpObject.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallback), help);
                    }
                    else
                    {
                        //Sockets schließen
                        Console.WriteLine("Reponse: " + message+"\n");
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                    }
                }
            }
            catch (Exception) { new Client().StartClient(IP, me); }
        }
    }
}
