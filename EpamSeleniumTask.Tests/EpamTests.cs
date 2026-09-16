using EpamSeleniumTask.Business.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.Linq;
using NUnit.Framework;

namespace EpamSeleniumTask.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class EpamTests : TestBase
{
    [TestCase("Java", "Ukraine")]
    public void SearchForPosition(string programmingLanguage, string country)
    {
        var logger = LoggerFactory.CreateLogger<JobSearchPage>();
        JobSearchPage searchPage = new(Driver!, WebsiteUrl, logger);
        string jobDetails = searchPage.Search(programmingLanguage, country);

        Assert.That(jobDetails, Does.Contain(programmingLanguage).IgnoreCase);
    }

    [TestCase("BLOCKCHAIN")]
    [TestCase("Cloud")]
    [TestCase("Automation")]
    public void GlobalSearch(string searchText)
    {
        var logger = LoggerFactory.CreateLogger<SearchResultsPage>();
        SearchResultsPage resultsPage = new(Driver!, WebsiteUrl, logger);
        IReadOnlyCollection<string> resultTitles = resultsPage.Search(searchText);

        Assert.IsNotEmpty(resultTitles);
        bool allResultsContainSearchText = resultTitles.All(title =>
            title.Contains(searchText, StringComparison.OrdinalIgnoreCase));

        Assert.True(
            allResultsContainSearchText,
            $"Not all search result links contain '{searchText}'.");
    }

    [TestCase("Code-Of-Conduct_01_26.pdf")]
    [Category("Download")]
    public void CodeOfEthicalConductIsDownloaded(string expectedFileName)
    {
        var logger = LoggerFactory.CreateLogger<HomePage>();
        HomePage homePage = new(Driver!, WebsiteUrl, logger);
        homePage.Open();
        homePage.OpenCodeOfEthicalConduct();

        string downloadedFile = WaitForDownload(DownloadDirectory!, expectedFileName);

        string actualFileName = Path.GetFileName(downloadedFile);
        if (!string.Equals(actualFileName, expectedFileName, StringComparison.OrdinalIgnoreCase))
        {
            string lowered = actualFileName.ToLowerInvariant();
            Assert.True(lowered.Contains("code") && lowered.Contains("conduct"),
                $"Downloaded file name '{actualFileName}' does not match expected '{expectedFileName}' and does not look like the Code of Conduct file.");
        }
    }

    [TestCase(2)]
    public void InsightsCarouselTitleMatchesArticleTitle(int swipeCount)
    {
        var logger = LoggerFactory.CreateLogger<InsightsPage>();
        InsightsPage insightsPage = new(Driver!, WebsiteUrl, logger);
        insightsPage.Open();
        insightsPage.SwipeCarousel(swipeCount);
        string carouselTitle = insightsPage.GetActiveArticleTitle();

        ArticlePage articlePage = insightsPage.OpenActiveArticle();

        Assert.AreEqual(carouselTitle, articlePage.GetTitle());
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
        throw new AssertionException($"File '{expectedFileName}' was not downloaded. Files found: {files}");
    }
}
