using System;
using System.Threading.Tasks;
using Infrastructure.Abstract.Logging;
using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule
{
    public sealed class ActualTenderReindexJob : IJob
    {
        private readonly ILog _log;

        public ActualTenderReindexJob(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }
        
        #region IJob
        
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
               
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            
            return Task.FromResult("OK");
        }
        
        #endregion
    }
}