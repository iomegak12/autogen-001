using System.IO;
using Microsoft.Extensions.Configuration;

namespace TrainingAgent
{
    public static class Utils
    {
        private static IConfigurationRoot _configuration;
        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    _configuration = GetConfiguration();
                }
                return _configuration;
            }
        }

        private static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static string GetConfigValue(string key)
        {
            return Configuration[key] ?? string.Empty;
        }
    }
}