using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActualTenderKeeper.Core
{
    public interface IElasticAgent
    {
        Task<ElasticResult> ReindexNotActualTenders(string queryJson, CancellationToken ct = default(CancellationToken));

        Task<ElasticResult> DeleteNotActualTenders(string queryJson, CancellationToken ct = default(CancellationToken));

        Task<ElasticResult<TenderDocumentsBatch>> ReadBatchOfTenderDocuments(
            int offset, CancellationToken ct);

        Task<IEnumerable<TenderDocument>> SelectNotActualTenderDocuments(
            IEnumerable<TenderDocument> docs, CancellationToken ct);

        Task<ElasticResult> DeleteTenderDocuments(
            IEnumerable<TenderDocument> docs, CancellationToken ct);
    }
}