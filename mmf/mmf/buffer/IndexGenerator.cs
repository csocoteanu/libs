using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mmf.buffer
{
    internal class IndexGenerator : IDisposable
    {
        private int m_nextIndex = 0;
        private int m_maxIndexValue = 0;
        private Queue<int> m_availableIndexes = null;

        public IndexGenerator(int maxIndexValue)
        {
            m_nextIndex = 0;
            m_maxIndexValue = maxIndexValue;
            m_availableIndexes = new Queue<int>();
        }

        public int? GetNextFreeIndex()
        {
            if (m_availableIndexes.Count > 0)
            {
                return m_availableIndexes.Dequeue();
            }
            else
            {
                if (m_nextIndex == m_maxIndexValue)
                    return null;
                return m_nextIndex++;
            }
        }

        public void ReleaseIndex(int index)
        {
            m_availableIndexes.Enqueue(index);
        }

        #region IDisposable Members
        public void Dispose()
        {
            m_availableIndexes.Clear();
            m_nextIndex = 0;
        }
        #endregion
    }
}
