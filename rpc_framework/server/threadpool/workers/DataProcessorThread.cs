using server.Properties;
using server.rwlock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.threadpool.workers
{
    public class DataProcessorThread : BaseWorker<string>
    {
        public DataProcessorThread(IRWLock<string> rwLock, IRWFactory<string> rwFactory) : base(rwLock, rwFactory) { }

        protected override void DoWork()
        {
            while (true)
            {
                string data = this.m_rwLock.ReadNextTask();
                if (!string.IsNullOrEmpty(data))
                {
                    Console.WriteLine("Received: " + data);
                }
            }
        }

        protected override string WorkerName { get { return Settings.Default.kProcessor; } }
        protected override void OnStartWork() { }
        protected override void OnStopWork() { }
    }
}
