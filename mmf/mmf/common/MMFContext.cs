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
        public ushort BufferSize { get; set; }
        public ushort BufferCount { get; set; }
        public ushort ObjectPoolCount { get; set; }

        public bool UseDefaultAllocation { get; set; }
        #endregion

        public MMFContext() { Init(); }
        private void Init()
        {
            this.BufferSize = Settings.Default.kBufferSize;
            this.BufferCount = Settings.Default.kBufferCount;
            this.ObjectPoolCount = Settings.Default.kObjectPoolCount;

            this.UseDefaultAllocation = Settings.Default.kUseDefaultAllocation;
        }
    }
}
