using EpamSeleniumTask.Pages;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace EpamSeleniumTask.Tests;

public class EpamTests
{
    private readonly IConfiguration _configuration;

    public EpamTests()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
    }

    private string WebsiteUrl => _configuration["WebsiteUrl"]!;

    private static ChromeDriver CreateDriver(string? downloadDirectory = null)
    {
        ChromeOptions options = new();
        options.AddArgument("--start-maximized");

        if (downloadDirectory is not null)
        {
            options.AddUserProfilePreference("download.default_directory", downloadDirectory);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
        }

        return new ChromeDriver(options);
    }

    [Theory]
    [InlineData("Java", "Ukraine")]
    public void SearchForPosition(string programmingLanguage, string country)
    {
        using ChromeDriver driver = CreateDriver();

        try
        {
            JobSearchPage searchPage = new(driver, WebsiteUrl);
            string jobDetails = searchPage.Search(programmingLanguage, country);

            Assert.Contains(programmingLanguage, jobDetails, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            driver.Quit();
        }
    }

    [Theory]
    [InlineData("BLOCKCHAIN")]
    [InlineData("Cloud")]
    [InlineData("Automation")]
    public void GlobalSearch(string searchText)
    {
        using ChromeDriver driver = CreateDriver();

        try
        {
            SearchResultsPage resultsPage = new(driver, WebsiteUrl);
            IReadOnlyCollection<string> resultTitles = resultsPage.Search(searchText);

            Assert.NotEmpty(resultTitles);
            bool allResultsContainSearchText = resultTitles.All(title =>
                title.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            Assert.True(
                allResultsContainSearchText,
                $"Not all search result links contain '{searchText}'.");
        }
        finally
        {
            driver.Quit();
        }
    }

    [Theory]
    [InlineData("Code-Of-Conduct_01_26.pdf")]
    public void CodeOfEthicalConductIsDownloaded(string expectedFileName)
    {
        string downloadDirectory = Path.Combine(Path.GetTempPath(), $"epam-download-{Guid.NewGuid():N}");
        Directory.CreateDirectory(downloadDirectory);

        using ChromeDriver driver = CreateDriver(downloadDirectory);

        try
        {
            HomePage homePage = new(driver, WebsiteUrl);
            homePage.Open();
            homePage.OpenCodeOfEthicalConduct();

            string downloadedFile = WaitForDownload(downloadDirectory, expectedFileName);

            string actualFileName = Path.GetFileName(downloadedFile);
            if (!string.Equals(actualFileName, expectedFileName, StringComparison.OrdinalIgnoreCase))
            {
                string lowered = actualFileName.ToLowerInvariant();
                Assert.True(lowered.Contains("code") && lowered.Contains("conduct"),
                    $"Downloaded file name '{actualFileName}' does not match expected '{expectedFileName}' and does not look like the Code of Conduct file.");
            }
        }
        finally
        {
            driver.Quit();
            Directory.Delete(downloadDirectory, true);
        }
    }

    [Theory]
    [InlineData(2)]
    public void InsightsCarouselTitleMatchesArticleTitle(int swipeCount)
    {
        using ChromeDriver driver = CreateDriver();

        try
        {
            InsightsPage insightsPage = new(driver, WebsiteUrl);
            insightsPage.Open();
            insightsPage.SwipeCarousel(swipeCount);
            string carouselTitle = insightsPage.GetActiveArticleTitle();

            ArticlePage articlePage = insightsPage.OpenActiveArticle();

            Assert.Equal(carouselTitle, articlePage.GetTitle());
        }
        finally
        {
            driver.Quit();
        }
    }

    private static string WaitForDownload(string directory, string expectedFileName)
    {
        DateTime timeout = DateTime.UtcNow.AddSeconds(30);

        while (DateTime.UtcNow < timeout)
        {
            string expectedPath = Path.Combine(directory, expectedFileName);
            if (File.Exists(expectedPath) && !File.Exists($"{expectedPath}.crdownload"))
            {
                return expectedPath;
            }

            foreach (string file in Directory.GetFiles(directory, "*.pdf"))
            {
                string fileName = Path.GetFileName(file);
                if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) &&
                    !File.Exists($"{file}.crdownload"))
                {
                    string lowered = fileName.ToLowerInvariant();
                    if (lowered.Contains("code") && lowered.Contains("conduct"))
                    {
                        return file;
                    }
                }
            }

            Thread.Sleep(250);
        }

        string files = string.Join(", ", Directory.GetFiles(directory).Select(Path.GetFileName));
        throw new Xunit.Sdk.XunitException($"File '{expectedFileName}' was not downloaded. Files found: {files}");
    }
}