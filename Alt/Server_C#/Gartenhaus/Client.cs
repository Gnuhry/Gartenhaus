using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Gartenhaus
{
    /// <summary>
    /// Class for Communication with the Arduino
    /// </summary>
    public class Client
    {
        private static ManualResetEvent connectDone =
        new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        private static string response;
        /// <summary>
        /// Start the transmition
        /// </summary>
        /// <param name="message">
        /// Comand for Arduino
        /// </param>
        private static void StartClient(int ArduinoID, string message)
        {
            try
            {
                //Set end adress
                Console.WriteLine(ArduinoID + "");
                IPAddress ipAddress = IPAddress.Parse(Arduino.GetAll(ArduinoID)[0]);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, Program.arduinoport);
                Socket client;
                try
                {
                    client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    // Connect
                    client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                    connectDone.WaitOne();


                    // Transmition
                    Send(client, message + "|");
                    sendDone.WaitOne();

                    // Receive
                    Receive(client);
                    receiveDone.WaitOne();
                }
                catch (Exception)
                {
                    return;
                }
                Console.WriteLine("Response received : {0}", response);
                //Close
                client.Shutdown(SocketShutdown.Both);
                client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Send to Arduino public Methode
        /// </summary>
        /// <param name="ID">
        /// Arduino ID
        /// </param>
        /// <param name="Message">
        /// Command for Arduino
        /// </param>
        public static void Arduino_Send(int ID, string Message)
        {
            StartClient(ID, Message);
            
        }
        /// <summary>
        /// Connection
        /// </summary>
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());

                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        /// <summary>
        /// Receive
        /// </summary>
        private static void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject
                {
                    workSocket = client
                };

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        /// <summary>
        /// Receive Callback
        /// </summary>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        /// <summary>
        /// Send
        /// </summary>
        private static void Send(Socket client, String data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }
        /// <summary>
        /// Send Callback
        /// </summary>
        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


    }
}
