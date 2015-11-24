using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    public class ThreadPool : IDisposable
    {
        public static readonly ThreadPool Instance = new ThreadPool();

        private ushort m_threadPerCore = 0;
        private int m_threadCount = 0;

        private ReadersWritersImpl<Socket> m_readersWritersImpl = null;
        private Worker[] m_workers = null;

        private ThreadPool()
        {
            this.m_threadPerCore = server.Properties.Settings.Default.kThreadsPerCore;
            this.m_threadCount = Environment.ProcessorCount * this.m_threadPerCore;

            this.m_readersWritersImpl = new ReadersWritersImpl<Socket>(this.m_threadCount);

            this.m_workers = new Worker[this.m_threadCount];
            for (int i = 0; i < this.m_workers.Length; i++)
            {
                this.m_workers[i] = new Worker();
                this.m_workers[i].StartWork();
            }
        }

        public Socket GetTask()
        {
            return this.m_readersWritersImpl.ReadNextTask();
        }

        public void AddTask(Socket sock)
        {
            this.m_readersWritersImpl.WriteTask(sock);
        }

        public void Dispose()
        {
            for (int i = 0; i < this.m_workers.Length; i++)
            {
                this.m_workers[i].StopWork();
            }
        }
    }
}
