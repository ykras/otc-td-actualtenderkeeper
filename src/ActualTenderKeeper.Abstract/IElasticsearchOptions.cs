using System.Collections.Generic;

namespace ActualTenderKeeper.Abstract
{
    public interface IElasticsearchOptions
    {
        IEnumerable<string> BootstrapHosts { get; }
        
        string ActualTenderIndexName { get; }
        
        string ArchiveTenderIndexName { get; }
        
        string TenderDocumentIndexName { get; }
        
        int BatchSize { get; }
    }
}