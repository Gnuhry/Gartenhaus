using System.Net.Sockets;
using System.Text;

namespace Gartenhaus_2
{
    public class HelpObject
    {
        public Socket socket = null;
        public const int BufferSize = 1024, localport = 5000, arduinoport = 5000;
        public const string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Win7\\source\\repos\\Gartenhaus\\Gartenhaus_2\\Gartenhaus_2.mdf;Integrated Security=True;Connect Timeout=30";
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}
