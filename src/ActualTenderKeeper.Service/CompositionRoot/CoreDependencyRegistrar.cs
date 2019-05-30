using ActualTenderKeeper.Abstract;
using ActualTenderKeeper.Core;
using SimpleInjector;

namespace ActualTenderKeeper.Service.CompositionRoot
{
    public static class CoreDependencyRegistrar
    {
        public static void RegisterDependenciesWith(Container container)
        {
            container.Register<IActualTenderService, ActualTenderService>(Lifestyle.Singleton);
        }
    }
}