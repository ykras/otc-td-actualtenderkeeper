using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActualTenderKeeper.Abstract
{
    public interface ITenderReindexController
    {
        Task StartAsync(CancellationToken ct = default(CancellationToken));

        Task StopAsync(CancellationToken ct = default(CancellationToken));
    }
}
