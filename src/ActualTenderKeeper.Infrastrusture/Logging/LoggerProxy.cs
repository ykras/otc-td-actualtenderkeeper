using System;
using ActualTenderKeeper.Abstract;


namespace ActualTenderKeeper.Infrastructure.Logging
{
    public sealed class LoggerProxy<T> : ILog
    {
        private readonly ILog _log;
        public LoggerProxy(ILogFactory factory) => _log = factory.CreateLog<T>();

        public void Debug(Exception exception) => _log.Debug(exception);

        public void Debug(string message, params object[] args) => _log.Debug(message, args);

        public void Debug(Exception exception, string message, params object[] args) => _log.Debug(exception, message, args);

        public void Error(Exception exception) => _log.Error(exception);

        public void Error(string message, params object[] args) => _log.Error(message, args);

        public void Error(Exception exception, string message, params object[] args) => _log.Error(exception, message, args);

        public void Fatal(Exception exception) => _log.Fatal(exception);

        public void Fatal(string message, params object[] args) => _log.Fatal(message, args);

        public void Fatal(Exception exception, string message, params object[] args) => _log.Fatal(exception, message, args);

        public void Info(Exception exception) => _log.Info(exception);

        public void Info(string message, params object[] args) => _log.Info(message, args);

        public void Info(Exception exception, string message, params object[] args) => _log.Info(exception, message, args);

        public void Trace(Exception exception) => _log.Trace(exception);

        public void Trace(string message, params object[] args) => _log.Trace(message, args);

        public void Trace(Exception exception, string message, params object[] args) => _log.Trace(exception, message, args);

        public void Warn(Exception exception) => _log.Warn(exception);

        public void Warn(string message, params object[] args) => _log.Warn(message, args);

        public void Warn(Exception exception, string message, params object[] args) => _log.Warn(exception, message, args);

        public bool IsDebugEnabled => _log.IsDebugEnabled;
        public bool IsErrorEnabled => _log.IsErrorEnabled;
        public bool IsFatalEnabled => _log.IsFatalEnabled;
        public bool IsInfoEnabled => _log.IsInfoEnabled;
        public bool IsTraceEnabled => _log.IsTraceEnabled;
        public bool IsWarnEnabled => _log.IsWarnEnabled;
    }
}
