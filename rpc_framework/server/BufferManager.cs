using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace server
{
    public class BufferManager
    {
        private BufferManager() { Init(); }
        public static readonly BufferManager ms_Instance = new BufferManager();

        private void Init()
        {
            
        }

        public void AllocSpace()
        {
        }

        public void Release()
        {
        }
    }
}
