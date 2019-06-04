using System;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;
using Infrastructure.Abstract.Logging;
using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class TenderArchiveJob : IJob
    {
        private readonly ITenderArchiveStrategy _strategy;
        private readonly ILog _log;
        
        public TenderArchiveJob(ITenderArchiveStrategy strategy, ILog log)
        {
            _strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }
        
        #region IJob
        
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _strategy.ArchiveNotActualTenders(context.CancellationToken);
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
        }
        
        #endregion
    }
}