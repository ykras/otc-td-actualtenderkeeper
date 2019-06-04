using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ActualTenderKeeper.Core
{
    public interface INotActualTendersArchiver
    {
        Task<ElasticProcessResult> ReindexNotActualTenders(string queryJson, CancellationToken ct = default(CancellationToken));

        Task<ElasticProcessResult> DeleteNotActualTenders(string queryJson, CancellationToken ct = default(CancellationToken));
    }
    
    public class ElasticProcessResult
    {
        public TimeSpan SpentTime { get; set; }
        
        public long NumberOfProcessedItems { get; set; }
        
        public IEnumerable<string> Failures { get; set; }
    }
}