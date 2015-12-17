using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mmf.Properties;

namespace mmf.context
{
    public class MMFContext
    {
        #region Properties
        public ushort BufferSize { get; private set; }
        public ushort BufferCount { get; private set; }
        public ushort ObjectPoolCount { get; private set; }

        public bool UseDefaultAllocation { get; set; }
        #endregion

        private MMFContext() { Init(); }
        public readonly static MMFContext Instance = new MMFContext();

        private void Init()
        {
            this.BufferSize = Settings.Default.kBufferSize;
            this.BufferCount = Settings.Default.kBufferCount;
            this.ObjectPoolCount = Settings.Default.kObjectPoolCount;

            this.UseDefaultAllocation = Settings.Default.kUseDefaultAllocation;
        }
    }
}
