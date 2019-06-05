using System;
using ActualTenderKeeper.Abstract;
using Microsoft.Extensions.Configuration;

namespace ActualTenderKeeper.Infrastructure.Configuration
{
    public sealed class JobScheduleOptions : IJobScheduleOptions
    {
        private const string ConfigSectionName = "JobSchedule";
        private const string TenderArchiveCronExpressionOptionKey = "TenderArchiveCronExpression";
        private const string TenderDocumentDeleteCronExpressionOptionKey = "TenderDocumentDeleteCronExpression";
        
        private readonly IConfiguration _config;
        
        public JobScheduleOptions(IConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _config = config.GetSection(ConfigSectionName);
        }
        
        #region ITenderReindexScheduleOptions

        public string TenderArchiveCronExpression =>
            _config.GetValue<string>(TenderArchiveCronExpressionOptionKey);

        public string TenderDocumentDeleteCronExpression =>
            _config.GetValue<string>(TenderDocumentDeleteCronExpressionOptionKey);

        #endregion
    }
}