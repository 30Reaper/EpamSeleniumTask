using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Serilog;
using EpamSeleniumTask.Core;
using Microsoft.Extensions.Logging;

namespace EpamSeleniumTask.Tests;

public abstract class TestBase
{
    protected IConfiguration Configuration { get; }
    protected ILoggerFactory LoggerFactory { get; }

    protected TestBase()
    {
        var env = Environment.GetEnvironmentVariable("TAF_ENV") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "";

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

        if (!string.IsNullOrEmpty(env))
            builder.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);

        builder.AddEnvironmentVariables();

        Configuration = builder.Build();

        Logging.Init(Configuration);
        LoggerFactory = Logging.LoggerFactory ?? new Serilog.Extensions.Logging.SerilogLoggerFactory(Serilog.Log.Logger);
    }

    protected string WebsiteUrl => Configuration["WebsiteUrl"]!;

    protected void RunTest(Action<IWebDriver> testAction, string? downloadDirectory = null, [CallerMemberName] string? testName = null)
    {
        IWebDriver driver = WebDriverProvider.Instance.GetOrCreate(Configuration, downloadDirectory);
        var logger = LoggerFactory.CreateLogger("Test");
        try
        {
            logger.LogInformation("Starting test {TestName}", testName);
            testAction(driver);
            logger.LogInformation("Test {TestName} finished successfully", testName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Test {TestName} failed", testName);
            try
            {
                string logDir = Configuration["Logging:LogDirectory"] ?? "logs";
                ScreenshotHelper.Capture(driver, logDir, testName ?? "screenshot");
            }
            catch (Exception captureEx)
            {
                logger.LogError(captureEx, "Failed capturing screenshot for {TestName}", testName);
            }

            throw;
        }
        finally
        {
            WebDriverProvider.Instance.QuitAndCleanup();
        }
    }
}
