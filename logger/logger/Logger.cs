using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace logger
{
    internal class Logger : ILogger
    {
        #region members
        private TextWriter m_writter = null;
        #endregion

        public Logger()
        {            
        }

        #region ILogger Members

        public void Debug(string format, params string[] args)
        {
            throw new NotImplementedException();
        }
        public void Debug(string message)
        {
            throw new NotImplementedException();
        }

        public void Info(string format, params string[] args)
        {
            throw new NotImplementedException();
        }
        public void Info(string message)
        {
            throw new NotImplementedException();
        }

        public void Warn(string format, params string[] args)
        {
            throw new NotImplementedException();
        }
        public void Warn(string message)
        {
            throw new NotImplementedException();
        }

        public void Error(string format, params string[] args)
        {
            throw new NotImplementedException();
        }
        public void Error(string message)
        {
            throw new NotImplementedException();
        }

        public void Fatal(string format, params string[] args)
        {
            throw new NotImplementedException();
        }
        public void Fatal(string message)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Write(string message)
        {
            lock (this) { m_writter.Write(message); }
        }

        public void Write(string format, params string[] arguments)
        {
            lock (this) { m_writter.Write(format, arguments); }
        }
    }
}
