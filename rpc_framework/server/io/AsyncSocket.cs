using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using server.Properties;
using System.Timers;
using server.io.data;

namespace server.io
{
    public class IncommingConnections : List<AsyncSocket>
    {
        private System.Timers.Timer m_timer = null;

        public const int kIntervalTick = 60000;
        public const int kMaxSocketConnections = 64000;
        public static readonly IncommingConnections Instance = new IncommingConnections();

        private IncommingConnections()
        {
            this.InitTimer();
        }

        private void InitTimer()
        {
            m_timer = new System.Timers.Timer(kIntervalTick);
            m_timer.Elapsed += OnTimedEvent;
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            this.ClearUnconnected();
        }

        private void ClearUnconnected()
        {
            lock (IncommingConnections.Instance)
            {
                for (int i = 0; i < IncommingConnections.Instance.Count; )
                {
                    AsyncSocket asyncConnection = IncommingConnections.Instance[i];
                    if (asyncConnection.IsConnected)
                    {
                        i++;
                    }
                    else
                    {
                        Utils.LogInfo(asyncConnection.Connection, "Closing connection....");

                        IncommingConnections.Instance.RemoveAt(i);
                        asyncConnection.Dispose();
                    }
                }
            }
        }

        public void AddConnection(AsyncSocket connection)
        {
            lock (IncommingConnections.Instance)
            {
                if (IncommingConnections.Instance.Count >= kMaxSocketConnections)
                    this.ClearUnconnected();
                IncommingConnections.Instance.Add(connection);
            }
        }

        public void RemoveConnection(AsyncSocket connection)
        {
            lock (IncommingConnections.Instance)
            {
                IncommingConnections.Instance.Remove(connection);
            }
        }
    }

    public class AsyncSocket : SocketData, IDisposable
    {
        private ManualResetEvent m_acceptEvent = null;

        public AsyncSocket(Socket connection) : base(connection)
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
            AsyncSocket remoteEP = new AsyncSocket(handler);

            Utils.LogInfo(handler, "Incomming Connection");

            listener.m_acceptEvent.Set();

            IncommingConnections.Instance.AddConnection(remoteEP);

            remoteEP.BeginReceive();
        }

        public void BeginReceive()
        {
            this.m_socket.BeginReceive(this.m_buffer, 0, Settings.Default.kBufferSize, 0, ReceiveCallback, this);
        }
        public static void ReceiveCallback(IAsyncResult ar)
        {
            AsyncSocket remoteEP = (AsyncSocket)ar.AsyncState;
            int readBytes = 0;

            if (!remoteEP.IsConnected)
            {
                IncommingConnections.Instance.RemoveConnection(remoteEP);
                return;
            }

            readBytes = remoteEP.Connection.EndReceive(ar);
            remoteEP.AppendData(remoteEP.m_buffer);

            if (readBytes == Settings.Default.kBufferSize)
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
                IncommingConnections.Instance.RemoveConnection(this);
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
