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
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide string argument for remote host.");
                return;
            }

            for (int i = 0; i < 1/*00000*/; i++)
            {
                try
                {
                    string data = args[0];
                    byte[] receiveBuffer = new byte[data.Length + 1];
                    IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                    IPEndPoint configuredEndPoint = new IPEndPoint(ipAddress, 65000);
                    Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    sock.Connect(configuredEndPoint);
                    Console.WriteLine("Succesfully conected to remote endpoint!");

                    sock.Send(Encoding.UTF8.GetBytes(data));
                    Console.WriteLine("Succesfully send string to remote endpoint!");

                    sock.Receive(receiveBuffer);
                    Console.WriteLine("Succesfully received string from remote endpoint: " + Encoding.UTF8.GetString(receiveBuffer));

                    sock.Close();
                    Console.WriteLine("Succesfully closed connection to remote endpoint!............#" + i);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error occured: " + ex.Message);
                } 
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

    }
}
