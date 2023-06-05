
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

        public void BulkImportToGroup(List<Member> members)
        {
            try
            {
                AnsiConsole.Markup("Trying to Create group...\n");

                _driver.Navigate().Refresh();

                var groupName = AnsiConsole.Ask<string>("What's The [green]GroupName[/] (group will be created)?");



                AnsiConsole.Markup("* [green][[Create group with contacts finished!]][/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.Markup("* [red][[ERROR while create group]][/]");
                AnsiConsole.WriteException(ex);

                if (AnsiConsole.Confirm("\n Wanna try again?"))
                    BulkImportToGroup(members);

                throw;
            }
        }
    }
}
