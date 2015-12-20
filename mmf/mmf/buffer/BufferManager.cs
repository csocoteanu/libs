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
        private MMFContext m_context = null;
        #endregion

        public BufferManager(MMFContext context) { Init(context); }
        private void Init(MMFContext context)
        {
            m_context = context;
            m_pageSize = context.BufferSize;
            m_pageCount = context.BufferCount;
        }

        #region IDisposable Members
        public void Dispose()
        {
        }
        #endregion
    }
}
