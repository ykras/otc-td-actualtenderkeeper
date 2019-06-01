using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;
using ActualTenderKeeper.Infrastructure.Configuration;
using ActualTenderKeeper.Infrastructure.Logging;
using ActualTenderKeeper.Infrastructure.Schedule;
using Infrastructure.Abstract.Logging;
using Infrastructure.Logging.NLog;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using ConfigurationBuilder = ActualTenderKeeper.Infrastructure.Configuration.ConfigurationBuilder;

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
           container.RegisterInstance(ConfigurationBuilder.BuildConfiguration());
           container.Register<IElasticsearchOptions, ElasticsearchOptions>(Lifestyle.Singleton);
           container.Register<ITenderReindexScheduleOptions, TenderReindexScheduleOptions>(Lifestyle.Singleton);
           container.RegisterInstance<IJobFactory>(
               new SimpleInjectorJobFactory(container,
                   typeof(IJobBuilder).Assembly));
           container.Register<ISchedulerProvider, SchedulerProvider>(Lifestyle.Singleton);
           container.Register<IJobBuilder, JobTriggerBuilder>(Lifestyle.Singleton);
           container.Register<ITriggerBuilder, JobTriggerBuilder>(Lifestyle.Singleton);
           container.Register<IHostedService, TenderReindexSchedule>(Lifestyle.Singleton);
        }
    }
    
    internal sealed class SimpleInjectorJobFactory : IJobFactory
    {
        private readonly Container _container;
        private readonly Dictionary<Type, InstanceProducer> _jobProducers;

        public SimpleInjectorJobFactory(
            Container container, params Assembly[] assemblies)
        { 
            _container = container ?? throw new ArgumentNullException(nameof(container));

            // By creating producers, jobs can be decorated.
            var transient = Lifestyle.Transient;
            _jobProducers =
                container.GetTypesToRegister(typeof(IJob), assemblies).ToDictionary(
                    type => type,
                    type => transient.CreateProducer(typeof(IJob), type, container));
        }
        
        #region IJobFactory
        
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler _)
        {
            try
            {
                var jobProducer = _jobProducers[bundle.JobDetail.JobType];
                return new AsyncScopedJobDecorator(
                    _container, () => (IJob)jobProducer.GetInstance());
            }
            catch (Exception e)
            {
                throw new SchedulerException($"Failed to create job {bundle.JobDetail.Key}", e);
            }
            
        }

        public void ReturnJob(IJob job)
        {
            // This will be handled automatically by Simple Injector
        }
        
        #endregion

        private sealed class AsyncScopedJobDecorator : IJob
        {
            private readonly Container _container;
            private readonly Func<IJob> _decorateeFactory;

            public AsyncScopedJobDecorator(
                Container container, Func<IJob> decorateeFactory)
            {
                _container = container ?? throw new ArgumentNullException(nameof(container));
                _decorateeFactory = decorateeFactory ?? throw new ArgumentNullException(nameof(container));
            }
            
            #region IJob
            
            public async Task Execute(IJobExecutionContext context)
            {
                using (AsyncScopedLifestyle.BeginScope(_container))
                {
                    var job = _decorateeFactory();
                    await job.Execute(context);
                }
            }
            
            #endregion
        }
    }
}
