using Microsoft.Extensions.Configuration;

namespace LikesChecker.Configuration;

public class ConfigurationHelper
{
    public static IConfiguration BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        return builder.Build();
    }
}