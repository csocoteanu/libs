using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using server.Properties;
using System.Runtime.InteropServices;

namespace server.memory
{
    public unsafe class UnmanagedBufferManager : IDisposable
    {
        #region Members
        private ushort m_pageSize = 0;
        private ushort m_pageCount = 0;
        private byte* m_PTRbaseMem = null;

        private ushort m_newIndex = 0;
        private Dictionary<ushort, bool> m_mappedPages = null;

        private UnmanagedBufferManager() { Init(); }
        public static readonly UnmanagedBufferManager ms_Instance = new UnmanagedBufferManager(); 
        #endregion

        private void Init()
        {
            m_pageSize = Settings.Default.kBufferSize;
            m_pageCount = Settings.Default.kBufferCount;
            m_PTRbaseMem = (byte *)Marshal.AllocHGlobal(m_pageCount * m_pageSize).ToPointer();

            m_newIndex = 0;
            m_mappedPages = new Dictionary<ushort, bool>();
        }

        public byte* AllocSpace()
        {
            byte* allocAddr = null;
            ushort newPageIndex = m_newIndex;
            bool hasUnallocatedPage = false;

            // Find first available mapped page
            // if possible
            foreach (var item in m_mappedPages)
            {
                ushort pageIndex = item.Key;
                bool pageIsMapped = item.Value;

                if (!pageIsMapped)
                {
                    newPageIndex = pageIndex;
                    hasUnallocatedPage = true;
                    break;
                }
            }

            // "allocate" a new page
            if (!hasUnallocatedPage)
            {
                if (m_newIndex < m_pageCount)
                {
                    m_newIndex++;
                }
                else
                {
                    // we might run out of allocated space
                    // so ensure that all other allocations will fail
                    return null;
                }
            }

            m_mappedPages[newPageIndex] = true;
            allocAddr = m_PTRbaseMem + newPageIndex;

            return allocAddr;
        }

        public void Release(byte * memory)
        {
            ushort pageIndex = (ushort)(memory - m_PTRbaseMem);

            // unmap page
            if (pageIndex >= 0)
                m_mappedPages[pageIndex] = false;
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)m_PTRbaseMem);
        }
    }
}
