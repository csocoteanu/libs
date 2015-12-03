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
    public class DispatcherThread : BaseWorker<Socket>
    {
        protected override string WorkerName { get { return Settings.Default.kDispatcher; } }

        private SocketReceiverThread m_receiver = null;

        public DispatcherThread(IRWLock<Socket> rwLock, IRWFactory<Socket> rwFactory)
            : base(rwLock, rwFactory)
        {
            m_receiver = new SocketReceiverThread(rwFactory.CreateRWLock(2), rwFactory);
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
