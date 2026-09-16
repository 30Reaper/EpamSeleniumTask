using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace EpamSeleniumTask.Core;

public static class BrowserFactory
{
    public static IWebDriver CreateDriver(IConfiguration configuration, string? downloadDirectory = null)
    {
        var browserName = configuration["Browser:Name"] ?? "Chrome";
        var headless = bool.TryParse(configuration["Browser:Headless"], out var h) && h;

        if (browserName.Equals("Chrome", StringComparison.OrdinalIgnoreCase))
        {
            ChromeOptions options = new();
            options.AddArgument("--start-maximized");
            if (headless)
                options.AddArgument("--headless=new");

            if (!string.IsNullOrEmpty(downloadDirectory) || !string.IsNullOrEmpty(configuration["Browser:DownloadDirectory"]))
            {
                string dir = downloadDirectory ?? configuration["Browser:DownloadDirectory"]!;
                options.AddUserProfilePreference("download.default_directory", dir);
                options.AddUserProfilePreference("download.prompt_for_download", false);
                options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
            }

            return new ChromeDriver(options);
        }

        throw new NotSupportedException($"Browser '{browserName}' is not supported by BrowserFactory.");
    }
}
