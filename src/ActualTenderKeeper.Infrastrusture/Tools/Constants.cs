namespace ActualTenderKeeper.Infrastructure.Tools
{
    public static class Constants
    {
        public const string ServiceName = "Traderegistry.Elastic.ActualTenderKeeper";
        public const string ServiceDisplayName = "Traderegistry.Elastic.ActualTenderKeeper";
        public const string ServiceDescription = "Keeper of actual tenders index in Elasticsearch";
        public const int DelayBeforeRestartServiceInMinutes = 1;
        public const int DelayBeforeResetFailCountInDays = 1;
    }
}