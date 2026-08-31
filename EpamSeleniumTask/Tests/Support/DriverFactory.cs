using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace EpamSeleniumTask.Tests.Support;

public static class DriverFactory
{
    private static IWebDriver? _current;

    public static IWebDriver? Current => _current;

    public static void Create(string? downloadDirectory = null)
    {
        if (_current is not null)
            return;

        ChromeOptions options = new();
        options.AddArgument("--start-maximized");

        if (downloadDirectory is not null)
        {
            options.AddUserProfilePreference("download.default_directory", downloadDirectory);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
        }

        _current = new ChromeDriver(options);
    }

    public static void Quit()
    {
        try
        {
            _current?.Quit();
            _current = null;
        }
        catch (Exception)
        {
            _current = null;
        }
    }
}
