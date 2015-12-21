using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using server.io.data;

namespace io.socket
{
    public class AsyncSocket : SocketData, IDisposable
    {
        private ManualResetEvent m_acceptEvent = null;

        public AsyncSocket(int bufferSize, Socket connection)
            : base(bufferSize, connection)
        {
            this.m_acceptEvent = new ManualResetEvent(false);
        }

        public void BeginAccept()
        {
            this.m_acceptEvent.Reset();

            this.m_socket.BeginAccept(AcceptCallback, this);

            this.m_acceptEvent.WaitOne();

        }
        public static void AcceptCallback(IAsyncResult ar)
        {
            AsyncSocket listener = (AsyncSocket)ar.AsyncState;
            Socket handler = listener.m_socket.EndAccept(ar);
            AsyncSocket remoteEP = new AsyncSocket(listener.m_bufferSize, handler);

            // Utils.LogInfo(handler, "Incomming Connection");

            listener.m_acceptEvent.Set();

            // IncommingConnections.Instance.AddConnection(remoteEP);

            remoteEP.BeginReceive();
        }

        public void BeginReceive()
        {
            this.m_socket.BeginReceive(this.m_buffer, 0, m_bufferSize, 0, ReceiveCallback, this);
        }
        public static void ReceiveCallback(IAsyncResult ar)
        {
            AsyncSocket remoteEP = (AsyncSocket)ar.AsyncState;
            int readBytes = 0;

            if (!remoteEP.IsConnected)
            {
                // IncommingConnections.Instance.RemoveConnection(remoteEP);
                return;
            }

            readBytes = remoteEP.Connection.EndReceive(ar);
            remoteEP.AppendData(remoteEP.m_buffer);

            if (readBytes == remoteEP.m_bufferSize)
            {
                remoteEP.BeginReceive();
            }
            else
            {
                if (remoteEP.m_data.Length > 1)
                {
                    string content = remoteEP.m_data.ToString();
                    Console.WriteLine("Read {0} bytes from socket.\n Data : {1}", content.Length, content);

                    remoteEP.SendData(Encoding.ASCII.GetBytes(content));
                }
            }
        }

        public override void SendData(byte[] buffer)
        {
            if (!this.IsConnected)
            {
                // IncommingConnections.Instance.RemoveConnection(this);
                return;
            }

            base.SendData(buffer);
        }

        #region IDisposable
        public void Dispose()
        {
            try
            {
                this.m_socket.Close();
            }
            catch
            {

            }
        }
        #endregion
    }
}
