using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using logger.common;

namespace logger
{
    internal class Logger : ILogger
    {
        #region members
        private string m_loggerType = null;
        private string m_loggerFolder = null;
        private string m_loggerPath = null;

        private eLogLevel m_logLevel = eLogLevel.kAll;
        private TextWriter m_writter = null;
        #endregion

        public Logger(string loggerType) { Init(loggerType); }
        private void Init(string loggerType)
        {
            this.m_loggerType = loggerType;
            this.m_logLevel = config.Configurator.Instance.LogLevel;

            CreateLoggerPath(this.m_loggerType, ref this.m_loggerFolder, ref this.m_loggerPath);
            this.m_writter = CreateOrOpenExisting(this.m_loggerFolder, this.m_loggerPath);
        }

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

        private void CreateLoggerPath(string loggerType, ref string loggerFolder, ref string loggerPath)
        {
            string tempPath = Path.GetTempPath();
            string logName = string.Format("{0}{1}", loggerType, Constants.kLogExtension);

            loggerFolder = Path.Combine(tempPath, Constants.kLogFolder);
            loggerPath = Path.Combine(loggerFolder, logName);
        }

        private TextWriter CreateOrOpenExisting(string loggerFolder, string loggerPath)
        {
            if (!Directory.Exists(loggerFolder))
                Directory.CreateDirectory(loggerFolder);
            if (!File.Exists(loggerPath))
                return new StreamWriter(File.Create(loggerPath));
            
            return File.AppendText(loggerPath);
        }

        private void ToLogFormat(eLogLevel logLevel, string logTime, string message)
        {
            string logMethod = logLevel.Stringify(this.LogLevel);
            if (!string.IsNullOrEmpty(logMethod))
                WriteLn(string.Format("[{0} - {1}]: {2}", logMethod, logTime, message)); 
        }
        private void ToLogFormat(eLogLevel logLevel, string logTime, string format, params string[] args)
        {
            ToLogFormat(logLevel, logTime, string.Format(format, args));
        }

        private void WriteLn(string text)
        {
            lock (this) { m_writter.WriteLine(text); }
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            lock (this)
            {
                m_writter.Close();
                m_writter.Dispose();

                if (this.OnDispose != null)
                    this.OnDispose(this, null);
            }
        }
        #endregion
    }
}
