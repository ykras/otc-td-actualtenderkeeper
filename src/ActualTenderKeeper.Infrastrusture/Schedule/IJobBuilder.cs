using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule
{
    public interface IJobBuilder
    {
        IJobDetail BuildActualTenderReindexJob();
    }
}