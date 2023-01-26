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
using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;
using TL.Methods;
using System.Drawing;

namespace Belem.Core.Services
{
    public class Trader
    {
        private const string BalanceHistoryPath = "#/finance/history";
        private Actions _action;

        public string Token { get; set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public WebDriver Browser { get; private set; }
        public string DomainAddress { get; private set; }

        private readonly IServiceProvider _serviceProvider;

        private readonly TimeSpan _buyTime;
        private readonly TimeSpan _sellTime;

        public int Engage { get; set; } = 25;
        public Stack<string> DomainStack { get; internal set; }

        public Trader
            (string token, string password, string usename,
            Stack<string> domains, IServiceProvider serviceProvider,
            TimeSpan buyTime, TimeSpan sellTime)
            : this(token, password, usename, domains, serviceProvider)

        {

            _buyTime = buyTime;
            _sellTime = sellTime;
        }

        public Trader(string token, string password, string usename, Stack<string> domains, IServiceProvider serviceProvider)
        {
            Token = token;
            Password = password;
            Username = usename;
            DomainStack = domains;
            _serviceProvider = serviceProvider;
            //_buyTime = buyTime;
            //_sellTime = sellTime;
        }
        public async Task Trade()
        {
            try
            {


                var now = DateTime.Now.TimeOfDay;
                if (now > _sellTime)
                {
                    await ApplicationLogger.LogInfo("Time passed");
                    return;
                }
                await ApplicationLogger.LogInfo($"BUY************ for {Username} ");
                await SetBrowser();
                Login(Username, Password);
                await Task.Delay(TimeSpan.FromSeconds(3));

                GoToTradePage();
                await Task.Delay(TimeSpan.FromSeconds(20));
                SetBuyOrder();
                await TakeAndSendScreenShot();

                now = DateTime.Now.TimeOfDay;

                var waitToBuy = (_buyTime - now);
                //sell time passed    
                //if buy time passed more than 10 min
                if (waitToBuy <= TimeSpan.Zero)
                {
                    if (waitToBuy.TotalMinutes > 10)
                    {
                        await ApplicationLogger.LogInfo("Buy time passed more than 10 min");
                        return;
                    }
                    waitToBuy = TimeSpan.FromSeconds(5);
                }



                Thread.Sleep(waitToBuy);

                BuyCoin();
                await TakeAndSendScreenShot();

                await Task.Delay(3000);

                SetSellOrder();

                now = DateTime.Now.TimeOfDay;
                var waitToSell = _sellTime - now;

                if (waitToSell <= TimeSpan.Zero)
                {

                    waitToBuy = TimeSpan.FromSeconds(2);
                }
                await Task.Delay(waitToSell);

                SellCoin();
                await TakeAndSendScreenShot();

                Thread.Sleep(3000);

                await TakeAndSendScreenShot(BalanceHistoryPath);

            }
            catch (Exception ex)
            {
                await ApplicationLogger.LogInfo($"exception for user {Username} executing method Trade exception : {ex.Message}");
                throw;
            }
            finally
            {
                await ReleaseResouces();

            }
        }

        public async Task Buy()
        {
            try
            {
                await ApplicationLogger.LogInfo($"BUY************ for {Username} ");
                await SetBrowser();
                Login(Username, Password);
                Thread.Sleep(3000);
                SetBuyOrder();
                BuyCoin();
                Thread.Sleep(3000);
            }
            catch (Exception ex)
            {
                await ApplicationLogger.LogInfo($"exception for user {Username} executing method {System.Reflection.MethodBase.GetCurrentMethod().Name} exception : {ex.Message}");
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
                await ApplicationLogger.LogInfo($"SELL************ for {Username} ");
                await SetBrowser();

                Login(Username, Password);

                Thread.Sleep(3000);

                SetSellOrder();

                SellCoin();

                Thread.Sleep(5000);

                await TakeAndSendScreenShot(BalanceHistoryPath);



            }
            catch (Exception ex)
            {
                await ApplicationLogger.LogInfo($"exception for user {Username} executing method {System.Reflection.MethodBase.GetCurrentMethod().Name} exception : {ex.Message}");
                throw;
            }
            finally
            {
                await ReleaseResouces();

            }
        }

        private async Task TakeAndSendScreenShot(string path = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    Browser.Navigate().GoToUrl($"{DomainAddress}/{path}");
                    Thread.Sleep(3000);
                }
                Browser.ExecuteScript("document.body.style.zoom = '0.8'");
                Thread.Sleep(2000);
                Screenshot screenShot = Browser.GetScreenshot();
                var tg = _serviceProvider.GetRequiredService<TelegramService>();
                await tg.SendPhotoToAdmins(new MemoryStream(screenShot.AsByteArray), Username);
            }
            catch (Exception ex)
            {
                await ApplicationLogger.LogInfo($"error in taking screen shot {Username} ex : {ex.Message}");
                throw;
            }
        }

