using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    public abstract class BaseWorker
    {
        protected Thread m_thread = null;

        protected abstract void DoWork();
        protected abstract void OnStartWork();
        protected abstract void OnStopWork();

        public BaseWorker()
        {
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
    }

    public class Processor : BaseWorker
    {
        private List<Socket> m_allConnections = null;
        private ReadersWritersImpl<Socket> m_clientConnections = null;

        public Processor(ReadersWritersImpl<Socket> connections)
        {
            m_allConnections = new List<Socket>();
            m_clientConnections = connections;
        }

        bool SocketConnected(Socket s)
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
                IList<Socket> newConnections = m_clientConnections.ReadAllTasks();
                if (newConnections != null)
                    m_allConnections.AddRange(newConnections);

                for (int i = 0; i < m_allConnections.Count; )
                {
                    Socket connection = m_allConnections[i];
                    if (SocketConnected(connection) && connection.Connected)
                    {
                        if (connection.Available > 0)
                        {
                            byte[] arr = new byte[1024];
                            connection.Receive(arr);
                            Console.WriteLine("Received: " + System.Text.Encoding.UTF8.GetString(arr).TrimEnd('\0')); 
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

        protected override void OnStartWork() { }
        protected override void OnStopWork() { }
    }

    public class Worker : BaseWorker
    {
        private Processor m_processor = null;
        private ReadersWritersImpl<Socket> m_clientConnections = null;

        public Worker()
        {
            m_clientConnections = new ReadersWritersImpl<Socket>(2);
            m_processor = new Processor(m_clientConnections);
        }

        protected override void DoWork()
        {
            while (true)
            {
                var connection = ThreadPool.Instance.GetTask();
                if (connection != null)
                {
                    m_clientConnections.WriteTask(connection);
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
