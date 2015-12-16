using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server;

namespace server_test
{
    class Program
    {
        static void Main(string[] args)
        {
            eServerMode serverMode = eServerMode.kAsyncSocket;
            RPCServer server = new RPCServer(serverMode);
            server.Start(false);
        }
    }
}
