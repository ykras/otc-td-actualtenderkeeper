using System.Threading;
using ActualTenderKeeper.Abstract;

namespace ActualTenderKeeper.Core
{
    public sealed class ActualTenderKeeperController : IActualTenderKeeperController
    {
        public ActualTenderKeeperController()
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