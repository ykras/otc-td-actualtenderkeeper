using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule
{
    public sealed class JobTriggerBuilder : IJobBuilder, ITriggerBuilder
    {
        public JobTriggerBuilder()
        {
            
        }
        
        #region IJobBuilder
        
        public IJobDetail BuildActualTenderReindexJob()
        {
            var jobDetail = JobBuilder.Create<ActualTenderReindexJob>()
                .WithIdentity(nameof(ActualTenderReindexJob))
                .Build();
            return jobDetail;
        }
        
        #endregion
        
        #region ITriggerBuilder
        
        public ITrigger BuildActualTenderReindexTrigger()
        {
            var builder = TriggerBuilder.Create()
                .WithIdentity("ActualTenderReindexTrigger")
                .WithCronSchedule("0 0/1 * 1/1 * ? *");
                return builder.Build();
        }
        
        #endregion
    }
}