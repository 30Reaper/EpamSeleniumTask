using OpenQA.Selenium;
using Serilog;

namespace EpamSeleniumTask.Core;

public static class ScreenshotHelper
{
    public static string Capture(IWebDriver driver, string directory, string name)
    {
        try
        {
            Directory.CreateDirectory(directory);

            var tsDriver = driver as ITakesScreenshot;
            if (tsDriver is null)
            {
                Log.Warning("Driver does not support screenshots.");
                return string.Empty;
            }

            var screenshot = tsDriver.GetScreenshot();

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidChar, '_');
            }

            string fileName = Path.Combine(
                directory,
                $"{name}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.png");

            File.WriteAllBytes(fileName, screenshot.AsByteArray);

            Log.Information("Saved screenshot to {File}", fileName);

            return fileName;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to capture screenshot.");
            return string.Empty;
        }
    }
}
