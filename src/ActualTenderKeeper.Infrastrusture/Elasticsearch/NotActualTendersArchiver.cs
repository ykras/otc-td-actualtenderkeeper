using System;
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
    public sealed class NotActualTendersArchiver : INotActualTendersArchiver, IDisposable
    {
        private readonly IElasticsearchOptions _options;
        private readonly StaticConnectionPool _connectionPool;
        private readonly ConnectionSettings _connectionSettings;
        private readonly ElasticClient _client;

        public NotActualTendersArchiver(IElasticsearchOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _connectionPool = new StaticConnectionPool(
                _options.BootstrapHosts.Select(x => new Uri($"http://{x}")));
            _connectionSettings = new ConnectionSettings(_connectionPool);
            _client = new ElasticClient(_connectionSettings);
        }
        
        #region IElasticClient
        
        public Task<ElasticProcessResult> ReindexNotActualTenders(string queryJson, CancellationToken ct)
        {
            EnsureNotDisposed();
            if (string.IsNullOrWhiteSpace(queryJson)) throw new ArgumentNullException(nameof(queryJson));
            return run();

            async Task<ElasticProcessResult> run()
            {
                var res = await _client.ReindexOnServerAsync(x => x
                    .Source(s => s.Index(_options.ActualTenderIndexName)
                        .Query<Tender>(q => q.Raw(queryJson))
                    )
                    .Destination(d => d.Index(_options.ArchiveTenderIndexName)), ct);
            
                EnsureValidResponse(res);
            
                return new ElasticProcessResult
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
            throw new Exception($"Failed to reindex tenders\n{reason}");
        }

        public Task<ElasticProcessResult> DeleteNotActualTenders(string queryJson, CancellationToken ct)
        {
            EnsureNotDisposed();
            if (string.IsNullOrWhiteSpace(queryJson)) throw new ArgumentNullException(nameof(queryJson));
            return run();
            
            async Task<ElasticProcessResult> run()
            {
                var res = await _client.DeleteByQueryAsync<Tender>(d => d
                    .Index(_options.ActualTenderIndexName)
                    .Query(q => q.Raw(queryJson)), ct);
                EnsureValidResponse(res);
                return new ElasticProcessResult
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