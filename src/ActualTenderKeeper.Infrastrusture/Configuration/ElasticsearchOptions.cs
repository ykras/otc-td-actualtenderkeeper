using System;
using System.Collections.Generic;
using ActualTenderKeeper.Abstract;
using Microsoft.Extensions.Configuration;

namespace ActualTenderKeeper.Infrastructure.Configuration
{
    public sealed class ElasticsearchOptions : IElasticsearchOptions
    {
        private readonly IConfiguration _config;
        private const string ConfigSectionName = "Elasticsearch";
        private const string BootstrapHostsOptionKey = "BootstrapHosts";
        private const string ActualTenderIndexNameOptionKey = "ActualTenderIndex";
        private const string NotActualTenderIndexNameOptionKey = "NotActualTenderIndex";
        private const string TenderDocumentIndexNameOptionKey = "TenderDocumentIndex";

        public ElasticsearchOptions(IConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _config = config.GetSection(ConfigSectionName);
        }
        
        #region IElasticsearchOptions

        public IEnumerable<string> BootstrapHosts => _config.GetSection(BootstrapHostsOptionKey).Get<string[]>();

        public string ActualTenderIndexName => _config.GetValue<string>(ActualTenderIndexNameOptionKey);

        public string NotActualTenderIndexName => _config.GetValue<string>(NotActualTenderIndexNameOptionKey);

        #endregion
    }
}