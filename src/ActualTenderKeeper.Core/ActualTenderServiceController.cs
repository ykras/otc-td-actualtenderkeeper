using System;
using System.Threading;
using ActualTenderKeeper.Abstract;

namespace ActualTenderKeeper.Core
{
    public sealed class ActualTenderServiceController : IActualTenderServiceController
    {
        private readonly IElasticsearchOptions _esOptions;
        
        
        public ActualTenderServiceController(IElasticsearchOptions esOpts)
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