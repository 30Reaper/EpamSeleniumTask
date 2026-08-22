using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace EpamSeleniumTask.Pages;

public sealed class InsightsPage : EpamPage
{
    private readonly By carousel = By.CssSelector("main .slider-ui-23");
    private readonly By nextButton = By.CssSelector("main .slider-ui-23:first-of-type button.slider__right-arrow");

    public InsightsPage(IWebDriver driver, string websiteUrl) : base(driver, websiteUrl)
    {
    }

    public void Open()
    {
        Driver.Navigate().GoToUrl($"{WebsiteUrl.TrimEnd('/')}/insights");
        AcceptCookies();
        Visible(carousel);
    }

    public void SwipeCarousel(int count)
    {
        for (int index = 0; index < count; index++)
        {
            Visible(nextButton).Click();
            Wait.Until(driver => driver.FindElements(By.CssSelector("main .slider-ui-23:first-of-type .owl-item.active")).Count > 0);
        }
    }

    public string GetActiveArticleTitle() => Normalize(Visible(By.CssSelector("main .slider-ui-23:first-of-type .owl-item.active .text-ui-23")).Text);

    public ArticlePage OpenActiveArticle()
    {
        Visible(By.CssSelector("main .slider-ui-23:first-of-type .owl-item.active a.slider-cta-link")).Click();
        return new ArticlePage(Driver, WebsiteUrl);
    }

    private static string Normalize(string value) => string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
