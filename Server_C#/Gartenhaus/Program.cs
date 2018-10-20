using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Gartenhaus
{
    class Program
    {
        public static string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Win7\\Documents\\Gartenhaus.mdf;Integrated Security=True;Connect Timeout=30";
        public static int loaclPort = 5000, arduinoport=5001;
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
}
