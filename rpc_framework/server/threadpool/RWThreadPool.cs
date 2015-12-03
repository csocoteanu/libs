using server.rwlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.threadpool
{
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
