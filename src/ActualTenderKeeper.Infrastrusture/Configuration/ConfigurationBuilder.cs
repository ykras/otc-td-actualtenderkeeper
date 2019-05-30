using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ActualTenderKeeper.Infrastructure.Configuration
{
    public static class ConfigurationBuilder
    {
        public static IConfiguration BuildConfiguration()
        {
            var configBasePath = AppContext.BaseDirectory;
            var environment = Environment.GetEnvironmentVariable("TRADEREGISTRY_ELASTIC_ACTUALTENDERKEEPER_ENVIRONMENT");
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
                .AddJsonFile(Path.Combine(configBasePath, "config.json"), optional: false, reloadOnChange: true)
                .AddJsonFile(Path.Combine(configBasePath, $"config.{environment}.json"), optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();
            return configuration;
        }
    }
}