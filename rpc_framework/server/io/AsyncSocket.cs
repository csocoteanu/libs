using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using server.threadpool.data;
using server.Properties;
using System.Timers;

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

    public class AsyncSocket : IDisposable
    {
        #region Members
        private byte[] m_buffer = null;
        private StringBuilder m_data = null;
        private Socket m_connection = null;

        private ManualResetEvent m_acceptEvent = null;
        #endregion

        #region Properties
        public Socket Connection { get { return m_connection; } }
        public StringBuilder Data { get { return m_data; } }

        public bool IsConnected
        {
            get
            {
                return this.IsSocketConnected && this.m_connection.Connected;
            }
        }

        private bool IsSocketConnected
        {
            get
            {
                bool part1 = (this.m_connection.Poll(1000, SelectMode.SelectRead));
                bool part2 = (this.m_connection.Available == 0);

                if (part1 && part2)
                    return false;
                else
                    return true;
            }
        }
        #endregion

        public AsyncSocket(Socket connection)
        {
            this.m_connection = connection;
            this.m_buffer = new byte[Settings.Default.kBufferSize];
            this.m_data = new StringBuilder();

            this.m_acceptEvent = new ManualResetEvent(false);
        }

        public void BeginAccept()
        {
            this.m_acceptEvent.Reset();

            this.m_connection.BeginAccept(AcceptCallback, this);

            this.m_acceptEvent.WaitOne();

        }
        public static void AcceptCallback(IAsyncResult ar)
        {
            AsyncSocket listener = (AsyncSocket)ar.AsyncState;
            Socket handler = listener.m_connection.EndAccept(ar);
            AsyncSocket remoteEP = new AsyncSocket(handler);

            Utils.LogInfo(handler, "Incomming Connection");

            listener.m_acceptEvent.Set();

            IncommingConnections.Instance.AddConnection(remoteEP);

            remoteEP.BeginReceive();
        }

        public void BeginReceive()
        {
            this.m_connection.BeginReceive(this.m_buffer, 0, Settings.Default.kBufferSize, 0, ReceiveCallback, this);
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

            readBytes = remoteEP.m_connection.EndReceive(ar);
            remoteEP.m_data.Append(Encoding.ASCII.GetString(remoteEP.m_buffer, 0, readBytes));

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

        public void SendData(byte[] buffer)
        {
            if (!this.IsConnected)
            {
                IncommingConnections.Instance.RemoveConnection(this);
                return;
            }

            this.m_connection.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, this.m_connection);
        }

        #region IDisposable
        public void Dispose()
        {
            try
            {
                m_connection.Close();
            }
            catch
            {
                
            }
        }
        #endregion
    }
}
