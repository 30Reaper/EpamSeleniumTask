using EpamSeleniumTask.Business.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.Linq;
using Xunit;

namespace EpamSeleniumTask.Tests;

public class EpamTests : TestBase
{
    [Theory]
    [InlineData("Java", "Ukraine")]
    public void SearchForPosition(string programmingLanguage, string country)
    {
        RunTest(driver =>
        {
            var logger = LoggerFactory.CreateLogger<JobSearchPage>();
            JobSearchPage searchPage = new(driver, WebsiteUrl, logger);
            string jobDetails = searchPage.Search(programmingLanguage, country);

            Assert.Contains(programmingLanguage, jobDetails, StringComparison.OrdinalIgnoreCase);
        });
    }

    [Theory]
    [InlineData("BLOCKCHAIN")]
    [InlineData("Cloud")]
    [InlineData("Automation")]
    public void GlobalSearch(string searchText)
    {
        RunTest(driver =>
        {
            var logger = LoggerFactory.CreateLogger<SearchResultsPage>();
            SearchResultsPage resultsPage = new(driver, WebsiteUrl, logger);
            IReadOnlyCollection<string> resultTitles = resultsPage.Search(searchText);

            Assert.NotEmpty(resultTitles);
            bool allResultsContainSearchText = resultTitles.All(title =>
                title.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            Assert.True(
                allResultsContainSearchText,
                $"Not all search result links contain '{searchText}'.");
        });
    }

    [Theory]
    [InlineData("Code-Of-Conduct_01_26.pdf")]
    public void CodeOfEthicalConductIsDownloaded(string expectedFileName)
    {
        string downloadDirectory = Path.Combine(Path.GetTempPath(), $"epam-download-{Guid.NewGuid():N}");
        Directory.CreateDirectory(downloadDirectory);

        try
        {
            RunTest(driver =>
            {
                var logger = LoggerFactory.CreateLogger<HomePage>();
                HomePage homePage = new(driver, WebsiteUrl, logger);
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
            }, downloadDirectory);
        }
        finally
        {
            Directory.Delete(downloadDirectory, true);
        }
    }

    [Theory]
    [InlineData(2)]
    public void InsightsCarouselTitleMatchesArticleTitle(int swipeCount)
    {
        RunTest(driver =>
        {
            var logger = LoggerFactory.CreateLogger<InsightsPage>();
            InsightsPage insightsPage = new(driver, WebsiteUrl, logger);
            insightsPage.Open();
            insightsPage.SwipeCarousel(swipeCount);
            string carouselTitle = insightsPage.GetActiveArticleTitle();

            ArticlePage articlePage = insightsPage.OpenActiveArticle();

            Assert.Equal(carouselTitle, articlePage.GetTitle());
        });
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
