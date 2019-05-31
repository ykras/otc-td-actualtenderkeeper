using System;
using System.Threading;
using ActualTenderKeeper.Abstract;

namespace ActualTenderKeeper.Core
{
    public sealed class TenderReindexController : ITenderReindexController
    {
        private readonly IElasticsearchOptions _esOptions;
        
        
        public TenderReindexController(IElasticsearchOptions esOpts)
        {
            if (esOpts == null) throw new ArgumentNullException(nameof(esOpts));
            _esOptions = esOpts;
        }
        
        
        public void StartAsync(CancellationToken ct = default(CancellationToken))
        {
            
        }

        public void StopAsync(CancellationToken ct = default(CancellationToken))
        {
           
        }
    }
}