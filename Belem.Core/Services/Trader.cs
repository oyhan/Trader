using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Telegram.Bot.Types;
using OpenQA.Selenium.Interactions;

namespace Belem.Core.Services
{
    public class Trader 
    {
        private Actions _action;

        public string Token { get; set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public WebDriver Browser { get; private set; }
        public string DomainAddress { get; private set; }

        public int Engage { get; set; } = 25;

        public Trader(string domainAddress, string password, string usename)
        {
            DomainAddress = domainAddress;
            Password = password;
            Username = usename;
        }

        public void Buy()
       {
            Console.WriteLine("BUY************");
            SetBrowser();
            Login(Username, Password);
            Thread.Sleep(3000);
            SetBuyOrder();
            BuyCoin();
            Thread.Sleep(3000);
            Browser.Quit();
        }

        public void Sell()
        {
            Console.WriteLine("SELL************");
            SetBrowser();

            Login(Username, Password);

            Thread.Sleep(3000);

            SetSellOrder();

            SellCoin();
            Thread.Sleep(3000);
            Browser.Quit();

        }


        private void SetBuyOrder()
        {
            Browser.Url = $"{DomainAddress}/#/exchange/{Token}_usdt";
            Thread.Sleep(3000);
            var gharbilak = GetElement(By.CssSelector($"[for|=\"slider_name_buy{Engage}\"]"));
            gharbilak.Click();
        }
        private void Login(string username, string password)
        {
            Browser.Url = $"{DomainAddress}/#/user/login";
            Thread.Sleep(2000);
            var userName = GetElement(By.Name("UserName"));
            userName.SendKeys(username);
            var passwordElement = GetElement(By.Name("Password"));

            passwordElement.SendKeys(password);

            var submit = GetElement(By.CssSelector(".btn.btn-submit"));
            submit.Click();
        }

        private void BuyCoin()
        {
            var submit = GetElement(By.CssSelector(".btn-submit.mb-btn.bg-buy"));
            submit.Click();
        }
        private void SellCoin()
        {
            var submit = GetElement(By.CssSelector(".btn-submit.mb-btn.bg-sell"));

            Browser.ExecuteScript("arguments[0].click();", submit);

            
            //submit.Click();
        }



        private void SetSellOrder()
        {
            Browser.Url = $"{DomainAddress}/#/exchange/{Token}_usdt";
            Thread.Sleep(3000);

            var gharbilak = GetElement(By.CssSelector("[for|=\"slider_name_sell100\"]"));
            
            _action.MoveToElement(gharbilak);
            _action.ScrollToElement(gharbilak).Perform();
            Browser.ExecuteScript("window.scroll(0,5000)");
            gharbilak.Click();


        }

        private IWebElement GetElement(By by)
        {
            try
            {
                var element = Browser.FindElement(by);
                return element;
            }
            catch (OpenQA.Selenium.NotFoundException)
            {

                Thread.Sleep(10000);
                Browser.Url = Browser.Url;
                var element = Browser.FindElement(by);
                return element;
            }

            throw new NotFoundException("Can not find element");
        }

        private void SetBrowser()
        {
            var linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("headless");
            chromeOptions.AddArguments("--start-maximized");

            if (linux)
            {
                var path = "selenium";
                var fileExiest = System.IO.File.Exists(path);
                var browser = new ChromeDriver(path, chromeOptions);
                Browser = browser;
            }
            else
            {
                var browser = new ChromeDriver(chromeOptions);
                Browser = browser;
            }

            Actions action = new Actions(Browser);

            _action = action;
        }

       
    }


}
