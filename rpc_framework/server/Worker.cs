using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    public class Worker
    {
        private Thread m_thread = null;

        public Worker()
        {
            this.m_thread = new Thread(new ThreadStart(this.DoWork));
            this.m_thread.Name = Utils.kWorkerName;
        }

        private void DoWork()
        {
            while (true)
            {
                var task = ThreadPool.Instance.GetTask();
                if (task != null)
                    Utils.DebugInfo((Socket)task);
            }
        }

        public void StartWork()
        {
            this.m_thread.Start();
        }

        public void StopWork()
        {
            this.m_thread.Abort();
        }
    }
}
