using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using logger.Properties;
using logger.common;

namespace logger.config
{
    public class Configurator
    {
        public static readonly Configurator Instance = new Configurator();
        public eLogLevel LogLevel { get; set; }
        public eLogType LogType { get; set; }
        public int LogSizeKB { get; set; }

        private Configurator() { Init(); }
        private void Init()
        {
            this.LogLevel = Settings.Default.kLogLevel.ToLogLevel();
            this.LogSizeKB = Settings.Default.kLogSize.KBMultiply();
            this.LogType = eLogType.kFile;
        }
    }
}
