using System;
using System.Threading;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;

namespace ActualTenderKeeper.Core
{
    public sealed class TenderDocumentDeletionStrategy : ITenderDocumentDeletionStrategy
    {
        private readonly IElasticAgent _elasticAgent;
        private readonly IElasticsearchOptions _options;
        private readonly ILog _log;
        
        public TenderDocumentDeletionStrategy(IElasticAgent elasticAgent,
            IElasticsearchOptions options,
            ILog log)
        {
            _elasticAgent = elasticAgent ?? throw new ArgumentNullException(nameof(elasticAgent));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        
        #region ITenderDocumentDeletionStrategy
        public async Task DeleteNotActualTenderDocuments(CancellationToken ct = default(CancellationToken))
        {
            LogStartDeleteTenderDocuments();
            
            var offset = 0;
            while (true)
            {
                var batchRes = await _elasticAgent.ReadBatchOfTenderDocuments(offset, ct);
                if (batchRes.NumberOfProcessedItems < 1) break;
                var docs = batchRes.Result.TenderDocuments;
                var notActualDocs = await _elasticAgent.SelectNotActualTenderDocuments(docs, ct);
                var deleteRes = await _elasticAgent.DeleteTenderDocuments(notActualDocs, ct);
                offset += _options.BatchSize;
            }

            LogCompleteDeleteTenderDocuments();
        }

        private void LogStartDeleteTenderDocuments()
        {
            _log.Info("Start delete of not actual tenders documents " +
                      $"from index {_options.TenderDocumentIndexName}");
        }

        private void LogCompleteDeleteTenderDocuments()
        {
            _log.Info("Deletion of not actual tenders documents completed");
        }
        
        #endregion
    }
}