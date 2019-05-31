using System;
using System.Linq;
using ActualTenderKeeper.Abstract;
using Elasticsearch.Net;
using Nest;

namespace ActualTenderKeeper.Infrastructure.Elasticsearch
{
    public sealed class ElasticInterop : IDisposable
    {
        private readonly IElasticsearchOptions _options;
        private readonly StaticConnectionPool _connectionPool;
        private readonly ConnectionSettings _connectionSettings;
        
        private readonly ElasticClient _client;

        public ElasticInterop(IElasticsearchOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _connectionPool = new StaticConnectionPool(_options.BootstrapHosts.Select(x => new Uri($"http://{x}")));
            _connectionSettings = new ConnectionSettings(_connectionPool);
            _client = new ElasticClient(_connectionSettings);
        }

        public Tender FindTender()
        {
            var response = _client.Search<Tender>(s => s
                .Index(_options.ActualTenderIndexName)
                .Type("tender")
                .Query(q => q.Ids(c => c
                    .Values(3760131)))
            );
            
            return response.Hits.FirstOrDefault()?.Source;

        }
        
        #region IDisposable
        
        private bool Disposed { get; set; }

        private void EnsureNotDisposed()
        {
            if (Disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        public void Dispose()
        {
            if (Disposed) return;
            (_connectionSettings as IDisposable).Dispose();
            (_connectionPool as IDisposable).Dispose();
        }

        #endregion
    }

    public sealed class Tender
    {
        public int Id { get; set; }
        
        public string TradeName { get; set; }
    }
}