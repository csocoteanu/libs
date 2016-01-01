using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using logger.common;

namespace logger
{
    internal class LoggerFactory
    {
        public static AbstractLogger CreateLogger(eLogType logType, string rootType)
        {
            switch (logType)
            {
                case eLogType.kStdOut:
                    return new Logger(rootType);
                case eLogType.kFile:
                    return new ConsoleLogger(rootType);
            }

            return null;
        }
    }
}
