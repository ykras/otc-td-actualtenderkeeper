using System.Threading;
using System.Threading.Tasks;

namespace ActualTenderKeeper.Abstract
{
    public interface ITenderArchiveStrategy
    {
        Task ArchiveNotActualTenders(CancellationToken ct = default(CancellationToken));
    }
}