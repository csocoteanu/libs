using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mmf.buffer
{
    /// <summary>
    /// Free index generator class.
    /// This class generates an index available for use for the next buffer allocations.
    /// It keeps track of the allocated and freed indexes, so that when requesting a new
    /// buffer allocation, it takes into account any released index and allocates a new 
    /// index if there isn`t any one available.
    /// </summary>
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

        /// <summary>
        /// Gets the next available index for the call.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Release an index and make it available for the next call.
        /// </summary>
        /// <param name="index"></param>
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
