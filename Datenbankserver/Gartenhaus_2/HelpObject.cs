/*
 * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
 */
using System.Net.Sockets;
using System.Text;

namespace Gartenhaus_2
{
    public class HelpObject //Object, welches beim Zwischenspeichern hilft.
    {
        public Socket socket = null;
        public const int BufferSize = 1024, localport = 5000, arduinoport = 5000;
        public const string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Win7\\source\\repos\\Gartenhaus\\Datenbankserver\\Gartenhaus_2.mdf;Integrated Security=True;Connect Timeout=30"; //muss verändert werden
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }
}
