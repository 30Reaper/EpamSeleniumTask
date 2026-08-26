using OpenQA.Selenium;
using Microsoft.Extensions.Logging;

namespace EpamSeleniumTask.Business.Pages;

public sealed class JobSearchPage : EpamPage
{
    public JobSearchPage(IWebDriver driver, string websiteUrl, ILogger logger) : base(driver, websiteUrl, logger)
    {
    }

    public string Search(string programmingLanguage, string country)
    {
        Logger.LogInformation("Searching jobs for {Language} in {Country}", programmingLanguage, country);
        Driver.Navigate().GoToUrl(WebsiteUrl);
        AcceptCookies();
        Visible(By.LinkText("Careers")).Click();
        Visible(By.PartialLinkText("START YOUR SEARCH")).Click();
        Visible(By.Name("search")).SendKeys(programmingLanguage);

        IWebElement countryInput = Visible(By.CssSelector("input[aria-label='Choose your country']"));
        countryInput.SendKeys(country);
        AcceptCookies();

        var remoteCheckboxLocator = By.XPath("//input[contains(@name,'vacancy_type-Remote')]");
        var remoteLabelLocator = By.XPath("//input[contains(@name,'vacancy_type-Remote')]/following-sibling::label");

        Wait.Until(driver =>
        {
            var cb = driver.FindElements(remoteCheckboxLocator).FirstOrDefault();
            if (cb is null)
                return false;

            try
            {
                if (!cb.Selected)
                {
                    var lbl = driver.FindElements(remoteLabelLocator).FirstOrDefault();
                    if (lbl is null || !lbl.Displayed)
                        return false;

                    lbl.Click();
                }

                return true;
            }
            catch (OpenQA.Selenium.StaleElementReferenceException)
            {
                return false;
            }
        });

        Visible(By.Name("submit_search_box_button")).Click();
        Logger.LogInformation("Submitted job search");
        By jobs = By.XPath("//div[@data-testid='accordion-section-container']");
        Wait.Until(driver => driver.FindElements(jobs).Count > 0);

        Wait.Until(driver =>
        {
            var elements = driver.FindElements(jobs);
            var last = elements.LastOrDefault();
            if (last is null || !last.Displayed)
                return false;

            try
            {
                last.Click();
                return true;
            }
            catch (OpenQA.Selenium.StaleElementReferenceException)
            {
                return false;
            }
        });

        return Wait.Until(driver =>
        {
            var elements = driver.FindElements(jobs);
            var last = elements.LastOrDefault();
            var text = last?.Text;
            Logger.LogInformation("Retrieved job details");
            return !string.IsNullOrEmpty(text) ? text : null;
        });
    }
}
