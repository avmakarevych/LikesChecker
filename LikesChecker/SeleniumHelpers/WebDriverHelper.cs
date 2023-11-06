using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace LikesChecker.SeleniumHelpers;

public class WebDriverHelper
{
    public static IWebElement WaitForElement(IWebDriver driver, By by, int timeoutInSeconds)
    {
        if (timeoutInSeconds > 0)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            wait.Until(drv => drv.FindElement(by));
        }
        return driver.FindElement(by);
    }
    
    public static ChromeOptions SetupChromeOptions()
    {
        var manager = new DriverManager();
        manager.SetUpDriver(new ChromeConfig());
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("window-size=400,900");
        options.AddArgument("--blink-settings=imagesEnabled=false");
        options.AddExcludedArgument("enable-logging");
        return options;
    }
}