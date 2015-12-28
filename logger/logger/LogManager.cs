using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logger
{
    public class LogManager
    {
        public static ILogger GetLogger(Type classType=null)
        {
            return new Logger();
        }
    }
}
