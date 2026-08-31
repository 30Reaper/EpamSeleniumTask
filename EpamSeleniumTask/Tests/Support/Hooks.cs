using TechTalk.SpecFlow;

namespace EpamSeleniumTask.Tests.Support;

[Binding]
public sealed class Hooks
{
    [BeforeScenario]
    public void BeforeScenario()
    {
        DriverFactory.Create();
    }

    [AfterScenario]
    public void AfterScenario()
    {
        DriverFactory.Quit();
    }
}
