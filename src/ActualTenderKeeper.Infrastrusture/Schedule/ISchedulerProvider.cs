using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule
{
    public interface ISchedulerProvider
    {
        Task<IScheduler> GetScheduler(CancellationToken ct = default(CancellationToken));
    }
}