using server.rwlock;
using server.threadpool.workers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server.threadpool
{
    public abstract class ThreadPool : IDisposable
    {
        protected int m_threadCount = 0;

        protected IRWLock<Socket> m_rwLock = null;
        protected DispatcherThread[] m_workers = null;
        protected abstract IRWFactory<Socket> RWFactory { get; }

        protected ThreadPool()
        {
            this.m_threadCount = Utils.DebugMode ? 0 : Environment.ProcessorCount;
            this.m_rwLock = this.RWFactory.CreateRWLock(this.m_threadCount);

            this.m_workers = new DispatcherThread[this.m_threadCount];
            for (int i = 0; i < this.m_workers.Length; i++)
            {
                this.m_workers[i] = new DispatcherThread(this.m_rwLock, this.RWFactory);
                this.m_workers[i].StartWork();
            }
        }

        public void AddNewConnection(Socket sock)
        {
            this.m_rwLock.WriteTask(sock);
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
