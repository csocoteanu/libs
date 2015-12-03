using server.rwlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.threadpool
{
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
}
