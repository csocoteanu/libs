using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using logger.common;
using logger.config;

namespace logger
{
    /// <summary>
    /// Creates or returns a new logger based for the input class type.
    /// If no class type is provided, than the logger can be used accross
    /// the entire assembly.
    /// The log is being dumped in the %temp%\logs\"<assembly name>"
    /// </summary>
    public class LogManager
    {
        #region Members
        private Dictionary<string, AbstractLogger> m_allLoggers = null;
        private static readonly LogManager ms_Instance = new LogManager();

        private LogManager() { Init(); }
        private void Init()
        {
            m_allLoggers = new Dictionary<string, AbstractLogger>();
        }

        private string ToLookupString(Type classType)
        {
            return (classType != null)
                   ? classType.FullName
                   : System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        }

        private void OnLoggerDispose(object sender, EventArgs e)
        {
            lock (this)
            {
                AbstractLogger logger = (AbstractLogger)sender;
                string lookupType = logger.LoggerRootType;

                logger.OnDispose -= OnLoggerDispose;
                this.m_allLoggers.Remove(lookupType);
            }
        }

        private AbstractLogger CreateLogger(string loggerType)
        {
            AbstractLogger logger = LoggerFactory.CreateLogger(Configurator.Instance.LogType, loggerType);
            logger.OnDispose += OnLoggerDispose;
            return logger;
        }
        #endregion

        /// <summary>
        /// Returns a new logger based for the input class type.
        /// If no class type is provided, than the logger can be
        /// used accross the entire assembly.
        /// The log is being dumped in the %temp%\logs\"<assembly name>"
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public static ILogger GetLogger(Type classType=null)
        {
            lock (ms_Instance)
            {
                string lookupString = ms_Instance.ToLookupString(classType);
                AbstractLogger logger = null;

                if (!ms_Instance.m_allLoggers.TryGetValue(lookupString, out logger))
                {
                    logger = ms_Instance.CreateLogger(lookupString);
                    ms_Instance.m_allLoggers[lookupString] = logger;
                }

                return logger;
            }
        }

        public static void DoCleanup()
        {
            lock (ms_Instance)
            {
                foreach (var logger in ms_Instance.m_allLoggers)
                    logger.Value.Dispose();

                ms_Instance.m_allLoggers.Clear(); 
            }
        }
    }
}
