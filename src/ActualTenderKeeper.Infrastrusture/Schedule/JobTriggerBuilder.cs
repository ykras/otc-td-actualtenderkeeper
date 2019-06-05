using System;
using ActualTenderKeeper.Abstract;
using ActualTenderKeeper.Infrastructure.Schedule.Jobs;
using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule
{
    public sealed class JobTriggerBuilder : IJobBuilder, ITriggerBuilder
    {
        private readonly IJobScheduleOptions _scheduleOptions;
        
        public JobTriggerBuilder(IJobScheduleOptions scheduleOptions)
        {
            _scheduleOptions = scheduleOptions ?? throw new ArgumentNullException(nameof(scheduleOptions));
        }
        
        #region IJobBuilder
        
        public IJobDetail BuildNotActualTenderArchiveJob()
        {
            var jobDetail = JobBuilder.Create<NotActualTenderArchiveJob>()
                .WithIdentity(nameof(NotActualTenderArchiveJob))
                .Build();
            return jobDetail;
        }

        public IJobDetail BuildNotActualTenderDocumentDeleteJob()
        {
            var jobDetail = JobBuilder.Create<NotActualTenderDocumentDeleteJob>()
                .WithIdentity(nameof(NotActualTenderDocumentDeleteJob))
                .Build();
            return jobDetail;
        }
        
        #endregion
        
        #region ITriggerBuilder
        
        public ITrigger BuildNotActualTenderArchiveTrigger()
        {
            var builder = TriggerBuilder.Create()
                .WithIdentity("NotActualTenderArchiveTrigger")
                .WithCronSchedule(_scheduleOptions.TenderArchiveCronExpression);
                return builder.Build();
        }
        
        public ITrigger BuildNotActualTenderDocumentDeleteTrigger()
        {
            var builder = TriggerBuilder.Create()
                .WithIdentity("NotActualTenderDocumentDeleteTrigger")
                .WithCronSchedule(_scheduleOptions.TenderDocumentDeleteCronExpression);
            return builder.Build();
        }
        
        #endregion
    }
}