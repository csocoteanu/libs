using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace client_test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                IPEndPoint configuredEndPoint = new IPEndPoint(ipAddress, 65000);
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                sock.Connect(configuredEndPoint);

                Console.WriteLine("Succesfully conected to remote endpoint!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: " + ex.Message);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}
