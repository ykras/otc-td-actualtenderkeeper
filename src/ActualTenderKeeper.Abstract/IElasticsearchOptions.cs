using System.Collections.Generic;

namespace ActualTenderKeeper.Abstract
{
    public interface IElasticsearchOptions
    {
        IEnumerable<string> BootstrapHosts { get; }
    }
}