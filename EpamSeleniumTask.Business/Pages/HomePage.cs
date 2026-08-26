using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Microsoft.Extensions.Logging;

namespace EpamSeleniumTask.Business.Pages;

public sealed class HomePage : EpamPage
{
    public HomePage(IWebDriver driver, string websiteUrl, ILogger logger) : base(driver, websiteUrl, logger)
    {
    }

    public void Open()
    {
        Logger.LogInformation("Navigating to {Url}", WebsiteUrl);
        Driver.Navigate().GoToUrl(WebsiteUrl);
        AcceptCookies();
    }

    public void OpenCodeOfEthicalConduct()
    {
        Logger.LogInformation("Opening Code of Ethical Conduct link");
        new Actions(Driver).ScrollByAmount(0, 1200).Perform();
        IWebElement link = Visible(By.XPath("//footer//*[self::a][contains(normalize-space(.), 'Code of Ethical Conduct (PDF)')]"));
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", link);
        link.Click();
    }
}
