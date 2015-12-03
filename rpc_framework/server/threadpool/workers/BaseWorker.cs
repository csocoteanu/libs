using server.Properties;
using server.rwlock;
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

        protected abstract string WorkerName { get; }
        protected abstract void DoWork();
        protected abstract void OnStartWork();
        protected abstract void OnStopWork();

        static int ms_count = 0;

        public BaseWorker(IRWLock<T> rwLock, IRWFactory<T> rwFactory)
        {
            this.m_rwLock = rwLock;
            this.m_rwFactory = rwFactory;

            this.m_thread = new Thread(new ThreadStart(this.DoWork));
            this.m_thread.Name = this.WorkerName;

            Console.WriteLine(string.Format("Spawning thread: {0}{1}", this.m_thread.Name, ms_count++));
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
}
