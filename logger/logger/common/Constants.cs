﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logger.common
{
    internal static class Constants
    {
        internal const string kDebug = "DEBUG";
        internal const string kInfo = "INFO";
        internal const string kWarn = "WARN";
        internal const string kError = "ERROR";
        internal const string kFatal = "FATAL";
        internal const string kALL = "ALL";

        internal const string kLogFolder = "logs";
        internal const string kLogExtension = ".log";
        internal const int kMaxLogCount = 10;

        internal const int KBMultiplier = 1024;
    }
}
