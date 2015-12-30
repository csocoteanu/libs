using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logger
{
    public interface ILogger : IDisposable
    {
        event EventHandler<EventArgs> OnDispose;

        eLogLevel LogLevel { get; }
        string LoggerRootType { get; }

        void Debug(string format, params string[] args);
        void Debug(string message);
        void Info(string format, params string[] args);
        void Info(string message);
        void Warn(string format, params string[] args);
        void Warn(string message);
        void Error(string format, params string[] args);
        void Error(string message);
        void Fatal(string format, params string[] args);
        void Fatal(string message);
    }
}
