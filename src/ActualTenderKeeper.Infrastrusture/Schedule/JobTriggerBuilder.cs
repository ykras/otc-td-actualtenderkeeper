using System;
using ActualTenderKeeper.Abstract;
using ActualTenderKeeper.Infrastructure.Schedule.Jobs;
using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule
{
    public sealed class JobTriggerBuilder : IJobBuilder, ITriggerBuilder
    {
        private readonly ITenderReindexScheduleOptions _scheduleOptions;
        
        public JobTriggerBuilder(ITenderReindexScheduleOptions scheduleOptions)
        {
            _scheduleOptions = scheduleOptions ?? throw new ArgumentNullException(nameof(scheduleOptions));
        }
        
        #region IJobBuilder
        
        public IJobDetail BuildActualTenderReindexJob()
        {
            var jobDetail = JobBuilder.Create<TenderArchiveJob>()
                .WithIdentity(nameof(TenderArchiveJob))
                .Build();
            return jobDetail;
        }
        
        #endregion
        
        #region ITriggerBuilder
        
        public ITrigger BuildActualTenderReindexTrigger()
        {
            var builder = TriggerBuilder.Create()
                .WithIdentity("ActualTenderReindexTrigger")
                .WithCronSchedule(_scheduleOptions.CronExpression);
                return builder.Build();
        }
        
        #endregion
    }
}