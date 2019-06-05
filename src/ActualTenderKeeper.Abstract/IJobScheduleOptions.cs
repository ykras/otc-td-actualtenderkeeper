namespace ActualTenderKeeper.Abstract
{
    public interface IJobScheduleOptions
    {
        string TenderArchiveCronExpression { get; }
        
        string TenderDocumentDeleteCronExpression { get; }
    }
}