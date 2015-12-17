using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mmf.context
{
    public class MMFContext
    {
        public ushort BufferSize { get; private set; }
        public ushort BufferCount { get; private set; }
        public ushort ObjectPoolCount { get; private set; }

        private MMFContext() { Init(); }
        public readonly static MMFContext Instance = new MMFContext();

        private void Init()
        {
        }
    }
}
