namespace ActualTenderKeeper.Abstract
{
    public interface ITenderReindexScheduleOptions
    {
        string CronExpression { get; }
    }
}