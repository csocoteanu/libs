using server.Properties;
using server.rwlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.threadpool.workers
{
    public class SocketReceiverThread : BaseWorker<Socket>
    {
        protected override string WorkerName { get { return Settings.Default.kReceiver; } }

        private DataProcessorThread m_processor = null;
        private List<Socket> m_allConnections = null;

        public SocketReceiverThread(IRWLock<Socket> rwLock, IRWFactory<Socket> rwFactory)
            : base(rwLock, rwFactory)
        {
            IRWFactory<string> dataRWFactory = rwFactory.CloneTo<string>();
            IRWLock<string> dataRWLock = dataRWFactory.CreateRWLock(2);

            m_allConnections = new List<Socket>();
            m_processor = new DataProcessorThread(dataRWLock, null);
        }

        private bool SocketConnected(Socket s)
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }

        protected override void DoWork()
        {
            while (true)
            {
                Socket newConnection = this.m_rwLock.ReadNextTask();
                if (newConnection != null)
                    m_allConnections.Add(newConnection);

                for (int i = 0; i < m_allConnections.Count; )
                {
                    Socket connection = m_allConnections[i];
                    if (SocketConnected(connection) && connection.Connected)
                    {
                        if (connection.Available > 0)
                        {
                            byte[] arr = new byte[1024];
                            connection.Receive(arr);
                            string data = System.Text.Encoding.UTF8.GetString(arr).TrimEnd('\0');

                            Utils.DebugInfo(connection, "New connection...");

                            m_processor.WriteTask(data);
                        }

                        i++;
                    }
                    else
                    {
                        m_allConnections.Remove(connection);
                        Utils.DebugInfo(connection, "Closing connection!");
                    }
                }
            }
        }

        protected override void OnStartWork()
        {
            this.m_processor.StartWork();
        }

        protected override void OnStopWork()
        {
            this.m_processor.StopWork();
        }
    }
}
