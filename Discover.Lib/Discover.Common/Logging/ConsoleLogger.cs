using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string loggerName, string message, params object[] args)
        {
            log(LoggingLevel.Info, loggerName, message, args);
        }

        public void Log(Type type, string message, params object[] args)
        {
            log(LoggingLevel.Info, type.FullName, message, args);
        }

        public void Log(string loggerName, LoggingLevel logLevel, string message, params object[] args)
        {
            log(logLevel, loggerName, message, args);
        }

        public void Log(Type type, LoggingLevel logLevel, string message, params object[] args)
        {
            log(logLevel, type.FullName, message, args);
        }

        public void Trace(string loggerName, string message, params object[] args)
        {
            log(LoggingLevel.Trace, loggerName, message, args);
        }

        public void Trace(Type type, string message, params object[] args)
        {
            log(LoggingLevel.Info, type.FullName, message, args);
        }

        public void Debug(string loggerName, string message, params object[] args)
        {
            log(LoggingLevel.Debug, loggerName, message, args);
        }

        public void Debug(Type type, string message, params object[] args)
        {
            log(LoggingLevel.Debug, type.FullName, message, args);
        }

        public void Info(string loggerName, string message, params object[] args)
        {
            log(LoggingLevel.Info, loggerName, message, args);
        }

        public void Info(Type type, string message, params object[] args)
        {
            log(LoggingLevel.Info, type.FullName, message, args);
        }

        public void Warn(string loggerName, string message, params object[] args)
        {
            log(LoggingLevel.Warn, loggerName, message, args);
        }

        public void Warn(Type type, string message, params object[] args)
        {
            log(LoggingLevel.Warn, type.FullName, message, args);
        }

        public void Error(string loggerName, string message, params object[] args)
        {
            log(LoggingLevel.Error, loggerName, message, args);
        }

        public void Error(Type type, string message, params object[] args)
        {
            log(LoggingLevel.Error, type.FullName, message, args);
        }

        public void Error(string loggerName, Exception ex, string message, params object[] args)
        {
            log(LoggingLevel.Error, loggerName, string.Format(message, args) + " :: " + ex.ToString());
        }

        public void Error(Type type, Exception ex, string message, params object[] args)
        {
            log(LoggingLevel.Error, type.FullName, string.Format(message, args) + " :: " + ex.ToString());
        }

        public void Fatal(string loggerName, string message, params object[] args)
        {
            log(LoggingLevel.Fatal, loggerName, message, args);
        }

        public void Fatal(Type type, string message, params object[] args)
        {
            log(LoggingLevel.Fatal, type.FullName, message, args);
        }

        private void log(LoggingLevel level, string loggerName, string message, params object[] args)
        {
            message = string.Format(message, args);
            string l = string.Format("{0} :: {1} :: {2} :: {3}", DateTime.Now.ToString(), level.ToString(), loggerName, message);
            Console.WriteLine(string.Format(l, args));
        }


        public List<LogItem> FindLogs(DateTime fromDate, DateTime toDate, string levelFilter, string loggerFilter, string messageFilter, string userFilter, string sessionFilter)
        {
            throw new NotImplementedException();
        }
    }
}
