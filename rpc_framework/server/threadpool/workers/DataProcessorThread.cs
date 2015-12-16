using server.Properties;
using server.rwlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using server.io.data;

namespace server.threadpool.workers
{
    public class DataProcessorThread : BaseWorker<SocketData>
    {
        public DataProcessorThread(IRWLock<SocketData> rwLock, IRWFactory<SocketData> rwFactory) : base(rwLock, rwFactory) { }

        protected override void DoWork()
        {
            for (SocketData sockData = this.m_rwLock.ReadNextTask(); sockData != null; sockData = this.m_rwLock.ReadNextTask())
            {
                string data = sockData.Data.ToString();
                sockData.ClearData();

                Console.WriteLine("Received: " + data);
                // echo data back
                sockData.SendData(Encoding.ASCII.GetBytes(data));
            }
        }

        protected override string WorkerName { get { return Settings.Default.kProcessor; } }
        protected override void OnStartWork() { }
        protected override void OnStopWork() { }
    }
}
