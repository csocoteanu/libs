using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmf.context;
using mmf.exceptions;
using mmf.common;

namespace mmf.buffer
{
    /// <summary>
    /// Class allocating a huge buffer and returning subsections of that array for further use.
    /// The purpose of this class is to prevent memory fragmentation, by facilitating callers 
    /// to use sub-sections of the same array.
    /// The array division is performed in chuncks of predefined "m_pageSize" and the total
    /// number of chuncks does not exceed the "m_pageCount" value.
    /// The caller of this API _should_ release memory the buffer after using it.
    /// </summary>
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

            // allocate a huge managed memory buffer
            m_pageSize = context.BufferSize;
            m_pageCount = context.BufferCount;
            m_bufferSpace = new byte[m_pageCount * m_pageSize];
        }

        /// <summary>
        /// Returns a subsection of the pre-allocated buffer,
        /// if there is any buffer index available,
        /// otherwise return a null subsection.
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> GetBuffer()
        {
            int? freeIndex = m_generator.GetNextFreeIndex();
            if (freeIndex.HasValue)
            {
                // get the proper offset, based on page size
                int offset = freeIndex.Value * m_pageSize;
                return new ArraySegment<byte>(this.m_bufferSpace, offset, m_pageSize);
            }
            else
            {
                throw new BufferManagerOOM(string.Format(Constants.kMaxAllocations_FMT, this.m_pageCount));
            }
        }

        /// <summary>
        /// Release the buffer,
        /// by making the used index available for the next get operations
        /// </summary>
        /// <param name="buffer"></param>
        public void FreeBuffer(ref ArraySegment<byte> buffer)
        {
            ValidateBuffer(buffer);

            int? bufferIndex = buffer.Offset / m_pageSize;
            m_generator.ReleaseIndex(ref bufferIndex);

            buffer = default(ArraySegment<byte>);
        }

        private void ValidateBuffer(ArraySegment<byte> buffer)
        {
            if (buffer.Array == null)
                throw new ArgumentException(string.Format(Constants.kInvalidArg_FMT, "buffer.Array"));
            if (buffer.Count != m_pageSize)
                throw new ArgumentException(string.Format(Constants.kInvalidArg_FMT, "buffer.Count"));
            if (buffer.Offset < 0 ||
                buffer.Offset >= m_bufferSpace.Length ||
                buffer.Offset % m_pageSize != 0)
            {
                throw new ArgumentException(string.Format(Constants.kInvalidArg_FMT, "buffer.Offset"));
            }
        }

        #region IDisposable Members
        public void Dispose()
        {
            m_generator.Dispose();
        }
        #endregion
    }
}
