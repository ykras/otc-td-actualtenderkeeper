using System;
using ActualTenderKeeper.Abstract;
using NLog;

namespace ActualTenderKeeper.Infrastructure.Logging.NLog
{
    internal sealed class NLogAdapter : ILog
    {
        private readonly ILogger _logger;

        public NLogAdapter(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool IsDebugEnabled => _logger.IsDebugEnabled;

        public bool IsErrorEnabled => _logger.IsErrorEnabled;

        public bool IsFatalEnabled => _logger.IsFatalEnabled;

        public bool IsInfoEnabled => _logger.IsInfoEnabled;

        public bool IsTraceEnabled => _logger.IsTraceEnabled;

        public bool IsWarnEnabled => _logger.IsWarnEnabled;

        public void Debug(Exception exception)
        {
            Debug(exception, String.Empty);
        }

        public void Debug(string message, params object[] args)
        {
            Debug(null, message, args);
        }

        public void Debug(Exception exception, string message, params object[] args)
        {
            if (!IsDebugEnabled) return;
            var logEvent = GetLogEvent(LogLevel.Debug, exception, message, args);
            _logger.Log(GetType(), logEvent);
        }

        public void Error(Exception exception)
        {
            Error(exception, String.Empty);
        }

        public void Error(string message, params object[] args)
        {
            Error(null, message, args);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            if (!IsErrorEnabled) return;
            var logEvent = GetLogEvent(LogLevel.Error, exception, message, args);
            _logger.Log(GetType(), logEvent);
        }

        public void Fatal(Exception exception)
        {
            Fatal(exception, String.Empty);
        }

        public void Fatal(string message, params object[] args)
        {
            Fatal(null, message, args);
        }

        public void Fatal(Exception exception, string message, params object[] args)
        {
            if (!IsFatalEnabled) return;
            var logEvent = GetLogEvent(LogLevel.Fatal, exception, message, args);
            _logger.Log(GetType(), logEvent);
        }

        public void Info(Exception exception)
        {
            Info(exception, String.Empty);
        }

        public void Info(string message, params object[] args)
        {
            Info(null, message, args);
        }

        public void Info(Exception exception, string message, params object[] args)
        {
            if (!IsInfoEnabled) return;
            var logEvent = GetLogEvent(LogLevel.Info, exception, message, args);
            _logger.Log(GetType(), logEvent);
        }

        public void Trace(Exception exception)
        {
            Trace(exception, String.Empty);
        }

        public void Trace(string message, params object[] args)
        {
            Trace(null, message, args);
        }

        public void Trace(Exception exception, string message, params object[] args)
        {
            if (!IsTraceEnabled) return;
            var logEvent = GetLogEvent(LogLevel.Trace, exception, message, args);
            _logger.Log(GetType(), logEvent);
        }

        public void Warn(Exception exception)
        {
            Warn(exception, String.Empty);
        }

        public void Warn(string message, params object[] args)
        {
            Warn(null, message, args);
        }

        public void Warn(Exception exception, string message, params object[] args)
        {
            if (!IsWarnEnabled) return;
            var logEvent = GetLogEvent(LogLevel.Warn, exception, message, args);
            _logger.Log(GetType(), logEvent);
        }

        private LogEventInfo GetLogEvent(LogLevel level, Exception exception, string message, object[] args)
        {
            var assemblyProp = string.Empty;
            var classProp = string.Empty;
            var methodProp = string.Empty;
            var messageProp = string.Empty;
            var innerMessageProp = string.Empty;
            var logEvent = new LogEventInfo(level, _logger.Name, null, message, args);
            if (exception != null)
            {
                logEvent.Exception = exception;
                assemblyProp = exception.Source;
                if (exception.TargetSite != null)
                {
                    methodProp = exception.TargetSite.Name;
                    if (exception.TargetSite.DeclaringType != null)
                    {
                        classProp = exception.TargetSite.DeclaringType.FullName;
                    }
                }
                messageProp = exception.Message;

                if (exception.InnerException != null)
                {
                    innerMessageProp = exception.InnerException.Message;
                }
            }
            logEvent.Properties["error-source"] = assemblyProp;
            logEvent.Properties["error-class"] = classProp;
            logEvent.Properties["error-method"] = methodProp;
            logEvent.Properties["error-message"] = messageProp;
            logEvent.Properties["inner-error-message"] = innerMessageProp;
            return logEvent;
        }
    }
}