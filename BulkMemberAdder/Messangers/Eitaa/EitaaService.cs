using BulkMemberAdder.Domain;
using BulkMemberAdder.Messangers.Eitaa.Pages;
using BulkMemberAdder.Utility;
using OpenQA.Selenium;

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

            new GroupPage(_driver).BulkImportToGroup(memberList);
        }


        public void Stop()
        {
            try
            {
                //// Get the current set of cookies
                //var cookies = _driver.Manage().Cookies.AllCookies;
                //// Save the cookies in a file or database
                //SaveCookies(cookies);

#if DEBUG
                Console.WriteLine("\nProccess finished.");
#else
                _driver.Quit();
                _driver.Close();
#endif
            }
            catch (Exception)
            {
                // Ignore errors if we are unable to close the browser
            }
        }
    }
}
