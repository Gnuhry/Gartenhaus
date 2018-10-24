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
    /// <summary>
    /// Object, which store data during the programm life
    /// </summary>
    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}
