using System.Threading;
using System.Threading.Tasks;

namespace ActualTenderKeeper.Abstract
{
    public interface ITenderDocumentDeletionStrategy
    {
        Task DeleteNotActualTenderDocuments(CancellationToken ct = default(CancellationToken));
    }
}