using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace EpamSeleniumTask.Tests;

[SetUpFixture]
public class LoggingOneTimeSetup
{
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        var env = Environment.GetEnvironmentVariable("TAF_ENV") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

        if (!string.IsNullOrEmpty(env))
            builder.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);

        builder.AddEnvironmentVariables();

        var configuration = builder.Build();

        EpamSeleniumTask.Core.Logging.Init(configuration);
    }

    [OneTimeTearDown]
    public void GlobalTeardown()
    {
        // Optionally flush/close logging sinks if necessary
    }
}
