using System;
using System.Threading;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace ActualTenderKeeper.Infrastructure.Schedule
{
    public sealed class SchedulerProvider : ISchedulerProvider
    {
        #region Fields
        private readonly IJobFactory _jobFactory;
        #endregion

        #region Constructors
        public SchedulerProvider(IJobFactory jobFactory)
        {
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
        }
        #endregion

        #region ISchedulerProvider
        
        public async Task<IScheduler> GetScheduler(CancellationToken ct = default(CancellationToken))
        {
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler(ct);
            scheduler.JobFactory = _jobFactory;
            return scheduler;
        }
        
        #endregion
    }
}