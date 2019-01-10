using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Gartenhaus
{
    public class Server2
    {
        TcpListener listener;
        int port=5000;
        public Server2()
        {
            listener = new TcpListener(IPAddress.Parse(Server.GetLocalIPAddress()), port);
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
            byte[] buffer = new byte[] { Convert.ToByte('E'), Convert.ToByte('l') };
            byte[] buffer2=new byte[10000];
            while (client.GetStream().DataAvailable)
            {
                Console.Write(Convert.ToChar(client.GetStream().ReadByte()));
            }
            Console.WriteLine("");
            client.GetStream().Write(buffer,0,2);
            client.Close();
            Console.ReadLine();
        }
    }
}
