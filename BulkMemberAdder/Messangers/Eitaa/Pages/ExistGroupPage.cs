using BulkMemberAdder.Domain;
using BulkMemberAdder.Utility;
using OpenQA.Selenium;
using Spectre.Console;

namespace BulkMemberAdder.Messangers.Eitaa.Pages
{
    public class ExistGroupPage
    {
        private readonly IWebDriver _driver;

        public ExistGroupPage(IWebDriver driver)
        {
            _driver = driver;
        }


        public bool IsSearchBtnExist => new SeleniumUtils(_driver).IsElementPresent(By.CssSelector(".input-field-input"));
        public IWebElement SearchBtn => _driver.FindElement(By.CssSelector(".input-field-input"));

        public bool IsGroupBtnExist => new SeleniumUtils(_driver).IsElementPresent(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div/div[2]/div[2]/div[3]/div/div/div[1]/div/div/ul/li[1]"));
        public IWebElement GroupBtn => _driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div/div[2]/div[2]/div[3]/div/div/div[1]/div/div/ul/li[1]"));

        public IWebElement ProfileBarBtn => _driver.FindElement(By.CssSelector("div.sidebar-header:nth-child(2)"));

        public bool IsContactPageAddBtnExist => new SeleniumUtils(_driver).IsElementPresent(By.XPath("/html/body/div[2]/div[1]/div[3]/div/div/div[2]/button"));
        public IWebElement ContactPageAddBtn => _driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[3]/div/div/div[2]/button"));

        public bool IsSearchContactTxtExist => new SeleniumUtils(_driver).IsElementPresent(By.XPath("/html/body/div[2]/div[1]/div[3]/div/div[2]/div[2]/div/div[1]/div/div/input"));
        public IWebElement SearchContactTxt => _driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[3]/div/div[2]/div[2]/div/div[1]/div/div/input"));

        public bool IsContactAddBtnExist => new SeleniumUtils(_driver).IsElementPresent(By.CssSelector("div.chatlist-container:nth-child(3) > div:nth-child(1) > ul:nth-child(1) > li:nth-child(1)"));
        public IWebElement ContactAddBtn => _driver.FindElement(By.CssSelector("div.chatlist-container:nth-child(3) > div:nth-child(1) > ul:nth-child(1) > li:nth-child(1)"));

        public IWebElement SaveContacts => _driver.FindElement(By.CssSelector("button.btn-circle:nth-child(1)"));
        public IWebElement ConfirmSaveContactsBtn => _driver.FindElement(By.CssSelector("button.btn:nth-child(1)"));


        public void BulkImportToExistGroup(List<Member> members)
        {
            try
            {
                AnsiConsole.Markup("Trying to import to exist group...\n");

                _driver.Navigate().Refresh();

                // open serach bar
                while (!IsSearchBtnExist)
                    Thread.Sleep(1);
                SearchBtn.Click();

                var groupName = AnsiConsole.Ask<string>("What's The [green]Exist Group name[/]?");
                SearchBtn.SendKeys(groupName);

                // open group bar
                Thread.Sleep(1000);
                if (!IsGroupBtnExist)
                {
                    // check if group bar exist
                    if (AnsiConsole.Confirm($"\nDidn't find group {groupName} do you want to try again?"))
                    {
                        BulkImportToExistGroup(members);
                        return;
                    }

                    return;
                }
                GroupBtn.Click();

                // open group profile
                ProfileBarBtn.Click();

                // select contacts
                AnsiConsole.Status()
                    .Start("Start Importing to group...", ctx =>
                    {
                        var counter = 0;
                        var totallCount = members.DistinctBy(x => x.Mobile).Count();

                        foreach (var member in members.DistinctBy(x => x.Mobile).Where(x => x.Mobile != string.Empty))
                        {
                            AnsiConsole.MarkupLine($"import {member.Mobile}...");

                            try
                            {
                                // open contact add page
                                try
                                {
                                    Thread.Sleep(100);
                                    if (IsContactPageAddBtnExist)
                                        ContactPageAddBtn.Click();
                                }
                                catch { }


                                while (!IsSearchContactTxtExist)
                                {
                                    Thread.Sleep(1);
                                    try{ ContactPageAddBtn.Click(); }
                                    catch { }
                                }
                                Thread.Sleep(1000);
                                SearchContactTxt.Clear();
                                SearchContactTxt.SendKeys($"+98{member.Mobile.Remove(0, 1)}");

                                Thread.Sleep(1000);
                                if (!IsContactAddBtnExist)
                                {
                                    AnsiConsole.MarkupLine("[red]Contact doesn't Exist.[/]");
                                    continue;
                                }

                                // select contact
                                ContactAddBtn.Click();

                                // save contacts to group
                                Thread.Sleep(1000);
                                SaveContacts.Click();

                                // confirm pop up
                                Thread.Sleep(1000);
                                ConfirmSaveContactsBtn.Click();

                                counter++;
                                ctx.Status($"{counter} Contact of {totallCount} imported successfully");
                                AnsiConsole.MarkupLine("[green]Successfully.[/]");
                            }
                            catch (Exception ex)
                            {
                                AnsiConsole.MarkupLine("[red]Error.[/]");
                                //AnsiConsole.WriteException(ex);
                            }
                        }
                    });

                AnsiConsole.Markup("* [green][[bulk import contacts to exist group finished!]][/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup("* [red][[ERROR while import to exist group]][/]");
                AnsiConsole.WriteException(ex);

                if (AnsiConsole.Confirm("\n Wanna try again?"))
                    BulkImportToExistGroup(members);

                throw;
            }
        }
    }
}
