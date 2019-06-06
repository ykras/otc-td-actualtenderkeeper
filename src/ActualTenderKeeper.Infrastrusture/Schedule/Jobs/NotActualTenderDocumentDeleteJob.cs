using System;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;
using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule.Jobs
{
    [DisallowConcurrentExecution]
    public class NotActualTenderDocumentDeleteJob : IJob
    {
        private readonly ITenderDocumentDeletionStrategy _strategy;
        private readonly ILog _log;
        
        public NotActualTenderDocumentDeleteJob(ITenderDocumentDeletionStrategy strategy, ILog log)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }
        
        #region IJob
        
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _strategy.DeleteNotActualTenderDocuments(context.CancellationToken);
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }
        
        #endregion
    }
}