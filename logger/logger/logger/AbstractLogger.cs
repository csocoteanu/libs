using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using logger.common;

namespace logger
{
    internal abstract class AbstractLogger : ILogger, IDisposableLogger
    {
        #region members
        protected string m_loggerType = null;
        protected eLogLevel m_logLevel = eLogLevel.kAll;
        #endregion

        public AbstractLogger(string loggerType) { Init(loggerType); }
        private void Init(string loggerType)
        {
            this.m_loggerType = loggerType;
            this.m_logLevel = config.Configurator.Instance.LogLevel;

            this.OnInitLogger();
        }

        #region Abstract Methods
        protected abstract void OnInitLogger();
        protected abstract void Writeline(string text);
        protected abstract void DoCleanup();
        #endregion

        #region ILogger Members
        public event EventHandler<EventArgs> OnDispose;

        public string LoggerRootType { get { return m_loggerType; } }
        public eLogLevel LogLevel { get { return m_logLevel; } }

        public void Debug(string format, params string[] args)
        {
            ToLogFormat(eLogLevel.kDebug, LogTime, format, args);
        }
        public void Debug(string message)
        {
            ToLogFormat(eLogLevel.kDebug, LogTime, message);
        }

        public void Info(string format, params string[] args)
        {
            ToLogFormat(eLogLevel.kInfo, LogTime, format, args);
        }
        public void Info(string message)
        {
            ToLogFormat(eLogLevel.kInfo, LogTime, message);
        }

        public void Warn(string format, params string[] args)
        {
            ToLogFormat(eLogLevel.kWarn, LogTime, format, args);
        }
        public void Warn(string message)
        {
            ToLogFormat(eLogLevel.kWarn, LogTime, message);
        }

        public void Error(string format, params string[] args)
        {
            ToLogFormat(eLogLevel.kError, LogTime, format, args);
        }
        public void Error(string message)
        {
            ToLogFormat(eLogLevel.kError, LogTime, message);
        }

        public void Fatal(string format, params string[] args)
        {
            ToLogFormat(eLogLevel.kFatal, LogTime, format, args);
        }
        public void Fatal(string message)
        {
            ToLogFormat(eLogLevel.kFatal, LogTime, message);
        }
        #endregion

        #region Inner methods
        private string LogTime
        {
            get
            {
                DateTime now = DateTime.Now;
                return string.Format("{0} {1}", now.ToShortDateString(), now.ToLongTimeString());
            }
        }

        private void ToLogFormat(eLogLevel logLevel, string logTime, string message)
        {
            string logMethod = logLevel.Stringify(this.LogLevel);
            if (!string.IsNullOrEmpty(logMethod))
                Writeline_sync(string.Format("[{0} - {1}]: {2}", logMethod, logTime, message)); 
        }
        private void ToLogFormat(eLogLevel logLevel, string logTime, string format, params string[] args)
        {
            ToLogFormat(logLevel, logTime, string.Format(format, args));
        }

        private void Writeline_sync(string text)
        {
            lock (this) { this.Writeline(text); }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            lock (this)
            {
                this.DoCleanup();

                if (this.OnDispose != null)
                    this.OnDispose(this, null);
            }
        }
        #endregion
    }
}
