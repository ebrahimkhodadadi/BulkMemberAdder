using BulkMemberAdder.Domain;
using BulkMemberAdder.Messangers.Eitaa.Pages;
using BulkMemberAdder.Utility;
using OpenQA.Selenium;
using Spectre.Console;

namespace BulkMemberAdder.Messangers.Eitaa
{
    public class EitaaService : IMessangerService
    {
        private IWebDriver _driver;

        public async Task Start(List<Member> memberList)
        {
            _driver = new DriverFactory().Create(DriverToUse.Chrome);

            new SignPage(_driver).LoginOrRegister();

            new ContactPage(_driver).BulkImportContacts(memberList);

            // select channel or group type
            var type = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\nSelect the [green]Channle or Group[/] and Enter:")
                    .AddChoices(new[] { "Group", "Channel", "ExistGroup" }));

            switch (type)
            {
                case "Channel":
                    new ChannelPage(_driver).BulkImportToChannel(memberList);
                    break;
                case "Group":
                    new GroupPage(_driver).BulkImportToGroup(memberList);
                    break;      
                case "ExistGroup":
                    new ExistGroupPage(_driver).BulkImportToExistGroup(memberList);
                    break;
            }

            Stop();
        }


        public void Stop()
        {
            try
            {
                //// Get the current set of cookies
                //var cookies = _driver.Manage().Cookies.AllCookies;
                //// Save the cookies in a file or database
                //SaveCookies(cookies);

                Console.WriteLine("\nProccess finished and Stoped.");
                //#if DEBUG
                //#else
                _driver.Quit();
                _driver.Close();
                //#endif
            }
            catch (Exception)
            {
                // Ignore errors if we are unable to close the browser
            }
        }
    }
}
