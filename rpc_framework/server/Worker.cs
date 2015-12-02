using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    public abstract class BaseWorker<T>
    {
        protected IRWLock<T> m_rwLock = null;
        protected IRWFactory<T> m_rwFactory = null;
        protected Thread m_thread = null;

        protected abstract void DoWork();
        protected abstract void OnStartWork();
        protected abstract void OnStopWork();

        public BaseWorker(IRWLock<T> rwLock, IRWFactory<T> rwFactory)
        {
            this.m_rwLock = rwLock;
            this.m_rwFactory = rwFactory;

            this.m_thread = new Thread(new ThreadStart(this.DoWork));
            this.m_thread.Name = Utils.kWorkerName;
        }

        public void StartWork()
        {
            this.OnStartWork();
            this.m_thread.Start();
        }

        public void StopWork()
        {
            this.OnStopWork();
            this.m_thread.Abort();
        }

        public void WriteTask(T task)
        {
            if (this.m_rwLock != null)
            {
                this.m_rwLock.WriteTask(task);
            }
        }

        public T ReadTask()
        {
            return (this.m_rwLock != null) ? this.m_rwLock.ReadNextTask() : default(T);
        }
    }

    public class DataProcessor : BaseWorker<string>
    {
        public DataProcessor(IRWLock<string> rwLock, IRWFactory<string> rwFactory) : base(rwLock, rwFactory) { }

        protected override void DoWork()
        {
            while (true)
            {
                string data = this.m_rwLock.ReadNextTask();
                if (!string.IsNullOrEmpty(data))
                {
                    Console.WriteLine("Received: " + data);
                }
            }
        }

        protected override void OnStartWork() { }
        protected override void OnStopWork() { }
    }

    public class SocketReceiver : BaseWorker<Socket>
    {
        private DataProcessor m_processor = null;
        private List<Socket> m_allConnections = null;

        public SocketReceiver(IRWLock<Socket> rwLock, IRWFactory<Socket> rwFactory)
            : base(rwLock, rwFactory)
        {
            IRWFactory<string> dataRWFactory = rwFactory.CloneTo<string>();
            IRWLock<string> dataRWLock = dataRWFactory.CreateRWLock(2);

            m_allConnections = new List<Socket>();
            m_processor = new DataProcessor(dataRWLock, null);
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
                            string data =  System.Text.Encoding.UTF8.GetString(arr).TrimEnd('\0');

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

    public class Worker : BaseWorker<Socket>
    {
        private SocketReceiver m_receiver = null;

        public Worker(IRWLock<Socket> rwLock, IRWFactory<Socket> rwFactory)
            : base(rwLock, rwFactory)
        {
            m_receiver = new SocketReceiver(rwFactory.CreateRWLock(2), rwFactory);
        }

        protected override void DoWork()
        {
            while (true)
            {
                var connection = this.m_rwLock.ReadNextTask();
                if (connection != null)
                {
                    m_receiver.WriteTask(connection);
                }
            }
        }

        protected override void OnStartWork()
        {
            this.m_receiver.StartWork();
        }

        protected override void OnStopWork()
        {
            this.m_receiver.StopWork();
        }
    }
}
