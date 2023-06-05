using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMemberAdder.Utility
{
    public class SeleniumUtils
    {
        private readonly IWebDriver _driver;
        public SeleniumUtils(IWebDriver driver)
        {
            _driver = driver;
        }
        public bool IsElementPresent(By by)
        {
            try
            {
                _driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public string CloseAlertAndGetItsText()
        {
            var alert = _driver.SwitchTo().Alert();
            alert.Accept();
            return alert.Text;
        }
    }
}
