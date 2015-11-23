using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private Thread[] m_workerThreads = null;
        private Queue m_tasks = null;

        private ThreadPool()
        {
            this.m_threadPerCore = server.Properties.Settings.Default.kThreadsPerCore;
            this.m_threadCount = Environment.ProcessorCount * this.m_threadPerCore;

            this.m_tasks = new Queue();

            this.m_workerThreads = new Thread[this.m_threadCount];
            for (int i = 0; i < this.m_workerThreads.Length; i++)
            {
                this.m_workerThreads[i] = new Thread(new ThreadStart(this.DoWork));
                this.m_workerThreads[i].Start();
            }
        }

        private void DoWork()
        {

        }

        public void Dispose()
        {
            for (int i = 0; i < this.m_workerThreads.Length; i++)
            {
                this.m_workerThreads[i].Join();
            }
        }
    }
}
