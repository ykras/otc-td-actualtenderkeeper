using System;
using System.Threading;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;
using Infrastructure.Abstract.Logging;

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
            var offset = 0;
            while (true)
            {
                var batch = await _elasticAgent.ReadBatchOfTenderDocuments(offset, ct);
                if (batch.NumberOfProcessedItems < 1) break;
                var docs = batch.Result.TenderDocuments;
                var notActualDocs = await _elasticAgent.SelectNotActualTenderDocuments(docs, ct);
                var res = await _elasticAgent.DeleteTenderDocuments(notActualDocs, ct);
                offset += _options.BatchSize;
            }
        }
        
        #endregion
    }
}