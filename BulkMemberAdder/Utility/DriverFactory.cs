using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using Spectre.Console;

namespace BulkMemberAdder.Utility;

public enum DriverToUse
{
    InternetExplorer,
    Chrome,
    Firefox
}

public class DriverFactory
{
    public DriverFactory()
    {
        DownloadDriver();
    }

    private static FirefoxOptions FirefoxOptions
    {
        get
        {
            var firefoxProfile = new FirefoxOptions();
            firefoxProfile.SetPreference("network.automatic-ntlm-auth.trusted-uris", "http://localhost");
            return firefoxProfile;
        }
    }

    public IWebDriver Create(DriverToUse driverToUse, int ImplicitWait = 0, int PageLoad = 0)
    {
        IWebDriver driver;

        switch (driverToUse)
        {
            case DriverToUse.InternetExplorer:
                driver = new InternetExplorerDriver(AppDomain.CurrentDomain.BaseDirectory, new InternetExplorerOptions(), TimeSpan.FromMinutes(5));
                break;
            case DriverToUse.Firefox:
                var firefoxProfile = FirefoxOptions;
                driver = new FirefoxDriver(firefoxProfile);
                driver.Manage().Window.Maximize();
                break;
            case DriverToUse.Chrome:
                ChromeDriverService service = ChromeDriverService.CreateDefaultService();
                service.SuppressInitialDiagnosticInformation = true;
                ChromeOptions options = new ChromeOptions();
                options.SetLoggingPreference(LogType.Driver, LogLevel.Off);
                options.AddArgument("--log-level=3");
                driver = new ChromeDriver(service, options);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

#if DEBUG
        driver.Manage().Window.Maximize();
#else
        driver.Manage().Window.Minimize();
#endif

        var timeouts = driver.Manage().Timeouts();

        //timeouts.ImplicitWait = TimeSpan.FromSeconds(ImplicitWait);
        //timeouts.PageLoad = TimeSpan.FromSeconds(PageLoad);

        //// Load the saved cookies
        //cookies = LoadCookies();

        //// Add the cookies to the driver
        //foreach (var cookie in cookies)
        //{
        //    driver.Manage().Cookies.AddCookie(cookie);
        //}

        // Suppress the onbeforeunload event first. This prevents the application hanging on a dialog box that does not close.
        ((IJavaScriptExecutor)driver).ExecuteScript("window.onbeforeunload = function(e){};");
        return driver;
    }

    // Download chrom driver if not exist
    public void DownloadDriver()
    {
        var current = Path.Combine(Directory.GetCurrentDirectory(), "chromedriver.exe");

        try
        {
            // check if exist
            if (File.Exists(current))
                return;

            // Synchronous
            AnsiConsole.Status()
                .Start("Downloading Chrome driver...", ctx =>
                {

                    string zipPath = Path.Combine(Directory.GetCurrentDirectory(), "chromedriver_win32.zip");

                    // download
                    string path = "https://chromedriver.storage.googleapis.com/94.0.4606.41/chromedriver_win32.zip";
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFile(path, zipPath);
                        client.Dispose();
                    }

                    // Update the status and spinner
                    ctx.Status("Start Unzip chrome driver...");
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    // unzip
                    string extractPath = Directory.GetCurrentDirectory();
                    System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractPath);

                    // remove zip
                    File.Delete(zipPath);
                });
        }
        catch (Exception ex)
        {
            AnsiConsole.Markup($"* [red][[ERROR while Downloading chrome driver]] " +
                $"download chrome driver manually and move to {current} [/]");
            AnsiConsole.WriteException(ex);
        }
    }
}