using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using logger.common;

namespace logger
{
    internal class Logger : AbstractLogger
    {
        public Logger(string type) : base(type) { }

        #region Members
        private string m_loggerFolder = null;
        private string m_loggerPath = null;
        private TextWriter m_writter = null;
        #endregion

        #region AbstractLogger
        protected override void OnInitLogger()
        {
            CreateLoggerPath(this.m_loggerType, ref this.m_loggerFolder, ref this.m_loggerPath);
            CreateOrOpenExisting(this.m_loggerFolder, this.m_loggerPath, ref this.m_writter);
        }

        protected override void DoCleanup()
        {
            m_writter.Close();
            m_writter.Dispose();
        }

        protected override void Writeline(string text)
        {
            m_writter.WriteLine(text);
        }
        #endregion

        #region Inner Methods
        private void CreateLoggerPath(string loggerType, ref string loggerFolder, ref string loggerPath)
        {
            string tempPath = Path.GetTempPath();
            string logName = string.Format("{0}{1}", loggerType, Constants.kLogExtension);

            loggerFolder = Path.Combine(tempPath, Constants.kLogFolder);
            loggerPath = Path.Combine(loggerFolder, logName);
        }

        private void CreateOrOpenExisting(string loggerFolder, string loggerPath, ref TextWriter writter)
        {
            if (!Directory.Exists(loggerFolder))
                Directory.CreateDirectory(loggerFolder);

            writter = (File.Exists(loggerPath)) ? File.AppendText(loggerPath) : new StreamWriter(File.Create(loggerPath));
        } 
        #endregion
    }
}
