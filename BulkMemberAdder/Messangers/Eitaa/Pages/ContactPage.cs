using BulkMemberAdder.Domain;
using BulkMemberAdder.Utility;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Spectre.Console;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace BulkMemberAdder.Messangers.Eitaa.Pages
{
    public class ContactPage
    {
        private readonly IWebDriver _driver;

        public ContactPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public IWebElement BurgerBtn => _driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div/div[1]/div[1]/div[2]"));
        public IWebElement ContactsBtn => _driver.FindElement(By.CssSelector(".tgico-user"));

        public bool IsContactBtnExist => new SeleniumUtils(_driver).IsElementPresent(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div[2]/div[2]/button"));
        public IWebElement AddContactBtn => _driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div[2]/div[2]/button"));

        public bool IsFirstNameExist => new SeleniumUtils(_driver).IsElementPresent(By.XPath("/html/body/div[5]/div/div[2]/div[1]/div[1]"));
        public IWebElement FirstNameTxt => _driver.FindElement(By.XPath("/html/body/div[5]/div/div[2]/div[1]/div[1]"));
        public IWebElement LasttNameTxt => _driver.FindElement(By.XPath("/html/body/div[5]/div/div[2]/div[2]/div[1]"));
        public IWebElement MobileTxt => _driver.FindElement(By.XPath("/html/body/div[5]/div/div[3]/div[1]"));

        public IWebElement SaveBtn => _driver.FindElement(By.CssSelector("button.btn-primary:nth-child(3)"));
        public bool IsSaveBtnExist => new SeleniumUtils(_driver).IsElementPresent(By.CssSelector("button.btn-primary:nth-child(3)"));

        public void BulkImportContacts(List<Member> members)
        {
            try
            {
                AnsiConsole.Markup("Trying to import contacts...\n");

                while (!BurgerBtn.Displayed)
                    Thread.Sleep(1);
                BurgerBtn.Click();

                while (!ContactsBtn.Displayed)
                    Thread.Sleep(1);
                ContactsBtn.Click();

                Thread.Sleep(2000);

                Console.OutputEncoding = Encoding.UTF8;

                var table = new Table();
                table.AddColumn("FirstName");
                table.AddColumn(new TableColumn("LastName"));
                table.AddColumn(new TableColumn("Mobile").Centered());
                table.AddColumn(new TableColumn("IsConfirmed").Centered());

                AnsiConsole.Live(table)
                    .Start(ctx =>
                    {
                        foreach (var member in members.Take(10))
                        {
                            OpenAddContactPopUp();

                            while (!IsFirstNameExist)
                                Thread.Sleep(1);

                            try
                            {
                                FirstNameTxt.Clear();
                                FirstNameTxt.SendKeys(member.FirstName);

                                LasttNameTxt.Clear();
                                LasttNameTxt.SendKeys(member.LastName);

                                MobileTxt.Clear();
                                MobileTxt.SendKeys($"+98{member.Mobile.Remove(0, 1)}");

                                SaveBtn.Click();
                            }
                            catch(Exception ex)
                            { }

                            if (!IsFirstNameExist)
                                table.AddRow(member.FirstName, member.LastName, member.Mobile, Emoji.Known.CheckMarkButton);
                            else
                                table.AddRow(member.FirstName, member.LastName, member.Mobile, Emoji.Known.Warning);

                            ctx.Refresh();
                        }
                    });

                AnsiConsole.Markup("* [green][[Import Contacts finished]][/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup("* [red][[ERROR while import contacts]][/]");
                AnsiConsole.WriteException(ex);

                if (AnsiConsole.Confirm("\n Wanna try again?"))
                    BulkImportContacts(members);

                throw;
            }
        }

        private void OpenAddContactPopUp()
        {
            try
            {
                if (IsContactBtnExist)
                    AddContactBtn.Click();
            }
            catch { }
        }
    }
}
