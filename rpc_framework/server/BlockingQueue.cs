using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    public class BlockingQueue<T> : IRWLock<T>
    {
        readonly Queue<T> q = new Queue<T>();

        public void WriteTask(T item)
        {
            lock (q)
            {
                q.Enqueue(item);
                System.Threading.Monitor.Pulse(q);
            }
        }

        public T ReadNextTask()
        {
            lock (q)
            {
                for (; ; )
                {
                    if (q.Count > 0)
                    {
                        return q.Dequeue();
                    }
                    System.Threading.Monitor.Wait(q);
                }
            }
        }
    }
}
