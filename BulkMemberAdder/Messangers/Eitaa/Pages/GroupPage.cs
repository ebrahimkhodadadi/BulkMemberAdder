
using BulkMemberAdder.Domain;
using BulkMemberAdder.Utility;
using OpenQA.Selenium;
using Spectre.Console;

namespace BulkMemberAdder.Messangers.Eitaa.Pages
{
    public class GroupPage
    {
        private readonly IWebDriver _driver;

        public GroupPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public bool IsMenuBtnExist => new SeleniumUtils(_driver).IsElementPresent(By.CssSelector("#new-menu"));
        public IWebElement MenuBtn => _driver.FindElement(By.CssSelector("#new-menu"));

        public IWebElement NewChannelBtn => _driver.FindElement(By.CssSelector(".tgico-newchannel"));

        public bool IsSearchContactTxtExist => new SeleniumUtils(_driver).IsElementPresent(By.CssSelector(".selector-search-input"));
        public IWebElement SearchContactTxt => _driver.FindElement(By.CssSelector(".selector-search-input"));

        //public bool IsContactAddBtnExist => new SeleniumUtils(_driver).IsElementPresent(By.CssSelector("div.chatlist-container:nth-child(3) > div:nth-child(1) > ul:nth-child(1) > li:nth-child(1)"));
        //public IWebElement ContactAddBtn => _driver.FindElement(By.CssSelector("div.chatlist-container:nth-child(3) > div:nth-child(1) > ul:nth-child(1) > li:nth-child(1)"));
                
        public bool IsContactAddBtnExist => new SeleniumUtils(_driver).IsElementPresent(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div[2]/div[2]/div/div[2]/div/ul/li[1]"));
        public IWebElement ContactAddBtn => _driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div[2]/div[2]/div/div[2]/div/ul/li[1]"));

        public IWebElement SaveContacts => _driver.FindElement(By.CssSelector("button.btn-circle:nth-child(1)"));

        public bool IsChannelNameTxtExist => new SeleniumUtils(_driver).IsElementPresent(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div[2]/div[2]/div/div[2]/div[1]/div[1]"));
        public IWebElement ChannelNameTxt => _driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div[2]/div[2]/div/div[2]/div[1]/div[1]"));
        public bool IsCreateChannelBtnExist => new SeleniumUtils(_driver).IsElementPresent(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div[2]/div[2]/button"));
        public IWebElement CreateChannelBtn => _driver.FindElement(By.XPath("/html/body/div[2]/div[1]/div[1]/div/div[2]/div[2]/button"));

        public void BulkImportToChannel(List<Member> members)
        {
            try
            {
                AnsiConsole.Markup("Trying to Create channel...\n");

                _driver.Navigate().Refresh();

                // click and open menu
                while (!IsMenuBtnExist)
                    Thread.Sleep(1);
                MenuBtn.Click();

                // open new group page
                Thread.Sleep(1000);
                NewChannelBtn.Click();

                var groupName = AnsiConsole.Ask<string>("What's The [green]channel[/] (channel will be created)?");

                // set group name
                while (!IsChannelNameTxtExist)
                    Thread.Sleep(1);
                ChannelNameTxt.Clear();
                ChannelNameTxt.SendKeys(groupName);

                // create channel
                while (!IsCreateChannelBtnExist)
                    Thread.Sleep(1);
                CreateChannelBtn.Click();

                while (!IsSearchContactTxtExist)
                    Thread.Sleep(1);

                // select contacts
                AnsiConsole.Status()
                    .Start("Start Importing to group...", ctx =>
                    {
                        foreach (var member in members.DistinctBy(x => x.Mobile))
                        {
                            AnsiConsole.MarkupLine($"import {member.Mobile}...");

                            try
                            {
                                SearchContactTxt.Clear();
                                SearchContactTxt.SendKeys($"+98{member.Mobile.Remove(0, 1)}");

                                Thread.Sleep(100);
                                if (!IsContactAddBtnExist)
                                    AnsiConsole.MarkupLine("[red]Contact doesn't Exist.[/]");

                                ContactAddBtn.Click();

                                AnsiConsole.MarkupLine("[green]Successfully.[/]");
                            }
                            catch (Exception ex)
                            {
                                AnsiConsole.MarkupLine("[red]Error.[/]");
                            }
                        }
                    });

                // click to go create group
                SaveContacts.Click();

                AnsiConsole.Markup("* [green][[Create group with contacts finished!]][/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup("* [red][[ERROR while create group]][/]");
                AnsiConsole.WriteException(ex);

                if (AnsiConsole.Confirm("\n Wanna try again?"))
                    BulkImportToChannel(members);

                throw;
            }
        }
    }
}
