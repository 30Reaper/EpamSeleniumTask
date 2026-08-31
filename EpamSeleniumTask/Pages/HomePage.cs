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

    public void OpenServicesMenuAndSelectCategory(string category)
    {
        IWebElement servicesLink = Visible(By.XPath("//a[contains(@class,'top-navigation__item-link') and normalize-space()='Services']"));
        new Actions(Driver).MoveToElement(servicesLink).Perform();

        try
        {
            IWebElement parentLi = servicesLink.FindElement(By.XPath("ancestor::li[contains(@class,'top-navigation__item')]"));
            new Actions(Driver).MoveToElement(parentLi).Perform();
        }
        catch { /* ignore if structure differs */ }

        By categoryLocator = By.XPath($"//a[contains(@class,'top-navigation__sub-link') and normalize-space()=\"{category}\"]");
        IWebElement categoryLink = Visible(categoryLocator);
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", categoryLink);
        categoryLink.Click();
    }
}
