using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using mmf.context;

namespace mmf.buffer
{
    public unsafe class UnmanagedBufferManager : IDisposable
    {
        #region Members
        private ushort m_pageSize = 0;
        private ushort m_pageCount = 0;
        private byte* m_PTRbaseMem = null;

        private MMFContext m_context;
        private IndexGenerator m_generator = null;
        #endregion

        public UnmanagedBufferManager(MMFContext context) { Init(context); }
        private void Init(MMFContext context)
        {
            m_pageSize = context.BufferSize;
            m_pageCount = context.BufferCount;
            m_PTRbaseMem = (byte*)Marshal.AllocHGlobal(m_pageCount * m_pageSize).ToPointer();

            m_context = context;
            m_generator = new IndexGenerator(m_pageCount);
        }

        public byte* AllocSpace()
        {
            int? newPageIndex = m_generator.GetNextFreeIndex();
            return (newPageIndex.HasValue) 
                    ? m_PTRbaseMem + newPageIndex.Value * m_pageSize
                    : null;
        }

        public void Release(byte * memory)
        {
            long pageIndex = memory - m_PTRbaseMem;
            m_generator.ReleaseIndex((int)(pageIndex / m_pageSize));
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)m_PTRbaseMem);
            m_generator.Dispose();
        }
    }
}
