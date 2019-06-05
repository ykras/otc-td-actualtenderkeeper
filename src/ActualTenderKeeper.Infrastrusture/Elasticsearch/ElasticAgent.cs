using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ActualTenderKeeper.Abstract;
using ActualTenderKeeper.Core;
using Elasticsearch.Net;
using Nest;
using Quartz.Util;

namespace ActualTenderKeeper.Infrastructure.Elasticsearch
{
    public sealed class ElasticAgent : IElasticAgent, IDisposable
    {
        private readonly IElasticsearchOptions _options;
        private readonly StaticConnectionPool _connectionPool;
        private readonly ConnectionSettings _connectionSettings;
        private readonly ElasticClient _client;

        public ElasticAgent(IElasticsearchOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _connectionPool = new StaticConnectionPool(
                _options.BootstrapHosts.Select(x => new Uri($"http://{x}")));
            _connectionSettings = new ConnectionSettings(_connectionPool);
            _client = new ElasticClient(_connectionSettings);
        }
        
        #region IElasticAgent
        
        public Task<ElasticResult> ReindexNotActualTenders(string queryJson, CancellationToken ct)
        {
            EnsureNotDisposed();
            if (string.IsNullOrWhiteSpace(queryJson)) throw new ArgumentNullException(nameof(queryJson));
            return run();

            async Task<ElasticResult> run()
            {
                var res = await _client.ReindexOnServerAsync(x => x
                    .Source(s => s.Index(_options.ActualTenderIndexName)
                        .Query<Tender>(q => q.Raw(queryJson))
                    )
                    .Destination(d => d.Index(_options.ArchiveTenderIndexName)), ct);
            
                EnsureValidResponse(res);
            
                return new ElasticResult
                {
                    SpentTime = TimeSpan.FromMilliseconds(res.Took?.Milliseconds ?? 0),
                    NumberOfProcessedItems = res.Total,
                    Failures = res.Failures.Select(x => x.Cause?.Reason).Where(x => !x.IsNullOrWhiteSpace())
                };
            }
        }

        private static void EnsureValidResponse(IResponse response)
        {
            if (response.IsValid) return;
            response.TryGetServerErrorReason(out var reason);
            throw new Exception(reason);
        }

        public Task<ElasticResult> DeleteNotActualTenders(string queryJson, CancellationToken ct)
        {
            EnsureNotDisposed();
            if (string.IsNullOrWhiteSpace(queryJson)) throw new ArgumentNullException(nameof(queryJson));
            return run();
            
            async Task<ElasticResult> run()
            {
                var res = await _client.DeleteByQueryAsync<Tender>(d => d
                    .Index(_options.ActualTenderIndexName)
                    .Query(q => q.Raw(queryJson)), ct);
                EnsureValidResponse(res);
                return new ElasticResult
                {
                    SpentTime = TimeSpan.FromMilliseconds(res.Took),
                    NumberOfProcessedItems = res.Deleted,
                    Failures = res.Failures.Where(x => x.Cause != null).Select(x => x.Cause.ToString())
                };
            }
        }

        public Task<ElasticResult<TenderDocumentsBatch>> ReadBatchOfTenderDocuments(
            int offset, CancellationToken ct)
        {
            EnsureNotDisposed();
            return run();

            async Task<ElasticResult<TenderDocumentsBatch>> run()
            {
                var res = await _client.SearchAsync<TenderDocument>(s => s
                    .Index(_options.TenderDocumentIndexName)
                    .Source(sf => sf.Includes(f => f.Fields(d => d.Id, d => d.TenderId)))
                    .Query(q => q.MatchAll())
                    .Sort(st => st.Ascending(d => d.Id))
                    .From(offset)
                    .Size(_options.BatchSize),
                    ct
                );
                EnsureValidResponse(res);
                var batch = new TenderDocumentsBatch
                {
                    Total = res.HitsMetadata.Total,
                    TenderDocuments = res.Documents
                };
                return new ElasticResult<TenderDocumentsBatch>
                {
                    SpentTime = TimeSpan.FromMilliseconds(res.Took),
                    NumberOfProcessedItems = res.Documents.Count,
                    Result = batch
                }; 
            }
        }

        public Task<IEnumerable<TenderDocument>> SelectNotActualTenderDocuments(IEnumerable<TenderDocument> docs,
            CancellationToken ct)
        {
            EnsureNotDisposed();
            if (docs == null) throw new ArgumentNullException(nameof(docs));
            if (!docs.Any()) return Task.FromResult(Enumerable.Empty<TenderDocument>());
            return run();

            async Task<IEnumerable<TenderDocument>> run()
            {
                //var tenderIds = docs.SelectMany(d => d.TenderId ?? Enumerable.Empty<long>()).ToArray();
                var tenderIds = docs.Select(d => d.TenderId);
                var actualTenderIds = await selectActualTenderIds(tenderIds);
                var actualTenderIdsSet = new HashSet<long>(actualTenderIds);
                //var notActualTenderDocuments = docs.Where(d => !actualTenderIdsSet.Overlaps(d.TenderId)).Select(d => d);
                var notActualTenderDocuments = docs.Where(d => !actualTenderIdsSet.Contains(d.TenderId))
                    .Select(d => d);
                return notActualTenderDocuments;
            }
            
            async Task<IEnumerable<long>> selectActualTenderIds(IEnumerable<long> ids)
            {
                if (!ids.Any()) return Enumerable.Empty<long>();
                var res = await _client.SearchAsync<Tender>(s => s
                        .Index(_options.ActualTenderIndexName)
                        .Source(sf => sf.Includes(f => f.Fields(d => d.Id)))
                        .Query(q => q.Ids(x => x.Values(ids))),
                    ct
                );
                EnsureValidResponse(res);
                var actualTenderIds = res.Hits.Select(h => h.Source.Id);
                return actualTenderIds;
            }
        }

        public Task<ElasticResult> DeleteTenderDocuments(IEnumerable<TenderDocument> docs, CancellationToken ct)
        {
            EnsureNotDisposed();
            if (docs == null) throw new ArgumentNullException(nameof(docs));
            return run();

            async Task<ElasticResult> run()
            {
                var docIds = docs.Select(d => d.Id);
                var res = await _client.DeleteByQueryAsync<Tender>(d => d
                    .Index(_options.ActualTenderIndexName)
                    .Query(q => q.Ids(x => x.Values(docIds))),
                    ct);
                EnsureValidResponse(res);
                return new ElasticResult
                {
                    SpentTime = TimeSpan.FromMilliseconds(res.Took),
                    NumberOfProcessedItems = res.Deleted,
                    Failures = res.Failures.Where(x => x.Cause != null).Select(x => x.Cause.ToString())
                };
            }
        }
        
        #endregion
        
        #region IDisposable

        private bool _disposed;
        
        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().FullName);
        }
        
        public void Dispose()
        {
            if (_disposed) return;
//            _httpClient.Dispose();
//            RestoreSecurityProtocolOptionsFromSnapshot();

            (_connectionSettings as IDisposable).Dispose();
            (_connectionPool as IDisposable).Dispose();

            _disposed = true;   
        }
        
        #endregion
    }
}