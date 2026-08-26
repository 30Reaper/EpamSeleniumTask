using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Microsoft.Extensions.Logging;

namespace EpamSeleniumTask.Business.Pages;

public abstract class EpamPage
{
    protected EpamPage(IWebDriver driver, string websiteUrl, ILogger logger)
    {
        Driver = driver;
        WebsiteUrl = websiteUrl;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        Logger = logger;
    }

    protected IWebDriver Driver { get; }
    protected string WebsiteUrl { get; }
    protected WebDriverWait Wait { get; }
    protected ILogger Logger { get; }

    protected void AcceptCookies()
    {
        try
        {
            Logger.LogDebug("Attempting to accept cookies");
            Wait.Until(driver =>
            {
                IWebElement? button = driver.FindElements(By.Id("onetrust-accept-btn-handler")).FirstOrDefault();
                return button is not null && button.Displayed ? button : null;
            }).Click();
            Logger.LogDebug("Accepted cookies if present");
        }
        catch (WebDriverTimeoutException)
        {
            Logger.LogDebug("No cookies dialog found to accept");
        }
    }

    protected IWebElement Visible(By locator) => Wait.Until(driver =>
    {
        IWebElement? element = driver.FindElements(locator).FirstOrDefault();
        return element is not null && element.Displayed ? element : null;
    });

    protected IWebElement Present(By locator) => Wait.Until(driver =>
        driver.FindElements(locator).FirstOrDefault());
}
