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
    public class ThreadPool : IDisposable
    {
        public static readonly ThreadPool Instance = new ThreadPool();

        private int m_readCount = 1;
        private Semaphore m_resourceAccess;       // controls access (read/write) to the resource
        private Semaphore m_readCountAccess;      // for syncing changes to shared variable readCount
        private Semaphore m_serviceQueue;         // FAIRNESS: preserves ordering of requests (signaling must be FIFO)

        private ushort m_threadPerCore = 0;
        private int m_threadCount = 0;

        private Thread[] m_workerThreads = null;
        private Queue m_tasks = null;

        private ThreadPool()
        {
            this.m_threadPerCore = server.Properties.Settings.Default.kThreadsPerCore;
            this.m_threadCount = Environment.ProcessorCount * this.m_threadPerCore;

            this.m_tasks = new Queue();

            this.m_workerThreads = new Thread[this.m_threadCount];
            for (int i = 0; i < this.m_workerThreads.Length; i++)
            {
                this.m_workerThreads[i] = new Thread(new ThreadStart(this.DoWork));
                this.m_workerThreads[i].Start();
            }

            // all semaphores initialised to 1
            this.m_resourceAccess = new Semaphore(1, m_workerThreads.Length);
            this.m_readCountAccess = new Semaphore(1, m_workerThreads.Length);
            this.m_serviceQueue = new Semaphore(1, m_workerThreads.Length);
        }

        private void DoWork()
        {
            while (true)
            {
                var task = this.GetTask();
                if (task != null)
                    this.DebugInfo((Socket)task);
            }

        }

        public void AddTask(object task)
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
            this.DebugInfo((Socket)task);
            // </WRITE>

            // <EXIT>
            // release resource access for next reader/writer
            this.m_resourceAccess.Release();
            // </EXIT>
        }

        public object GetTask()
        {
            object task = null;

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
                task = this.m_tasks.Dequeue();
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

        public void Dispose()
        {
            for (int i = 0; i < this.m_workerThreads.Length; i++)
            {
                this.m_workerThreads[i].Join();
            }
        }

        public void DebugInfo(Socket sock)
        {
            IPEndPoint ipRemote = (IPEndPoint)sock.RemoteEndPoint;
            IPEndPoint ipLocal = (IPEndPoint)sock.LocalEndPoint;
            int tid = Thread.CurrentThread.ManagedThreadId;

            Console.WriteLine(string.Format("[Thread: {0}] Local: {1}: {2} Remote: {3}: {4}", tid, ipLocal.Address.ToString(), ipLocal.Port, ipRemote.Address.ToString(), ipRemote.Port)); 
        }
    }
}