        public async Task RedeemMoney()
        {
            try
            {
                await ApplicationLogger.LogInfo($"RedeemMoney************ for {Username} on domain {DomainAddress}");
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
                await ApplicationLogger.LogInfo($"RedeemMoney************ FINISHED for {Username} on domain {DomainAddress}");
            }
            catch (Exception ex)
            {
                await ApplicationLogger.LogInfo($"exception for user {Username} executing method {System.Reflection.MethodBase.GetCurrentMethod().Name} exception : {ex.Message}");
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
                await ApplicationLogger.LogInfo($"SubscribeMoney ************ for {Username} on domain {DomainAddress}");
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
                await ApplicationLogger.LogInfo($"SubscribeMoney FINISHED ************ for {Username} on domain {DomainAddress}");
            }
            catch (Exception ex)
            {
                await ApplicationLogger.LogInfo($"exception for user {Username} executing method {System.Reflection.MethodBase.GetCurrentMethod().Name} exception : {ex.Message}");
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
            await ApplicationLogger.LogInfo($"put back domian {DomainAddress} for {Username} ");
        }

        private void SetBuyOrder()
        {
            var path = $"{DomainAddress}/#/exchange/{Token}_usdt";
            if (Browser.Url.ToLower() != path.ToLower())
            {
                Browser.Url = path;
                Thread.Sleep(3000);
            }

            Browser.MonkeyPatchXMLHttpRequest();
            Browser.CheckPendingRequests(20);
            var gharbilak = GetElement(By.CssSelector($"[for|=\"slider_name_buy{Engage}\"]"));
            ClickMe(gharbilak);
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
            Browser.CheckPendingRequests(20);
            ClickMe(submit);

        }
        private void SellCoin()
        {
            Browser.CheckPendingRequests(20);
            var submit = GetElement(By.CssSelector(".btn-submit.mb-btn.bg-sell"));

            if (submit is null)
            {
                throw new Exception("can not find submit to sell");
            }

            ClickMe(submit);



            //submit.Click();
        }



        private void SetSellOrder()
        {
            var path = $"{DomainAddress}/#/exchange/{Token}_usdt";
            if (Browser.Url.ToLower() != path.ToLower())
            {
                Browser.Url = path;
                Thread.Sleep(3000);
            }

            Browser.MonkeyPatchXMLHttpRequest();
            Browser.CheckPendingRequests(20);
            var gharbilak = GetElement(By.CssSelector("[for|=\"slider_name_sell100\"]"));

            if (gharbilak is null)
            {
                throw new Exception("can not find gharbilak for sell");
            }
            //_action.MoveToElement(gharbilak);
            //_action.ScrollToElement(gharbilak).Perform();
            //Browser.ExecuteScript("window.scroll(0,5000)");

            ClickMe(gharbilak);

            //gharbilak.Click();


        }

        private IWebElement GetElement(By by)
        {
            try
            {
                var element = Browser.FindElement(by, 20);
                return element;
            }
            catch (OpenQA.Selenium.NotFoundException)
            {

                //Browser.Url = Browser.Url;
                Browser.Navigate().Refresh();
                var element = Browser.FindElement(by, 30);
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

                Browser.Navigate().Refresh();
                Thread.Sleep(10000);
                //Browser.Url = Browser.Url;
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
                //await ApplicationLogger.Log($"wating for a url to be free... {Username}");
                Thread.Sleep(1000);
            }
            await ApplicationLogger.LogInfo($"Domain {baseUrl} got free!");

            DomainAddress = baseUrl;

            var linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("no-sandbox");
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.76";
            chromeOptions.AddArgument($"user-agent={userAgent}");
            chromeOptions.AddArguments("--start-maximized");



            if (linux)
            {
                var profilePath = $"/usr/root/{Username}/browsing";
                if (!Directory.Exists(profilePath))
                {
                    Directory.CreateDirectory(profilePath);
                }
                chromeOptions.AddArguments($"user-data-dir={profilePath}");
                chromeOptions.AddArguments("headless");
                var path = "/app/selenium";
                var fileExiest = System.IO.File.Exists(path);
                var browser = new ChromeDriver(path, chromeOptions);
                Browser = browser;
            }
            else
            {
                var profilePath = $"c:/profile/{Username}/browsing";
                if (!Directory.Exists(profilePath))
                {
                    Directory.CreateDirectory(profilePath);
                }
                chromeOptions.AddArguments($"user-data-dir={profilePath}");
                var browser = new ChromeDriver(chromeOptions);
                Browser = browser;
            }

            Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Actions action = new Actions(Browser);

            _action = action;
        }
        private void GoToTradePage()
        {
            Browser.Url = $"{DomainAddress}/#/exchange/{Token}_usdt";
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

    }


}
