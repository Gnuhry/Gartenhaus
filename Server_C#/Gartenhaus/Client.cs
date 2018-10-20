using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Gartenhaus
{
    public class Client
    {
        private static ManualResetEvent connectDone =
        new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        private static string response;
        private static void StartClient(int ArduinoID, string message) //Methode, welche zuständig für die Übermittlung ist
        {
            try
            {
                //End Addresse festlegen
                IPAddress ipAddress = IPAddress.Parse(Arduino.GetAll(ArduinoID)[0]);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Program.arduinoport);

                // TCP/IP socket erstellen
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // Verbinden
                    client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                    connectDone.WaitOne();


                    // Daten senden
                    Send(client, message + "|");
                    sendDone.WaitOne();

                    // Daten empfangen
                    Receive(client);
                    receiveDone.WaitOne();
                }
                catch (Exception)
                {
                    //Wenn senden nicht möglich, dann Befehl speichern
                    DataSendSave(ArduinoID, message);
                    return;
                }
                Console.WriteLine("Response received : {0}", response);
                Arduino.RemoveDataSend(ArduinoID);
                //Socket schließen
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void DataSendSave(int id, string message)
        {
            Arduino.AddDataSend(id, message);
        }
        public static void Arduino_Send(int ID, string Message)
        {
            StartClient(ID, Message);
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
