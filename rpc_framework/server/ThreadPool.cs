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
    public abstract class ThreadPool : IDisposable
    {
        protected int m_threadCount = 0;

        protected IRWLock<Socket> m_rwLock = null;
        protected Worker[] m_workers = null;
        protected abstract IRWFactory<Socket> RWFactory { get; }

        protected ThreadPool()
        {
            this.m_threadCount = Environment.ProcessorCount;
            this.m_rwLock = this.RWFactory.CreateRWLock(this.m_threadCount);

            this.m_workers = new Worker[this.m_threadCount];
            for (int i = 0; i < this.m_workers.Length; i++)
            {
                this.m_workers[i] = new Worker(this.m_rwLock, this.RWFactory);
                this.m_workers[i].StartWork();
            }
        }

        public Socket GetTask()
        {
            return this.m_rwLock.ReadNextTask();
        }

        public void AddTask(Socket sock)
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

    public class BlockingQueueThreadPool : ThreadPool
    {
        public static readonly BlockingQueueThreadPool Instance = new BlockingQueueThreadPool();

        protected override IRWFactory<Socket> RWFactory
        {
            get { return BlockingQueueRWFactory<Socket>.Instance; }
        }

        public class BlockingQueueRWFactory<T> : IRWFactory<T>
        {
            public static readonly BlockingQueueRWFactory<T> Instance = new BlockingQueueRWFactory<T>();

            private BlockingQueueRWFactory() { }

            public IRWLock<T> CreateRWLock(int maxThreadCount)
            {
                return new BlockingQueue<T>();
            }

            public IRWFactory<U> CloneTo<U>()
            {
                return BlockingQueueRWFactory<U>.Instance;
            }
        }
    }

    public class RWThreadPool : ThreadPool
    {
        public static readonly RWThreadPool Instance = new RWThreadPool();

        protected override IRWFactory<Socket> RWFactory
        {
            get { return RWTLockFactory<Socket>.Instance; }
        }

        public class RWTLockFactory<T> : IRWFactory<T>
        {
            public static readonly RWTLockFactory<T> Instance = new RWTLockFactory<T>();

            private RWTLockFactory() { }

            public IRWLock<T> CreateRWLock(int maxThreadCount)
            {
                return new ReadersWritersImpl<T>(maxThreadCount);
            }

            public IRWFactory<U> CloneTo<U>()
            {
                return RWTLockFactory<U>.Instance;
            }
        }
    }
}
