using Bogus.DataSets;
using BulkMemberAdder.Utility;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Spectre.Console;
using System.Threading;

namespace BulkMemberAdder.Messangers.Eitaa.Pages
{
    public class SignPage
    {
        private readonly IWebDriver _driver;
        private const string baseUrl = "https://web.eitaa.com";

        public SignPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement LoginButton => _driver.FindElement(By.XPath("//*[@id=\"auth-pages\"]/div/div[2]/div[1]/div/div[3]/button"));
        public bool IsPhoneFieldExist => new SeleniumUtils(_driver).IsElementPresent(By.XPath("//*[@id=\"auth-pages\"]/div/div[2]/div[1]/div/div[3]/div[2]/div[1]"));
        public IWebElement PhoneField => _driver.FindElement(By.XPath("//*[@id=\"auth-pages\"]/div/div[2]/div[1]/div/div[3]/div[2]/div[1]"));
        public IWebElement IsPhoneValid => _driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div[1]/div/div[3]/div[2]/label/span"));


        public IWebElement OTPCode => _driver.FindElement(By.XPath("//*[@id=\"auth-pages\"]/div/div[2]/div[3]/div/div[3]/div/input"));
        public IWebElement IsOTPValid => _driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div[3]/div/div[3]/div/label/span"));

        public IWebElement RegisterName => _driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div[5]/div/div[2]/div[1]/div[1]"));
        public IWebElement RegisterButton => _driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/div[5]/div/div[2]/button"));

        public void LoginOrRegister()
        {
            try
            {
                AnsiConsole.Markup("Trying to Login...\n");

                _driver.Navigate().GoToUrl(baseUrl);

                Console.Clear();

                Login();

                ValidateOTP();

                Thread.Sleep(3000);
                Register();
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup("* [red][[ERROR while Login or Register]][/]");
                AnsiConsole.WriteException(ex);

                if (AnsiConsole.Confirm("\n Wanna try again?"))
                    Login();

                throw;
            }
        }

        private void Login()
        {
            var phoneNumber = AnsiConsole.Ask<string>("What's your [green]phone number[/] for login in Eitaa? +98 ");

            while (!IsPhoneFieldExist)
                Thread.Sleep(1);
            PhoneField.Clear();
            PhoneField.SendKeys($"+98{phoneNumber}");

            Thread.Sleep(1000);
            LoginButton.Click();

            Thread.Sleep(2000);
            if (IsPhoneValid.Text == "شماره تلفن نامعتبر است")
            {
                AnsiConsole.Markup("* [red][[phone number is wrong try again]][/]");
                Login();
            }
        }

        private void ValidateOTP()
        {
            var otpCode = AnsiConsole.Ask<string>("What's The [green]OTP Code[/] Sent you in Eitaa or sms?");
            while (!OTPCode.Displayed)
                Thread.Sleep(1);
            OTPCode.Clear();
            OTPCode.SendKeys(otpCode);

            Thread.Sleep(2000);
            if (IsOTPValid.Text == "کد نامعتبر است" || otpCode.Length < 4)
            {
                AnsiConsole.Markup("\n* [red][[OTP code is wrong try again]][/]");
                ValidateOTP();
            }
        }

        private void Register()
        {
            try
            {
                if (!RegisterName.Displayed)
                    return;

                RegisterName.Clear();
                RegisterName.SendKeys("admin");

                RegisterButton.Click();
            }
            catch(OpenQA.Selenium.NoSuchElementException ex)
            { }
        }
    }
}
