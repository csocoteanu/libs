using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server.rwlock
{
    public class ReadersWritersImpl<T> : IRWLock<T>
    {
        private int m_readCount = 0;
        private int m_maxThreads = 0;

        private Queue<T> m_tasks = null;

        private Semaphore m_resourceAccess = null;       // controls access (read/write) to the resource
        private Semaphore m_readCountAccess = null;      // for syncing changes to shared variable readCount
        private Semaphore m_serviceQueue = null;         // FAIRNESS: preserves ordering of requests (signaling must be FIFO)

        public ReadersWritersImpl(int maxThreads)
        {
            this.m_maxThreads = maxThreads;
            this.m_tasks = new Queue<T>();

            this.m_resourceAccess = new Semaphore(1, maxThreads);
            this.m_readCountAccess = new Semaphore(1, maxThreads);
            this.m_serviceQueue = new Semaphore(1, maxThreads);
        }

        public T ReadNextTask()
        {
            T task = default(T);

            // wait in line to be serviced
            this.m_serviceQueue.WaitOne();
            this.m_readCountAccess.WaitOne();        // request exclusive access to readCount
            // <ENTER>
            if (this.m_readCount == 0)         // if there are no readers already reading:
                this.m_resourceAccess.WaitOne();     // request resource access for readers (writers blocked)
            this.m_readCount++;                // update count of active readers
            // </ENTER>
            this.m_serviceQueue.Release();           // let next in line be serviced
            this.m_readCountAccess.Release();        // release access to readCount

            // <READ>
            // reading is performed
            if (this.m_tasks.Count > 0)
            {
                task = this.m_tasks.Dequeue();
                Utils.LogInfo(task as Socket);
            }
            // </READ>

            this.m_readCountAccess.WaitOne();        // request exclusive access to readCount
            // <EXIT>
            this.m_readCount--;                // update count of active readers
            if (this.m_readCount == 0)         // if there are no readers left:
                this.m_resourceAccess.Release();     // release resource access for all
            // </EXIT>
            this.m_readCountAccess.Release();        // release access to readCount

            return task;
        }

        public void WriteTask(T task)
        {
            // wait in line to be serviced
            this.m_serviceQueue.WaitOne();
            // <ENTER>
            this.m_resourceAccess.WaitOne();         // request exclusive access to resource
            // </ENTER>
            this.m_serviceQueue.Release();           // let next in line be serviced

            // <WRITE>
            // writing is performed
            this.m_tasks.Enqueue(task);
            Utils.LogInfo(task as Socket);
            // </WRITE>

            // <EXIT>
            // release resource access for next reader/writer
            this.m_resourceAccess.Release();
            // </EXIT>
        }
    }
}
