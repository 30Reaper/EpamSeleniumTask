using System;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using Xunit;
using EpamSeleniumTask.Pages;
using EpamSeleniumTask.Tests.Support;

namespace EpamSeleniumTask.Tests.StepDefinitions;

[Binding]
public sealed class ServicesSteps
{
    private IConfiguration _configuration;
    private HomePage? _homePage;
    private ServicePage? _servicePage;

    public ServicesSteps()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
    }

    private string WebsiteUrl => _configuration["WebsiteUrl"]!;

    [Given("I open the Epam home page")]
    public void GivenIOpenTheEpamHomePage()
    {
        IWebDriver driver = DriverFactory.Current!;
        _homePage = new HomePage(driver, WebsiteUrl);
        _homePage.Open();
    }

    [When("I open Services menu and select \"(.*)\"")]
    public void WhenIOpenServicesMenuAndSelect(string category)
    {
        _homePage!.OpenServicesMenuAndSelectCategory(category);
        _servicePage = new ServicePage(DriverFactory.Current!, WebsiteUrl);
    }

    [Then("the page title contains \"(.*)\"")]
    public void ThenThePageTitleContains(string expected)
    {
        string title = _servicePage!.GetBreadcrumbTitle();
        Assert.Contains(expected, title, StringComparison.OrdinalIgnoreCase);
    }

    [Then("the 'Our Related Expertise' section is displayed")]
    public void ThenTheOurRelatedExpertiseSectionIsDisplayed()
    {
        bool visible = _servicePage!.IsOurRelatedExpertiseSectionVisible();
        Assert.True(visible, "'Our Related Expertise' section is not visible on the page.");
    }
}
