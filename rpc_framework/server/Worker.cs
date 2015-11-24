using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    public abstract class BaseWorker
    {
        protected Thread m_thread = null;

        protected abstract void DoWork();
        protected abstract void OnStartWork();
        protected abstract void OnStopWork();

        public BaseWorker()
        {
            this.m_thread = new Thread(new ThreadStart(this.DoWork));
            this.m_thread.Name = Utils.kWorkerName;
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
    }

    public class Processor : BaseWorker
    {
        protected override void DoWork() { }
        protected override void OnStartWork() { }
        protected override void OnStopWork() { }
    }

    public class Worker : BaseWorker
    {
        private Processor m_processor = null;

        public Worker() : base()
        {
            m_processor = new Processor();
        }

        protected override void DoWork()
        {
            while (true)
            {
                var task = ThreadPool.Instance.GetTask();
                if (task != null)
                    Utils.DebugInfo((Socket)task);
            }
        }

        protected override void OnStartWork()
        {
            this.m_processor.StartWork();
        }

        protected override void OnStopWork()
        {
            this.m_processor.StopWork();
        }
    }
}
