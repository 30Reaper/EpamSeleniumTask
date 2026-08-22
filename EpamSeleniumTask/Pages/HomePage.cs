using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace EpamSeleniumTask.Pages;

public sealed class HomePage : EpamPage
{
    public HomePage(IWebDriver driver, string websiteUrl) : base(driver, websiteUrl)
    {
    }

    public void Open()
    {
        Driver.Navigate().GoToUrl(WebsiteUrl);
        AcceptCookies();
    }

    public void OpenCodeOfEthicalConduct()
    {
        new Actions(Driver).ScrollByAmount(0, 1200).Perform();
        IWebElement link = Visible(By.XPath("//footer//*[self::a][contains(normalize-space(.), 'Code of Ethical Conduct (PDF)')]") );
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", link);
        link.Click();
    }
}
