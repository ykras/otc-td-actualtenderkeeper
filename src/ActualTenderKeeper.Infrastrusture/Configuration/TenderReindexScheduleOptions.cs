using System;
using ActualTenderKeeper.Abstract;
using Microsoft.Extensions.Configuration;

namespace ActualTenderKeeper.Infrastructure.Configuration
{
    public sealed class TenderReindexScheduleOptions : ITenderReindexScheduleOptions
    {
        private const string ConfigSectionName = "TenderReindexSchedule";
        private const string CronExpressionOptionKey = "CronExpression";
        
        private readonly IConfiguration _config;
        
        public TenderReindexScheduleOptions(IConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _config = config.GetSection(ConfigSectionName);
        }
        
        #region ITenderReindexScheduleOptions

        public string CronExpression => _config.GetValue<string>(CronExpressionOptionKey);

        #endregion
    }
}