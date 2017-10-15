using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Discover.Logging.NLog.NLogDataTableAdapters;

namespace Discover.Logging.NLog
{
    public class NLogger : ILogger
    {
        public void Log(string loggerName, string message, params object[] args)
        {
            log(loggerName, LoggingLevel.Info, message, args);
        }

        public void Log(Type type, string message, params object[] args)
        {
            log(type, LoggingLevel.Info, message, args);
        }

        public void Log(string loggerName, LoggingLevel logLevel, string message, params object[] args)
        {
            log(loggerName, logLevel, message, args);
        }

        public void Log(Type type, LoggingLevel logLevel, string message, params object[] args)
        {
            log(type, logLevel, message, args);
        }

        public void Trace(string loggerName, string message, params object[] args)
        {
            log(loggerName, LoggingLevel.Trace, message, args);
        }

        public void Trace(Type type, string message, params object[] args)
        {
            log(type, LoggingLevel.Trace, message, args);
        }

        public void Debug(string loggerName, string message, params object[] args)
        {
            log(loggerName, LoggingLevel.Debug, message, args);
        }

        public void Debug(Type type, string message, params object[] args)
        {
            log(type, LoggingLevel.Debug, message, args);
        }

        public void Info(string loggerName, string message, params object[] args)
        {
            log(loggerName, LoggingLevel.Info, message, args);
        }

        public void Info(Type type, string message, params object[] args)
        {
            log(type, LoggingLevel.Info, message, args);
        }

        public void Warn(string loggerName, string message, params object[] args)
        {
            log(loggerName, LoggingLevel.Warn, message, args);
        }

        public void Warn(Type type, string message, params object[] args)
        {
            log(type, LoggingLevel.Warn, message, args);
        }

        public void Error(string loggerName, string message, params object[] args)
        {
            log(loggerName, LoggingLevel.Error, message, args);
        }

        public void Error(Type type, string message, params object[] args)
        {
            log(type, LoggingLevel.Error, message, args);
        }

        public void Error(string loggerName, Exception ex, string message, params object[] args)
        {
            log(ex, loggerName, LoggingLevel.Error, message, args);
        }

        public void Error(Type type, Exception ex, string message, params object[] args)
        {
            log(ex, type, LoggingLevel.Error, message, args);
        }

        public void Fatal(string loggerName, string message, params object[] args)
        {
            log(loggerName, LoggingLevel.Fatal, message, args);
        }

        public void Fatal(Type type, string message, params object[] args)
        {
            log(type, LoggingLevel.Fatal, message, args);
        }

        public List<LogItem> FindLogs(DateTime fromDate, DateTime toDate, string levelFilter, string loggerFilter, string messageFilter, string userFilter, string sessionFilter)
        {
            List<LogItem> logs = new List<LogItem>();
            LogTableAdapter adapter = new LogTableAdapter();
            NLogData.LogDataTable table = adapter.FindLogs(fromDate, toDate, levelFilter, loggerFilter, messageFilter, userFilter, sessionFilter);
            if (table.Count > 0)
            {
                foreach (NLogData.LogRow row in table)
                {
                    logs.Add(new LogItem() { Id = row.Id, 
                        Logger = row.Logger, 
                        Level = row.Level, 
                        Message = row.Message, 
                        SessionId = row.SessionId, 
                        TimeStamp = row.TimeStamp, 
                        User = row.User });
                }
            }
            return logs;
        }

        private void log(Exception ex, string loggerName, LoggingLevel logLevel, string message, params object[] args)
        {
            message = string.Format(message, args) + " :: " + ex.ToString();
            Logger logger = LogManager.GetLogger(loggerName);
            logger.Log(ToLogLevel(logLevel), message);
        }

        private void log(Exception ex, Type logType, LoggingLevel logLevel, string message, params object[] args)
        {
            message = string.Format(message, args) + " :: " + ex.ToString();
            Logger logger = LogManager.GetLogger(logType.FullName);
            logger.Log(ToLogLevel(logLevel), message);
        }

        private void log(string loggerName, LoggingLevel logLevel, string message, params object[] args)
        {
            Logger logger = LogManager.GetLogger(loggerName);
            logger.Log(ToLogLevel(logLevel), message, args);
        }

        private void log(Type logType, LoggingLevel logLevel, string message, params object[] args)
        {
            Logger logger = LogManager.GetLogger(logType.FullName);
            logger.Log(ToLogLevel(logLevel), message, args);
           
        }

        private LogLevel ToLogLevel(LoggingLevel level)
        {
            switch (level)
            {
                case LoggingLevel.Debug:
                    return LogLevel.Debug;
                case LoggingLevel.Error:
                    return LogLevel.Error;
                case LoggingLevel.Fatal:
                    return LogLevel.Fatal;
                case LoggingLevel.Info:
                    return LogLevel.Info;
                case LoggingLevel.Trace:
                    return LogLevel.Trace;
                case LoggingLevel.Warn:
                    return LogLevel.Warn;
                default:
                    return LogLevel.Info;
            }
        }

        public void InitDatabaseSchema()
        {
            var commands = new string[0];

            using (var script = new System.IO.StreamReader(this.GetType().Assembly.GetManifestResourceStream(this.GetType().Assembly.GetName().Name + ".NLogger.sql")))
            {
                commands = script.ReadToEnd().Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
            }

            using (var adapter = new LogTableAdapter())
            {
                adapter.Connection.Open();

                var cmd = adapter.Connection.CreateCommand();

                foreach (var command in commands)
                {
                    cmd.CommandText = command;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
