using server.Properties;
using server.rwlock;
using server.threadpool.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.threadpool.workers
{
    public class SocketReceiverThread : BaseWorker<Socket>
    {
        protected override string WorkerName { get { return Settings.Default.kReceiver; } }

        private DataProcessorThread m_processor = null;
        private List<SocketData> m_allConnections = null;

        public SocketReceiverThread(IRWLock<Socket> rwLock, IRWFactory<Socket> rwFactory)
            : base(rwLock, rwFactory)
        {
            IRWFactory<SocketData> dataRWFactory = rwFactory.CloneTo<SocketData>();
            IRWLock<SocketData> dataRWLock = dataRWFactory.CreateRWLock(2);

            m_allConnections = new List<SocketData>();
            m_processor = new DataProcessorThread(dataRWLock, null);
        }

        protected override void DoWork()
        {
            while (true)
            {
                Socket newConnection = this.m_rwLock.ReadNextTask();
                if (newConnection != null)
                {
                    Utils.LogInfo(newConnection, "New connection!");
                    SocketData sockData = new SocketData(newConnection);
                    m_allConnections.Add(sockData);
                }

                for (int i = 0; i < m_allConnections.Count; )
                {
                    SocketData sockData = m_allConnections[i];
                    SocketData.eSocketStatus sockStatus = (sockData.IsConnected) ? sockData.ReceiveData() : SocketData.eSocketStatus.kUnconnected;

                    switch (sockStatus)
                    {
                        case SocketData.eSocketStatus.kSuccesfull:
                            m_processor.WriteTask(sockData);
                            i++;
                            break;
                        case SocketData.eSocketStatus.kNotReady:
                            i++;
                            break;
                        case SocketData.eSocketStatus.kError:
                        case SocketData.eSocketStatus.kUnconnected:
                            m_allConnections.Remove(sockData);
                            Utils.LogInfo(sockData.Connection, "Closing connection...");
                            break;
                    }
                }
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
