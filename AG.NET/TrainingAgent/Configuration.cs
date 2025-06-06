#pragma warning disable IDE0073
// The code that's violating the rule is on this line.
#pragma warning restore IDE0073

using Microsoft.Extensions.Configuration;

public class Configuration
{
    public static IConfigurationRoot ConfigureAppSettings()
    {
        var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                        .AddJsonFile("appsettings.Production.json", optional: true, reloadOnChange: true)
#if DEBUG
                        .AddJsonFile(GetUserJsonFilename(), optional: true, reloadOnChange: true)
#endif
                        .Build();
        return config;
    }

    static string GetUserJsonFilename()
    {
        return $"appsettings.user_{Environment.UserName.ToLower()}.json";
    }
}
