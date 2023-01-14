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
using OpenQA.Selenium.Support.UI;
using System.Security.Policy;

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
        public Stack<string> DomainStack { get; internal set; }

        public Trader(string token, string password, string usename, Stack<string> domains)
        {
            Token = token;
            Password = password;
            Username = usename;
            DomainStack = domains;
        }

       

        public async Task Buy()
        {
            try
            {
                await ApplicationLogger.Log($"BUY************ for {Username} ");
                await SetBrowser();
                Login(Username, Password);
                Thread.Sleep(3000);
                SetBuyOrder();
                BuyCoin();
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                await ApplicationLogger.Log($"exception for user {Username} executing method {System.Reflection.MethodBase.GetCurrentMethod().Name} exception : {ex.Message}");
                throw;
            }
            finally
            {
                await ReleaseResouces();

            }
        }



        public async Task Sell()
        {
            try
            {
                await ApplicationLogger.Log($"SELL************ for {Username} ");
                await SetBrowser();

                Login(Username, Password);

                Thread.Sleep(3000);

                SetSellOrder();

                SellCoin();

                Thread.Sleep(3000);

            }
            catch (Exception ex)
            {
                await ApplicationLogger.Log($"exception for user {Username} executing method {System.Reflection.MethodBase.GetCurrentMethod().Name} exception : {ex.Message}");
                throw;
            }
            finally
            {
                await ReleaseResouces();

            }
        }


        public async Task RedeemMoney()
        {
            try
            {
                await ApplicationLogger.Log($"RedeemMoney************ for {Username} on domain {DomainAddress}");
                await SetBrowser();



                Login(Username, Password);

                Thread.Sleep(4000);

                var togg = GetElements(By.ClassName("toggle-lang"));
                Browser.ExecuteScript("arguments[0].click();", togg.First());

                Thread.Sleep(2000);


                Browser.Url = $"{DomainAddress}/#/earn/account";
                Thread.Sleep(2000);
                var btn = GetElement(By.CssSelector(".btn.button"));
                if (btn != null)
                {
                    if (btn.Text.ToLower().Contains("Early Redemption".ToLower()))
                    {
                        Thread.Sleep(1000);
                        ClickMe(btn);
                        Thread.Sleep(1000);
                        var confirmBtn = GetElement(By.XPath("//button[@type=\"submit\"]"));
                        if (confirmBtn != null)
                        {
                            if (confirmBtn.Text.ToLower().Contains("Confirm".ToLower()))
                            {
                                Thread.Sleep(1500);
                                ClickMe(confirmBtn);
                                Thread.Sleep(1000);


                            }
                        }
                    }
                }
                Thread.Sleep(2000);
                await ApplicationLogger.Log($"RedeemMoney************ FINISHED for {Username} on domain {DomainAddress}");
            }
            catch (Exception ex)
            {
                await ApplicationLogger.Log($"exception for user {Username} executing method {System.Reflection.MethodBase.GetCurrentMethod().Name} exception : {ex.Message}");
                throw;
            }
            finally
            {
                await ReleaseResouces();
            }


        }


        public async Task SubscribeMoney()
        {
            try
            {
                await ApplicationLogger.Log($"SubscribeMoney ************ for {Username} on domain {DomainAddress}");
                await SetBrowser();

                Login(Username, Password);

                //Thread.Sleep(4000);

                var togg = GetElements(By.ClassName("toggle-lang"));
                Browser.ExecuteScript("arguments[0].click();", togg.First());

                Thread.Sleep(2000);


                Browser.Url = $"{DomainAddress}/#/earn";
                Thread.Sleep(2000);
                var btns = GetElements(By.CssSelector(".btn.button"));
                Thread.Sleep(2000);
                var btn = btns
                    .FirstOrDefault(b => b.Text.ToLower().Contains("Subscription".ToLower()));
                if (btn != null)
                {
                    {
                        Thread.Sleep(4000);

                        ClickMe(btn);
                        Thread.Sleep(2000);

                        var maximumBtn = GetElement(By.XPath("//span[@class=\"input-group-btn\"]"));
                        if (maximumBtn != null)
                        {
                            if (maximumBtn.Text.ToLower().Contains("Maximum".ToLower()))
                            {
                                Thread.Sleep(2000);
                                ClickMe(maximumBtn);

                                ConfirmAgreement();

                            }
                        }
                    }
                }
                await ApplicationLogger.Log($"SubscribeMoney FINISHED ************ for {Username} on domain {DomainAddress}");
            }
            catch (Exception ex)
            {
                await ApplicationLogger.Log($"exception for user {Username} executing method {System.Reflection.MethodBase.GetCurrentMethod().Name} exception : {ex.Message}");
                throw;
            }
            finally
            {
                await ReleaseResouces();

            }

        }

        private void ConfirmAgreement()
        {
            var btn = GetElement(By.XPath("//label[@for=\"earn-checkbox\"]"));
            if (btn is not null)
            {
                ClickMe(btn);
            }

            var confirm = GetElements(By.CssSelector(".btn.button"))
                .FirstOrDefault(b => b.Text.ToLower().Contains("Confirm subscription".ToLower()));
            if (confirm != null)
            {
                ClickMe(confirm);
            }

        }

        private void ClickMe(IWebElement btn)
        {
            Browser.ExecuteScript("arguments[0].click();", btn);

        }

        //private void Wait(int v)
        //{
        //    var x = new WebDriverWait(Browser, TimeSpan.FromMilliseconds(v));
        //    x.IgnoreExceptionTypes();
        //    x.Until()
        //}

        private async Task ReleaseResouces()
        {
            Browser.Quit();
            DomainStack.Push(DomainAddress);
            await ApplicationLogger.Log($"put back domian {DomainAddress} for {Username} ");
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

            //_action.MoveToElement(gharbilak);
            //_action.ScrollToElement(gharbilak).Perform();
            //Browser.ExecuteScript("window.scroll(0,5000)");
            Browser.ExecuteScript("arguments[0].click();", gharbilak);
            //gharbilak.Click();


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

                Browser.Url = Browser.Url;
                var element = Browser.FindElement(by);
                return element;
            }

            throw new NotFoundException("Can not find element");
        }

        private IEnumerable<IWebElement> GetElements(By by)
        {
            try
            {
                var element = Browser.FindElements(by);
                return element;
            }
            catch (OpenQA.Selenium.NotFoundException)
            {

                Thread.Sleep(10000);
                Browser.Url = Browser.Url;
                var element = Browser.FindElements(by);
                return element;
            }

            throw new NotFoundException("Can not find element");
        }

        private async Task SetBrowser()
        {
            var baseUrl = string.Empty;
            while (!DomainStack.TryPop(out baseUrl))
            {
                //await ApplicationLogger.Log("wating for a url to be free...");
                Thread.Sleep(1000);
            }
            await ApplicationLogger.Log($"Domain {baseUrl} got free!");

            DomainAddress = baseUrl;

            var linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("no-sandbox");
            chromeOptions.AddArguments("headless");
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.76";
            chromeOptions.AddArgument($"user-agent={userAgent}");
            chromeOptions.AddArguments("--start-maximized");

            if (linux)
            {
                var path = "/app/selenium";
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
