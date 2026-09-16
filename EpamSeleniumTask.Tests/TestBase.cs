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
    protected IWebDriver? Driver { get; private set; }

    protected string? DownloadDirectory { get; private set; }

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

        LoggerFactory = Logging.LoggerFactory ?? new Serilog.Extensions.Logging.SerilogLoggerFactory(Serilog.Log.Logger);
    }

    [NUnit.Framework.SetUp]
    public void SetUp()
    {
        var props = NUnit.Framework.TestContext.CurrentContext.Test.Properties;

        if (props.ContainsKey("Category") &&
            props["Category"].Cast<string?>().Any(c =>
                string.Equals(c, "Download", StringComparison.OrdinalIgnoreCase)))
        {
            DownloadDirectory = Path.Combine(
                Path.GetTempPath(),
                $"epam-download-{Guid.NewGuid():N}");

            Directory.CreateDirectory(DownloadDirectory);
        }

        if (props.ContainsKey("Category") &&
            props["Category"].Cast<string?>().Any(c =>
                string.Equals(c, "API", StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        Driver = WebDriverProvider.Instance.GetOrCreate(Configuration, DownloadDirectory);

        var logger = LoggerFactory.CreateLogger("Test");

        logger.LogInformation(
            "SetUp complete for test {TestName}",
            NUnit.Framework.TestContext.CurrentContext.Test.Name);
    }
    [NUnit.Framework.TearDown]
    public void TearDown()
    {
        var logger = LoggerFactory.CreateLogger("Test");

        try
        {
            var testStatus = NUnit.Framework.TestContext.CurrentContext.Result.Outcome.Status;

            if (testStatus == NUnit.Framework.Interfaces.TestStatus.Failed &&
                Driver != null)
            {
                string logDir = Configuration["Logging:LogDirectory"] ?? "logs";

                ScreenshotHelper.Capture(
                    Driver,
                    logDir,
                    NUnit.Framework.TestContext.CurrentContext.Test.Name);

                logger.LogInformation(
                    "Screenshot captured for failed test {TestName}",
                    NUnit.Framework.TestContext.CurrentContext.Test.Name);
            }

            WebDriverProvider.Instance.QuitAndCleanup();

            logger.LogInformation(
                "TearDown: WebDriver quit and cleaned up for test {TestName}",
                NUnit.Framework.TestContext.CurrentContext.Test.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error during TearDown for test {TestName}",
                NUnit.Framework.TestContext.CurrentContext.Test.Name);
        }

        if (!string.IsNullOrEmpty(DownloadDirectory) &&
            Directory.Exists(DownloadDirectory))
        {
            try
            {
                Directory.Delete(DownloadDirectory, true);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    "Failed to delete download directory {DownloadDir}",
                    DownloadDirectory);
            }
            finally
            {
                DownloadDirectory = null;
            }
        }
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
