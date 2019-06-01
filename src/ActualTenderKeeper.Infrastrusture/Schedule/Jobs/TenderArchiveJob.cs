using System;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;
using ActualTenderKeeper.Infrastructure.Elasticsearch;
using Infrastructure.Abstract.Logging;
using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class TenderArchiveJob : IJob
    {
        private readonly ILog _log;

        private readonly IElasticsearchOptions _options;

        public TenderArchiveJob(ILog log,
            IElasticsearchOptions options)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));


            _options = options;
        }
        
        #region IJob
        
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var interop = new ElasticInterop(_options);

                var t = interop.FindTender();
                
                throw new Exception("ERR");

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