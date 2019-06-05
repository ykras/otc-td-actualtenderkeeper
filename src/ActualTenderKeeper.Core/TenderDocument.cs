using System.Collections.Generic;

namespace ActualTenderKeeper.Core
{
    public sealed class TenderDocument
    {
        public long Id { get; set; }
        
        public long TenderId { get; set; }
    }

    public sealed class TenderDocumentsBatch
    {
        public long Total { get; set; }
        
        public IEnumerable<TenderDocument> TenderDocuments { get; set; }
    }
}