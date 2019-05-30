using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ActualTenderKeeper.Abstract
{
    public interface IActualTenderService
    {
        void StartAsync(CancellationToken ct = default(CancellationToken));

        void StopAsync(CancellationToken ct = default(CancellationToken));
    }
}
