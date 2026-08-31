using OpenQA.Selenium;

namespace EpamSeleniumTask.Pages;

public sealed class ServicePage : EpamPage
{
    public ServicePage(IWebDriver driver, string websiteUrl) : base(driver, websiteUrl)
    {
    }

    public string GetBreadcrumbTitle()
    {
        WaitForPageLoad();

        try
        {
            IWebElement crumb = Visible(By.CssSelector("nav.breadcrumbs-ui a[aria-current='page']"));
            return crumb.Text.Trim();
        }
        catch (WebDriverTimeoutException)
        {
            IWebElement last = Visible(By.XPath("//nav[contains(@class,'breadcrumbs-ui')]//ol/li[last()]/a"));
            return last.Text.Trim();
        }
    }

    public string GetTitle()
    {
        WaitForPageLoad();

        try
        {
            return Visible(By.CssSelector("h1")).Text;
        }
        catch (WebDriverTimeoutException)
        {
            try
            {
                return Visible(By.XPath("//span[contains(@class,'museo-sans-500') or contains(@class,'gradient-text')]")).Text;
            }
            catch (WebDriverTimeoutException)
            {
                return string.Empty;
            }
        }
    }

    public bool IsOurRelatedExpertiseSectionVisible()
    {
        try
        {
            WaitForPageLoad();

            try
            {
                IWebElement element = Visible(By.XPath("//span[contains(@class,'museo-sans-light') and contains(normalize-space(.),'Our Related Expertise')]"));
                return element is not null;
            }
            catch (WebDriverTimeoutException)
            {
                IWebElement element = Visible(By.XPath("//*[contains(translate(normalize-space(.),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'our related expertise')]") );
                return element is not null;
            }
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
}
