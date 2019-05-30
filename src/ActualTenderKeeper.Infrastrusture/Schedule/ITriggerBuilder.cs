using Quartz;

namespace ActualTenderKeeper.Infrastructure.Schedule
{
    public interface ITriggerBuilder
    {
        ITrigger BuildActualTenderReindexTrigger();
    }
}