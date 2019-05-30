using System.Threading;
using ActualTenderKeeper.Abstract;

namespace ActualTenderKeeper.Core
{
    public sealed class ActualTenderService : IActualTenderService
    {
        public ActualTenderService()
        {
            
        }
        
        
        public void StartAsync(CancellationToken ct = default(CancellationToken))
        {
            
        }

        public void StopAsync(CancellationToken ct = default(CancellationToken))
        {
           
        }
    }
}