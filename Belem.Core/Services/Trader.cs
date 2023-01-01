using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belem.Core.Services
{
    public class Trader : IDisposable
    {
        public string Token { get; set; }
        public string Username { get;  private set; }
        public string Password { get; private set; }
        public WebDriver Browser { get; private set; }
        public string DomainAddress  { get; private set; }

        public int Engage { get; set; } = 25;

        public Trader(string domainAddress, WebDriver browser, string password, string usename)
        {
            DomainAddress = domainAddress;
            Browser = browser;
            Password = password;
            Username = usename;
        }

        public void Buy()
        {
            Browser.Url = "https://google.com";
            if (Browser.Title =="Google")
            {
                Console.WriteLine("YES***********");
            }
            //Login(Username, Password);
            //Thread.Sleep(3000);
            //SetBuyOrder();
            //BuyCoin();
        }

        public void Sell()
        {

            Login(Username, Password);

            Thread.Sleep(3000);

            SetSellOrder();

            SellCoin();

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
            submit.Click();
        }



        private void SetSellOrder()
        {
            Browser.Url = $"{DomainAddress}/#/exchange/{Token}_usdt";
            Thread.Sleep(3000);
            var gharbilak = GetElement(By.CssSelector("[for|=\"slider_name_sell100\"]"));
            gharbilak.Click();


        }

        private IWebElement GetElement(By by)
        {
            try
            {
                var element = GetElement(by);
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

        public void Dispose()
        {
            Browser.Quit();
        }
    }

  
}
