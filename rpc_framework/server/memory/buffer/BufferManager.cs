using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using server.Properties;

namespace server.memory
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
            m_pageSize = Settings.Default.kBufferSize;
            m_pageCount = Settings.Default.kBufferCount;
        }


    }
}
