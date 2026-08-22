using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System.Linq;
using System.Collections.Generic;

namespace EpamSeleniumTask.Pages;

public sealed class SearchResultsPage : EpamPage
{
    public SearchResultsPage(IWebDriver driver, string websiteUrl) : base(driver, websiteUrl)
    {
    }

    public IReadOnlyCollection<string> Search(string searchText)
    {
        Driver.Navigate().GoToUrl(WebsiteUrl);
        AcceptCookies();
        Visible(By.ClassName("search-icon")).Click();
        Visible(By.Id("new_form_search")).SendKeys(searchText);
        Visible(By.CssSelector("button.custom-search-button")).Click();

        By results = By.CssSelector("article.search-results__item");
        Wait.Until(driver => driver.FindElements(results).Count > 0);

        int unchangedPasses = 0;
        int previousCount = 0;
        while (unchangedPasses < 2)
        {
            int currentCount = Driver.FindElements(results).Count;
            new Actions(Driver).ScrollByAmount(0, 900).Perform();
            try
            {
                Wait.Until(driver => driver.FindElements(results).Count > currentCount);
                unchangedPasses = 0;
            }
            catch (WebDriverTimeoutException)
            {
                unchangedPasses++;
            }

            if (currentCount == previousCount)
            {
                unchangedPasses++;
            }

            previousCount = currentCount;
        }

        return Driver.FindElements(By.CssSelector("article.search-results__item a.search-results__title-link"))
            .Select(link => link.Text)
            .Where(text => !string.IsNullOrWhiteSpace(text))
            .ToList();
    }
}
