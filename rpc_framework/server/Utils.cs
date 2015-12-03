using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    public static class Utils
    {
        public static void DebugInfo(Socket sock, string auxMessage = null)
        {
            IPEndPoint ipRemote = (IPEndPoint)sock.RemoteEndPoint;
            IPEndPoint ipLocal = (IPEndPoint)sock.LocalEndPoint;
            int tid = Thread.CurrentThread.ManagedThreadId;

            Console.WriteLine(string.Format("[Thread: {0}] Local: {1}: {2} Remote: {3}: {4} :: {5}",
                string.Format("({0}:{1})", Thread.CurrentThread.Name, tid), 
                ipLocal.Address.ToString(), 
                ipLocal.Port, 
                ipRemote.Address.ToString(), 
                ipRemote.Port,
                string.IsNullOrEmpty(auxMessage) ? string.Empty : auxMessage));
        }
    }
}
