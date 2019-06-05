using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;
using Infrastructure.Abstract.Logging;

using static ActualTenderKeeper.Core.ElasticQuery;

namespace ActualTenderKeeper.Core
{
    public sealed class TenderArchiveStrategy : ITenderArchiveStrategy
    {
        private readonly IElasticAgent _elasticAgent;
        private readonly IElasticsearchOptions _options;
        private readonly ILog _log;

        public TenderArchiveStrategy(IElasticAgent elasticAgent,
            IElasticsearchOptions options,
            ILog log)
        {
            _elasticAgent = elasticAgent ?? throw new ArgumentNullException(nameof(elasticAgent));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }
        
        #region ITenderArchiveStrategy

        public async Task ArchiveNotActualTenders(CancellationToken ct = default(CancellationToken))
        {
            LogStartReindexTenders();
            var r = await _elasticAgent.ReindexNotActualTenders(NotActualTendersQueryJson, ct);
            LogCompleteReindexTendres(r);
            
            LogStartDeleteTenders();
            r = await _elasticAgent.DeleteNotActualTenders(NotActualTendersQueryJson, ct);
            LogCompleteDeleteTenders(r);
        }

        private void LogStartReindexTenders()
        {
            _log.Info("Start reindexing of not actual tenders " +
                      $"from index {_options.ActualTenderIndexName} to index {_options.ArchiveTenderIndexName}");
        }

        private void LogCompleteReindexTendres(ElasticResult r)
        {
            _log.Info("Reindexing of not actual tenders completed");
            _log.Info($"It's took {r.SpentTime.TotalMilliseconds} milliseconds");
            _log.Info($"Reindexed {r.NumberOfProcessedItems} not actual tenders");
            if (r.Failures?.Count() > 0)
            {
                _log.Info("Some failures occured:");
                _log.Info($"{string.Join(Environment.NewLine, r.Failures)}");     
            }
        }

        private void LogStartDeleteTenders()
        {
            _log.Info("Start deletion of not actual tenders " +
                      $"from index {_options.ActualTenderIndexName}");
        }

        private void LogCompleteDeleteTenders(ElasticResult r)
        {
            _log.Info("Deletion of not actual tenders completed");
            _log.Info($"It's took {r.SpentTime.TotalMilliseconds} milliseconds");
            _log.Info($"Deleted {r.NumberOfProcessedItems} not actual tenders");
            if (r.Failures?.Count() > 0)
            {
                _log.Info("Some failures occured:");
                _log.Info($"{string.Join(Environment.NewLine, r.Failures)}");     
            }
        }

        #endregion
    }
}