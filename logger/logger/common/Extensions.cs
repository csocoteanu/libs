using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logger.common
{
    public static class Extensions
    {
        public static string Stringify(this eLogLevel thLogLevel, eLogLevel maxLogLevel)
        {
            if (maxLogLevel < thLogLevel)
                return null;

            switch (thLogLevel)
            {
                case eLogLevel.kDebug:
                    return Constants.kDebug;
                case eLogLevel.kInfo:
                    return Constants.kInfo;
                case eLogLevel.kWarn:
                    return Constants.kWarn;
                case eLogLevel.kError:
                    return Constants.kError;
                case eLogLevel.kFatal:
                    return Constants.kFatal;
            }

            throw new InvalidOperationException(string.Format("Invalid log level provided: {0}", thLogLevel.ToString()));
        }

        public static eLogLevel ToLogLevel(this string thLogLevelString)
        {
            switch (thLogLevelString)
            {
                case Constants.kDebug:
                    return eLogLevel.kDebug;
                case Constants.kInfo:
                    return eLogLevel.kInfo;
                case Constants.kWarn:
                    return eLogLevel.kWarn;
                case Constants.kError:
                    return eLogLevel.kError;
                case Constants.kFatal:
                    return eLogLevel.kFatal;
            }

            throw new InvalidOperationException(string.Format("Invalid log level provided: {0}", thLogLevelString));
        }
    }
}
