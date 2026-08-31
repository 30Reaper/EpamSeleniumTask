using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EpamSeleniumTask.Pages;

public abstract class EpamPage
{
    protected EpamPage(IWebDriver driver, string websiteUrl)
    {
        Driver = driver;
        WebsiteUrl = websiteUrl;
        Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    protected IWebDriver Driver { get; }
    protected string WebsiteUrl { get; }
    protected WebDriverWait Wait { get; }

    protected void AcceptCookies()
    {
        try
        {
            Wait.Until(driver =>
            {
                IWebElement? button = driver.FindElements(By.Id("onetrust-accept-btn-handler")).FirstOrDefault();
                return button is not null && button.Displayed ? button : null;
            }).Click();
        }
        catch (WebDriverTimeoutException)
        {
            // If the cookie consent button is not found, it may have already been accepted or not present; ignore
        }
    }

    protected IWebElement Visible(By locator) => Wait.Until(driver =>
    {
        IWebElement? element = driver.FindElements(locator).FirstOrDefault();
        return element is not null && element.Displayed ? element : null;
    });

    protected IWebElement Present(By locator) => Wait.Until(driver =>
        driver.FindElements(locator).FirstOrDefault());

    protected void WaitForPageLoad()
    {
        try
        {
            Wait.Until(driver =>
            {
                try
                {
                    var js = (IJavaScriptExecutor)driver;
                    string ready = js.ExecuteScript("return document.readyState")?.ToString() ?? string.Empty;
                    return string.Equals(ready, "complete", StringComparison.OrdinalIgnoreCase);
                }
                catch
                {
                    return false;
                }
            });
        }
        catch (WebDriverTimeoutException)
        {
            // Ignore timeout; page may still be partially loaded
        }
    }
}
