using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;
using ActualTenderKeeper.Infrastructure.Schedule;
using Infrastructure.Abstract.Logging;
using Infrastructure.Logging;
using Microsoft.Extensions.Hosting;
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

        private static Task Main() => Run();
        
        private static async Task Run()
        {
            var windows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            using (var container = CreateDiContainer())
            {
                await (windows ? RunWinServiceUsing(container) : RunDaemonUsing(container));
            }
        }

        private static Task RunWinServiceUsing(Container container)
        {
            RegisterDependenciesWith(container);
            var task = Task.Run(() =>
            {
                var log = container.GetInstance<ILog>();
                var svc = container.GetInstance<IHostedService>();
                HostFactory.Run(hc =>
                {
                    hc.Service<IHostedService>(c =>
                    {
                        c.ConstructUsing(_ => svc);
                        c.WhenStarted(s => s.StartAsync(default));
                        c.WhenStopped(s => s.StopAsync(default));
                        c.WhenContinued(s => s.StartAsync(default));
                        c.WhenPaused(s => s.StopAsync(default));
                        c.WhenShutdown(s => s.StopAsync(default));
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
            });
            return task;
        }

        private static async Task RunDaemonUsing(Container container)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) => {
                    services.AddSimpleInjector(container, options =>
                    {
                        // Hooks hosted services into the Generic Host pipeline
                        // while resolving them through Simple Injector
                        options.AddHostedService<TenderReindexSchedule>();
                    });
                })
                .UseConsoleLifetime()
                .Build()
                .UseSimpleInjector(container);
            RegisterDependenciesWith(container);
            await builder.RunAsync();
        }

        private static Container CreateDiContainer()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
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
            container.Verify();
        }

    }
}
