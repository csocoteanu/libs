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
                sock.Send(Encoding.UTF8.GetBytes("Ana are mere"));

                Console.WriteLine("Succesfully conected to remote endpoint!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occured: " + ex.Message);
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

    }
}
