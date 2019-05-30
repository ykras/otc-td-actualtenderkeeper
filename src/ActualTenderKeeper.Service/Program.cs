using System;
using System.Linq;
using System.Reflection;
using ActualTenderKeeper.Abstract;
using Infrastructure.Abstract.Logging;
using Infrastructure.Logging;
using NLog;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Topshelf;

namespace ActualTenderKeeper.Service
{
    internal static class Program
    {
        private const string ServiceName = "Traderegistry.Elastic.ActualTenderKeeper";
        private const string ServiceDisplayName = "Traderegistry.Elastic.ActualTenderKeeper";
        private const string ServiceDescription = "Keeper of actual tenders index in Elasticsearch";
        private const int DelayBeforeRestartServiceInMinutes = 1;
        private const int DelayBeforeResetFailCountInDays = 1;
        
        private static void Main()
        {
            RunService();
        }

        private static void RunService()
        {
            var log = NullLog.Instance;
            try
            {
                using (var container = CreateDiContainer())
                {
                    log = container.GetInstance<ILog>();
                    var svc = container.GetInstance<IActualTenderKeeperController>();
                    HostFactory.Run(hc =>
                    {
                        hc.Service<IActualTenderKeeperController>(c =>
                        {
                            c.ConstructUsing(_ => svc);
                            c.WhenStarted(s => s.StartAsync());
                            c.WhenStopped(s => s.StopAsync());
                            c.WhenContinued(s => s.StartAsync());
                            c.WhenPaused(s => s.StopAsync());
                            c.WhenShutdown(s => s.StopAsync());
                        });
                        hc.EnableShutdown();
                        hc.EnablePauseAndContinue();
                        hc.EnableServiceRecovery(c =>
                        {
                            c.OnCrashOnly();
                            c.RestartService(DelayBeforeRestartServiceInMinutes);
                            c.SetResetPeriod(DelayBeforeResetFailCountInDays);
                        });
                        hc.OnException(e => log.Error(e));
                        hc.SetServiceName(ServiceName);
                        hc.SetDisplayName(ServiceDisplayName);
                        hc.SetDescription(ServiceDescription);
                        hc.RunAsLocalSystem();
                        hc.StartAutomatically();
                    });
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex,"Failed to start service");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static Container CreateDiContainer()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            RegisterDependenciesWith(container);
            container.Verify();
            return container;
        }

        private static void RegisterDependenciesWith(Container container)
        {
            var registrars =
                from t in Assembly.GetExecutingAssembly().GetExportedTypes()
                let m = t.GetMethod(nameof(RegisterDependenciesWith),
                    BindingFlags.Public | BindingFlags.Static, null,
                    new[] { typeof(Container) }, null)
                where t.Namespace.Contains("CompositionRoot") && m != null
                select m;
            foreach (var registrar in registrars)
            {
                registrar.Invoke(null, new object[] { container });
            }
        }

    }
}
