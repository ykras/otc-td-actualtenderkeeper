using System;
using System.Collections.Generic;

namespace ActualTenderKeeper.Core
{
    public class ElasticResult
    {
        public TimeSpan SpentTime { get; set; }
        
        public long NumberOfProcessedItems { get; set; }
        
        public IEnumerable<string> Failures { get; set; }
    }

    public sealed class ElasticResult<TResult> : ElasticResult
    {
        public TResult Result { get; set; }
    }
}