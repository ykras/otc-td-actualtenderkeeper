using System;

namespace ActualTenderKeeper.Abstract
{
    public interface ILogFactory
    {
        ILog CreateLog(Type typeAssociatedWithRequestedLog);

        ILog CreateLog<T>();
    }
}