using System.Reflection;
using LikesChecker.Logging;
using LikesChecker.SeleniumHelpers;
using LikesChecker.Utilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenQA.Selenium;
using ConfigurationManager = LikesChecker.Configuration.ConfigurationHelper;

namespace LikesChecker.Accounts;

public class AccountManager
{
    private const string AccountFolderName = "Accounts";
    private const string AccountFileName = "accountInfo.txt";
    private static readonly string BasePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    private static readonly IConfiguration Configuration = ConfigurationManager.BuildConfiguration();
    private static readonly int PageLoadTimeout = int.Parse(Configuration.GetSection("ApplicationSettings")["PageLoadTimeoutSeconds"]);
    private static readonly int ElementWaitTimeout = int.Parse(Configuration.GetSection("ApplicationSettings")["ElementWaitTimeoutSeconds"]);

    public static List<Account> LoadAccounts(string[] args)
    {
        var accounts = new List<Account>();

        try
        {
            var accountDirectoryPath = Path.Combine(BasePath, AccountFolderName);

            if (args.Length > 0)
            {
                LoadSingleAccount(accounts, accountDirectoryPath, args[0]);
            }
            else
            {
                LoadAllAccounts(accounts, accountDirectoryPath);
            }
        }
        catch (Exception e)
        {
            LoggingHelper.LogException("Error loading accounts", e, "log.txt");
        }

        return accounts;
    }

    private static void LoadSingleAccount(ICollection<Account> accounts, string accountDirectoryPath, string accountName)
    {
        var accountInfoPath = Path.Combine(accountDirectoryPath, accountName, AccountFileName);
        if (File.Exists(accountInfoPath))
        {
            var account = DeserializeAccount(File.ReadAllText(accountInfoPath));
            if (account != null)
            {
                accounts.Add(account);
            }
        }
    }

    private static void LoadAllAccounts(ICollection<Account> accounts, string accountDirectoryPath)
    {
        var accountFolders = Directory.GetDirectories(accountDirectoryPath);
        foreach (var accountFolder in accountFolders)
        {
            var accountInfoFile = Path.Combine(accountFolder, AccountFileName);
            if (File.Exists(accountInfoFile))
            {
                var account = DeserializeAccount(File.ReadAllText(accountInfoFile));
                if (account != null)
                {
                    accounts.Add(account);
                }
            }
        }
    }

    public static void ProcessAccounts(IWebDriver driver, List<Account> accounts)
    {
        foreach (var account in accounts)
        {
            ProcessAccount(driver, account);
        }
    }
    
    private static void ProcessAccount(IWebDriver driver, Account account)
    {
        try
        {
            driver.Navigate().GoToUrl(account.Link);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(PageLoadTimeout);
            
            UpdateAccountLikesAndMessages(driver, account);
            UpdateAccountLikesWeekAndLastSimp(driver, account);
            
            SerializeAccount(account);
        }
        catch (Exception e)
        {
            LoggingHelper.LogException(account.Nickname + " Error", e, Path.Combine(BasePath, "log.txt"));
            account.Error = true;
        }
    }

    private static void UpdateAccountLikesAndMessages(IWebDriver driver, Account account)
    {
        string likesXPath = @"//*[@id='content']/div/div/div[3]/div[2]/div/div[2]/a[2]/div[1]";
        string messagesXPath = @"//*[@id='content']/div/div/div[3]/div[2]/div/div[2]/a[1]/div[1]";
        
        IWebElement Likes = WebDriverHelper.WaitForElement(driver, By.XPath(likesXPath), ElementWaitTimeout);
        IWebElement Messages = WebDriverHelper.WaitForElement(driver, By.XPath(messagesXPath), ElementWaitTimeout);

        account.Likes = Likes.Text;
        account.Messages = Messages.Text;
    }

    private static void UpdateAccountLikesWeekAndLastSimp(IWebDriver driver, Account account)
    {
        driver.Navigate().GoToUrl($"{account.Link}/likes");
        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(PageLoadTimeout);
        
        string lastSimpXPath = @"//*[@id='content']/div/div/div[3]/div[1]/div[1]/div[1]/abbr";
        string likesWeekClassName = "muted";
        
        IWebElement LastSimp = WebDriverHelper.WaitForElement(driver, By.XPath(lastSimpXPath), ElementWaitTimeout);
        IWebElement LikesWeek = WebDriverHelper.WaitForElement(driver, By.ClassName(likesWeekClassName), ElementWaitTimeout);

        double lastLike = double.Parse(LastSimp.GetAttribute("data-time"));
        account.LikesWeek = LikesWeek.Text.Split("- ")[1].Split(", за месяц")[0];
        account.LastSimp = TimeUtility.UnixTimeStampToDateTime(lastLike);
    }
    
    private static void SerializeAccount(Account account)
    {
        string accountInfoPath = Path.Combine(BasePath, "Accounts", account.Foldername, "accountInfo.txt");
        SerializationHelper.SerializeAndWriteToFile(accountInfoPath, account);
    }
    
    private static Account DeserializeAccount(string jsonData)
    {
        try
        {
            return JsonConvert.DeserializeObject<Account>(jsonData);
        }
        catch (JsonException e)
        {
            LoggingHelper.LogException("Deserialization error", e, "log.txt");
            return null;
        }
    }
}