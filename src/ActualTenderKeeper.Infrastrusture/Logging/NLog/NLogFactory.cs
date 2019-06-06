using System;
using ActualTenderKeeper.Abstract;
using NLog;

namespace ActualTenderKeeper.Infrastructure.Logging.NLog
{
    public sealed class NLogFactory : ILogFactory
    {
        public ILog CreateLog(Type typeAssociatedWithRequestedLog)
        {
            if (typeAssociatedWithRequestedLog == null)
            {
                throw new ArgumentNullException(nameof(typeAssociatedWithRequestedLog));
            }
            return LogFor(typeAssociatedWithRequestedLog);
        }

        public ILog CreateLog<T>()
        {
            return LogFor(typeof(T));
        }

        private static ILog LogFor(Type type)
        {
            var log = LogManager.GetLogger(type.FullName);
            return new NLogAdapter(log);
        }
    }
}