using System;
using System.Collections.Generic;
using ActualTenderKeeper.Abstract;
using Microsoft.Extensions.Configuration;

namespace ActualTenderKeeper.Infrastructure.Configuration
{
    public sealed class Options : IElasticsearchOptions
    {
        private readonly IConfiguration _config;
        private const string ConfigSectionName = "Elasticsearch";
        private const string BootstrapHostsOptionKey = "BootstrapHosts";

        public Options(IConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _config = config.GetSection(ConfigSectionName);
        }
        
        #region IElasticsearchOptions

        public IEnumerable<string> BootstrapHosts => _config.GetSection(BootstrapHostsOptionKey).Get<string[]>();

        #endregion
    }
}