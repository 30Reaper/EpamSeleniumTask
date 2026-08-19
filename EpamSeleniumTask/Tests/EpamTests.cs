using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Microsoft.Extensions.Configuration;
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

    private static IWebDriver CreateDriver()
    {
        ChromeOptions options = new();
        options.AddArgument("--start-maximized");

        return new ChromeDriver(options);
    }

    [Theory]
    [InlineData("Java", "Ukraine")]
    public void SearchForPosition(string programmingLanguage, string country)
    {
        IWebDriver driver = CreateDriver();

        try
        {
            // 1. Browser setup
            driver.Manage().Window.Maximize();

            // Implicit wait
            driver.Manage().Timeouts().ImplicitWait =
                TimeSpan.FromSeconds(5);

            // Explicit wait
            WebDriverWait wait = new WebDriverWait(
                driver,
                TimeSpan.FromSeconds(10));

            // 2. Navigate to EPAM
            driver.Navigate().GoToUrl(WebsiteUrl);

            // 3. Accept cookies
            try
            {
                IWebElement acceptCookies = new WebDriverWait(
                    driver,
                    TimeSpan.FromSeconds(5))
                    .Until(driver =>
                        driver.FindElement(
                            By.Id("onetrust-accept-btn-handler")));

                acceptCookies.Click();
            }
            catch (WebDriverTimeoutException)
            {
                // Cookie popup was not displayed
            }

            // 4. Find Careers link and click
            IWebElement careersLink = wait.Until(driver =>
                driver.FindElement(
                    By.LinkText("Careers")));

            careersLink.Click();

            // 5. Click "Start Your Search Here"
            IWebElement startSearchLink = wait.Until(driver =>
                driver.FindElement(
                    By.PartialLinkText("START YOUR SEARCH")));

            startSearchLink.Click();

            // 6. Enter programming language
            IWebElement searchInput = wait.Until(driver =>
                driver.FindElement(
                    By.Name("search")));

            searchInput.Clear();
            searchInput.SendKeys(programmingLanguage);

            // 7. Select country
            IWebElement countryInput = wait.Until(driver =>
                driver.FindElement(
                    By.CssSelector(
                        "input[aria-label='Choose your country']")));

            countryInput.Clear();
            countryInput.SendKeys(country);

            // 8. Accept cookies again when redirected to the 
            try
            {
                IWebElement acceptCookies = new WebDriverWait(
                    driver,
                    TimeSpan.FromSeconds(3))
                    .Until(driver =>
                        driver.FindElement(
                            By.Id("onetrust-accept-btn-handler")));

                acceptCookies.Click();
            }
            catch (WebDriverTimeoutException)
            {
                // Cookie popup was not displayed
            }

            // 9. Select Remote
            IWebElement remoteCheckbox = wait.Until(driver =>
                driver.FindElement(
                    By.XPath(
                        "//input[contains(@name,'vacancy_type-Remote')]")));

            if (!remoteCheckbox.Selected)
            {
                IWebElement remoteLabel = wait.Until(driver =>
                    driver.FindElement(
                        By.XPath(
                            "//input[contains(@name,'vacancy_type-Remote')]/following-sibling::label")));

                remoteLabel.Click();
            }

            // 10. Click Search
            IWebElement searchButton = wait.Until(driver =>
                driver.FindElement(
                    By.Name("submit_search_box_button")));

            searchButton.Click();

            // 11. Wait until at least one result appears
            wait.Until(driver =>
                driver.FindElements(
                    By.XPath(
                        "//div[@data-testid='accordion-section-container']"))
                .Count > 0);

            // 12. Find the latest result
            IWebElement lastJob = wait.Until(driver =>
            {
                var jobs = driver.FindElements(
                    By.XPath(
                        "//div[@data-testid='accordion-section-container']"));

                return jobs.Count > 0
                    ? jobs.Last()
                    : null;
            });

            // 13. Expand the latest result
            wait.Until(driver =>
            {
                try
                {
                    lastJob.Click();
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    var jobs = driver.FindElements(
                        By.XPath(
                            "//div[@data-testid='accordion-section-container']"));

                    if (jobs.Count == 0)
                        return false;

                    lastJob = jobs.Last();

                    return false;
                }
            });

            // 14. Get text of the expanded latest result
            string jobDetails = wait.Until(driver =>
            {
                try
                {
                    var jobs = driver.FindElements(
                        By.XPath(
                            "//div[@data-testid='accordion-section-container']"));

                    if (jobs.Count == 0)
                        return null;

                    IWebElement latestJob = jobs.Last();

                    string text = latestJob.Text;

                    return string.IsNullOrWhiteSpace(text)
                        ? null
                        : text;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
            });

            // 15. Validate programming language
            Assert.Contains(
                programmingLanguage,
                jobDetails,
                StringComparison.OrdinalIgnoreCase);

            Console.WriteLine(
                $"Latest job contains: {programmingLanguage}");
        }
        finally
        {
            // 16. Always close the browser
            driver.Quit();
        }
    }

    [Theory]
    [InlineData("BLOCKCHAIN")]
    [InlineData("Cloud")]
    [InlineData("Automation")]
    public void GlobalSearch(string searchText)
    {
        IWebDriver driver = CreateDriver();

        try
        {
            // 1. Browser setup
            driver.Manage().Window.Maximize();

            // Implicit wait
            driver.Manage().Timeouts().ImplicitWait =
                TimeSpan.FromSeconds(5);

            // Explicit wait
            WebDriverWait wait = new WebDriverWait(
                driver,
                TimeSpan.FromSeconds(10));

            // 2. Navigate to EPAM
            driver.Navigate().GoToUrl(WebsiteUrl);

            // 3. Accept cookies
            try
            {
                IWebElement acceptCookies = new WebDriverWait(
                    driver,
                    TimeSpan.FromSeconds(5))
                    .Until(driver =>
                        driver.FindElement(
                            By.Id("onetrust-accept-btn-handler")));

                acceptCookies.Click();
            }
            catch (WebDriverTimeoutException)
            {
                // Cookie popup was not displayed
            }

            // 4. Find and click search icon
            IWebElement searchIcon = wait.Until(driver =>
                driver.FindElement(
                    By.ClassName("search-icon")));

            searchIcon.Click();

            // 5. Find global search input
            IWebElement searchInput = wait.Until(driver =>
                driver.FindElement(
                    By.Id("new_form_search")));

            searchInput.Clear();
            searchInput.SendKeys(searchText);

            // 6. Find and click Find button
            IWebElement findButton = wait.Until(driver =>
                driver.FindElement(
                    By.CssSelector(
                        "button.custom-search-button")));

            findButton.Click();

            // 7. Wait until at least one search result appears
            wait.Until(driver =>
                driver.FindElements(
                    By.CssSelector(
                        "article.search-results__item"))
                .Count > 0);

            wait.Until(driver =>
                driver.FindElements(
                    By.CssSelector(
                        "article.search-results__item:last-child"))
                .Count > 0);

            // 8. Scroll down to load all available results
            int previousCount = 0;

            while (true)
            {
                int currentCount = driver.FindElements(
                    By.CssSelector(
                        "article.search-results__item"))
                    .Count;

                ((IJavaScriptExecutor)driver).ExecuteScript(
                    "window.scrollTo(0, document.body.scrollHeight);");

                try
                {
                    wait.Until(driver =>
                        driver.FindElements(
                            By.CssSelector(
                                "article.search-results__item"))
                        .Count > currentCount);
                }
                catch (WebDriverTimeoutException)
                {
                    // No more results were loaded
                    break;
                }

                if (currentCount == previousCount)
                {
                    break;
                }

                previousCount = currentCount;
            }

            // 9. Collect all result links AFTER lazy loading
            var resultContainers = driver.FindElements(
                By.CssSelector(
                    "article.search-results__item"));

            IReadOnlyCollection<IWebElement> resultLinks = resultContainers
                .SelectMany(result =>
                    result.FindElements(By.TagName("a")))
                .Where(link =>
                    link.GetAttribute("class")?
                        .Contains("search-results__title-link") == true)
                .ToList();

            var firstResultTitle = resultContainers.First().FindElements(
                By.XPath(
                    ".//a[contains(@class, 'search-results__title-link')]") );

            // 10. Make sure results exist
            Assert.NotEmpty(resultLinks);
            Assert.NotEmpty(firstResultTitle);

            // 11. Validate all links using LINQ
            bool allResultsContainSearchText = resultLinks.All(link =>
                link.Text.Contains(
                    searchText,
                    StringComparison.OrdinalIgnoreCase));

            // 12. Assertion
            Assert.True(
                allResultsContainSearchText,
                $"Not all search result links contain '{searchText}'.");

            Console.WriteLine(
                $"All {resultLinks.Count} search result links contain '{searchText}'.");
        }
        finally
        {
            // 13. Always close the browser
            driver.Quit();
        }
    }
}