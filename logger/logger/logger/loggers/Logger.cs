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
        private long m_logSize = 0;
        private int m_maxLogSize = 0;
        private StreamWriter m_writter = null;
        #endregion

        #region AbstractLogger
        protected override void OnInitLogger()
        {
            CreateLoggerPath(this.m_loggerType, ref this.m_loggerFolder, ref this.m_loggerPath);
            CreateOrOpenExisting(this.m_loggerFolder, this.m_loggerPath, ref this.m_writter);

            m_maxLogSize = config.Configurator.Instance.LogSizeKB;
            m_logSize = m_writter.BaseStream.Position;
        }

        protected override void DoCleanup()
        {
            m_writter.Close();
            m_writter.Dispose();
        }

        protected override void Writeline(string text)
        {
            CheckLogRotate();
            m_writter.WriteLine(text);

            m_logSize += text.Length;
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

        private void CreateOrOpenExisting(string loggerFolder, string loggerPath, ref StreamWriter writter)
        {
            if (!Directory.Exists(loggerFolder))
                Directory.CreateDirectory(loggerFolder);

            writter = (File.Exists(loggerPath)) ? File.AppendText(loggerPath) : new StreamWriter(File.Create(loggerPath));
        } 
        #endregion

        #region Log Rotate
        private void CheckLogRotate()
        {
            // if exceeded preconfigured file size
            if (m_logSize >= m_maxLogSize)
            {
                // close existing file
                m_writter.Close();
                // rename the file to the next rotated log 
                File.Move(m_loggerPath, GetLogRotatePath());
                // open a new handle to the new log
                m_writter = new StreamWriter(File.Create(m_loggerPath));
            }
        }

        private string GetLogRotatePath()
        {
            int lastFileIndex = 0;
            string pattern = string.Format("{0}{1}{2}", m_loggerType, "*", Constants.kLogExtension);
            string[] allLogFiles = Directory.GetFiles(m_loggerFolder, pattern);

            // iterate through all log created files for the logger type
            foreach (var fileName in allLogFiles)
            {
                // because we are given the entire file path
                int start = Path.Combine(m_loggerFolder, m_loggerType).Length;
                int end = fileName.IndexOf(Constants.kLogExtension);
                int count = end - start;

                if (count > 0)
                {
                    // based on the start and end indexes
                    // compute the biggest log index, if possible
                    int number = 0;
                    string fileNumber = fileName.Substring(start, count);
                    if (int.TryParse(fileNumber, out number) && number > lastFileIndex)
                        lastFileIndex = number;
                }
            }

            // after computing the new index (in a round robin manner)
            // and assume the files have the maximum size
            // return the proper file name
            lastFileIndex = (lastFileIndex % Constants.kMaxLogCount) + 1;
            string newFileName = string.Format("{0}{1}{2}", m_loggerType, lastFileIndex, Constants.kLogExtension);
            return Path.Combine(m_loggerFolder, newFileName);
        }
        #endregion
    }
}
