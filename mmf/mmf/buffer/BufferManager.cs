using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmf.context;

namespace mmf.buffer
{
    public class BufferManager
    {
        #region Members
        private ushort m_pageSize = 0;
        private ushort m_pageCount = 0;
        #endregion

        private BufferManager() { Init(); }
        public static readonly BufferManager ms_Instance = new BufferManager();

        private void Init()
        {
            m_pageSize = MMFContext.Instance.BufferSize;
            m_pageCount = MMFContext.Instance.BufferCount;
        }
    }
}
