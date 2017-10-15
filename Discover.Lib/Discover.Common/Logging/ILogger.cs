using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Logging
{
    public interface ILogger
    {
        void Log(string loggerName, string message, params object[] args);

        void Log(Type type, string message, params object[] args);

        void Log(string loggerName, LoggingLevel logLevel, string message, params object[] args);

        void Log(Type type, LoggingLevel logLevel, string message, params object[] args);

        void Trace(string loggerName, string message, params object[] args);

        void Trace(Type type, string message, params object[] args);

        void Debug(string loggerName, string message, params object[] args);

        void Debug(Type type, string message, params object[] args);

        void Info(string loggerName, string message, params object[] args);

        void Info(Type type, string message, params object[] args);

        void Warn(string loggerName, string message, params object[] args);

        void Warn(Type type, string message, params object[] args);

        void Error(string loggerName, string message, params object[] args);

        void Error(Type type, string message, params object[] args);

        void Error(string loggerName, Exception ex, string message, params object[] args);

        void Error(Type type, Exception ex, string message, params object[] args);

        void Fatal(string loggerName, string message, params object[] args);

        void Fatal(Type type, string message, params object[] args);

        List<LogItem> FindLogs(DateTime fromDate, DateTime toDate, string levelFilter, string loggerFilter, string messageFilter, string userFilter, string sessionFilter);
    }
}
