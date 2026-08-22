using OpenQA.Selenium;

namespace EpamSeleniumTask.Pages;

public sealed class ArticlePage : EpamPage
{
    public ArticlePage(IWebDriver driver, string websiteUrl) : base(driver, websiteUrl)
    {
    }

    public string GetTitle() => Normalize(Visible(By.CssSelector("main h1, main [class*='title'] h1")).Text);

    private static string Normalize(string value) => string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
}
