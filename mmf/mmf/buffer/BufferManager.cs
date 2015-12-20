using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmf.context;

namespace mmf.buffer
{
    public class BufferManager : IDisposable
    {
        #region Members
        private ushort m_pageSize = 0;
        private ushort m_pageCount = 0;
        private byte[] m_bufferSpace = null;

        private MMFContext m_context = null;
        private IndexGenerator m_generator = null;
        #endregion

        public BufferManager(MMFContext context) { Init(context); }
        private void Init(MMFContext context)
        {
            m_context = context;
            m_generator = new IndexGenerator(context.BufferCount);

            m_pageSize = context.BufferSize;
            m_pageCount = context.BufferCount;
            m_bufferSpace = new byte[m_pageCount * m_pageSize];
        }

        public ArraySegment<byte>? GetBuffer()
        {
            int? freeIndex = m_generator.GetNextFreeIndex();
            ArraySegment<byte>? newBuffer = null;

            if (freeIndex.HasValue)
            {
                int offset = freeIndex.Value * m_pageSize;
                newBuffer = new ArraySegment<byte>(this.m_bufferSpace, offset, m_pageSize);
            }

            return newBuffer;
        }

        public void FreeBuffer(ArraySegment<byte> buffer)
        {
            m_generator.ReleaseIndex(buffer.Offset);
        }

        #region IDisposable Members
        public void Dispose()
        {
            m_generator.Dispose();
        }
        #endregion
    }
}
