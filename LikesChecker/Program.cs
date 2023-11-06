using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using LikesChecker.Accounts;
using LikesChecker.SeleniumHelpers;
using NLog;
using ConfigurationManager = LikesChecker.Configuration.ConfigurationHelper;

namespace LikesChecker
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            var configuration = ConfigurationManager.BuildConfiguration();
            logger.Info("Application started.");
            
            string host = configuration.GetSection("ApplicationSettings")["HostUrl"];

            using (IWebDriver driver = new ChromeDriver(WebDriverHelper.SetupChromeOptions()))
            {
                driver.Navigate().GoToUrl(host);
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(100);
                List<Account> accounts = AccountManager.LoadAccounts(args);
                AccountManager.ProcessAccounts(driver, accounts);
            }
            
            CleanUp();
        }
        private static void CleanUp()
        {
            logger.Info("Application ended.");
            Process.GetCurrentProcess().CloseMainWindow();
            Environment.Exit(0);
        }
    }
}