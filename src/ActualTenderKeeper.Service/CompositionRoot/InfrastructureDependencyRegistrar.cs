using System;
using System.Collections.Generic;
using System.Text;
using ActualTenderKeeper.Infrastructure.Logging;
using Infrastructure.Abstract.Logging;
using Infrastructure.Logging.NLog;
using SimpleInjector;

namespace ActualTenderKeeper.Service.CompositionRoot
{
    public static class InfrastructureDependencyRegistrar
    {
        public static void RegisterDependenciesWith(Container container)
        {
           container.Register<ILogFactory, NLogFactory>(Lifestyle.Singleton);
           container.RegisterConditional(
               typeof(ILog),
               c => typeof(LoggerProxy<>).MakeGenericType(c.Consumer.ImplementationType),
               Lifestyle.Singleton,
               c => true);
        }
    }
}
