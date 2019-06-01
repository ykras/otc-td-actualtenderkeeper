using System;
using System.Threading;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;
using ActualTenderKeeper.Infrastructure.Tools;
using Infrastructure.Abstract.Logging;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule
{
    public sealed class TenderReindexSchedule : IHostedService
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly IJobBuilder _jobBuilder;
        private readonly ITriggerBuilder _triggerBuilder;
        private AsyncLazy<IScheduler> _asyncScheduler;
        private readonly ILog _log;
        
        public TenderReindexSchedule(ISchedulerProvider schedulerProvider,
            IJobBuilder jobBuilder, ITriggerBuilder triggerBuilder, ILog log)
        {
            _schedulerProvider = schedulerProvider ?? throw new ArgumentNullException(nameof(schedulerProvider));
            _jobBuilder = jobBuilder ?? throw new ArgumentNullException(nameof(jobBuilder));
            _triggerBuilder = triggerBuilder ?? throw new ArgumentNullException(nameof(triggerBuilder));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            InitAsyncScheduler();
        }

        #region IActualTenderServiceController
        
        public async Task StartAsync(CancellationToken ct = default(CancellationToken))
        {
            try
            {
                await ScheduleJob(_jobBuilder.BuildActualTenderReindexJob(),
                    _triggerBuilder.BuildActualTenderReindexTrigger()).ConfigureAwait(false);
                await StartScheduler().ConfigureAwait(false);
            }
            catch (Exception e)
            {
               _log.Error(e);
               throw;
            }
        }

        public async Task StopAsync(CancellationToken ct = default(CancellationToken))
        {
            try
            {
                await StopScheduler();
            }
            catch (Exception e)
            {
                _log.Error(e);
                throw;
            }
        }
        
        #endregion
        
        private void InitAsyncScheduler()
        {
            _asyncScheduler = new AsyncLazy<IScheduler>(() => _schedulerProvider.GetScheduler());
        }

        private async Task ScheduleJob(IJobDetail job, ITrigger trigger)
        {
            var scheduler = await _asyncScheduler;
            await scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
        }

        private async Task StartScheduler()
        {
            var scheduler = await _asyncScheduler;
            await scheduler.Start().ConfigureAwait(false);
        }

        private async Task StopScheduler()
        {
            var scheduler = await _asyncScheduler;
            await scheduler.Shutdown().ConfigureAwait(false);
            InitAsyncScheduler();
        }
    }
}