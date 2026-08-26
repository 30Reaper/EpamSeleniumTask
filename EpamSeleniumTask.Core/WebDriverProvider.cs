using System.Threading;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Serilog;

namespace EpamSeleniumTask.Core;

public sealed class WebDriverProvider
{
    private static readonly AsyncLocal<IWebDriver?> _current = new();
    private static readonly Lazy<WebDriverProvider> _instance = new(() => new WebDriverProvider());

    public static WebDriverProvider Instance => _instance.Value;

    private WebDriverProvider() { }

    public IWebDriver GetOrCreate(IConfiguration configuration, string? downloadDirectory = null)
    {
        if (_current.Value is null)
        {
            _current.Value = BrowserFactory.CreateDriver(configuration, downloadDirectory);
            Log.Information("Created new webdriver instance.");
        }

        return _current.Value!;
    }

    public void QuitAndCleanup()
    {
        try
        {
            _current.Value?.Quit();
            _current.Value?.Dispose();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Exception while quitting driver");
        }
        finally
        {
            _current.Value = null;
        }
    }
}
