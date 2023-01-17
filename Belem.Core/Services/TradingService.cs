using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V106.CSS;
using OpenQA.Selenium.Interactions;
using System.IO;
using System.Runtime.InteropServices;

namespace Belem.Core.Services
{
    public class TradingService
    {
        private readonly AppSettings _appSettings;

        private Stack<string> Domains = new Stack<string>();
        private readonly IServiceProvider _serviceProvider;


        public TradingService(AppSettings appSettings, IServiceProvider serviceProvider)
        {
            this._appSettings = appSettings;


            foreach (var item in _appSettings.Domains)
            {
                Domains.Push(item);
            }
            _serviceProvider = serviceProvider;
            //InitializeInstances();

        }

        public async Task SetSellAndButOrders(TimeSpan buyTime, TimeSpan sellTime, string token)
        {
            await ApplicationLogger.Log($"Current Settings = {_appSettings}");

            foreach (var user in _appSettings.Credentials)
            {
                Console.Write($"****Setting up trader for user {user.Key}...");
                var trader = new Trader(token, user.Value, user.Key , Domains, _serviceProvider)
                {
                    Engage = _appSettings.EngageInPercent
                };

                await Scheduler.SetUpTimer(buyTime, trader.Buy);
                await Scheduler.SetUpTimer(sellTime, trader.Sell);
            }
        }

        public async Task Buy(string token)
        {
            await ApplicationLogger.Log($"Current Settings = {_appSettings}");

            foreach (var user in _appSettings.Credentials)
            {
                Console.Write($"****Buy for {token} user {user.Key}...");
                var trader = new Trader(token, user.Value, user.Key, Domains, _serviceProvider)
                {
                    Engage = _appSettings.EngageInPercent
                };

                await trader.Buy();
            }
        }
        public async Task Sell(string token)
        {
            await ApplicationLogger.Log($"Current Settings = {_appSettings}");

            foreach (var user in _appSettings.Credentials)
            {
                Console.Write($"****Sell {token} for user {user.Key}...");
                var trader = new Trader(token, user.Value, user.Key, Domains, _serviceProvider)
                {
                    Engage = _appSettings.EngageInPercent
                };
                await trader.Sell();
            }
        }

        public async Task SetupTimers()
        {
            await ApplicationLogger.Log($"SetTimers to redem and subscribe money....");

            foreach (var user in _appSettings.Credentials)
            {
                await ApplicationLogger.Log($"****Setting up trader for user {user.Key}...");
                var trader = new Trader("", user.Value, user.Key, Domains, _serviceProvider)
                {
                    Engage = _appSettings.EngageInPercent
                };

                await Scheduler.PeriodicTimer(new TimeSpan(18,15,0), trader.RedeemMoney, new TimeSpan(24, 0, 0));
                await Scheduler.PeriodicTimer(new TimeSpan(22,15,0), trader.SubscribeMoney, new TimeSpan(24, 0, 0));
            }
        }

        public async Task RedeemMoney()
        {
            await ApplicationLogger.Log($"SetTimers to redem and subscribe money....");

            foreach (var user in _appSettings.Credentials)
            {
                await ApplicationLogger.Log($"****Setting up trader for user {user.Key}...");
                var trader = new Trader("", user.Value, user.Key, Domains, _serviceProvider)
                {
                    Engage = _appSettings.EngageInPercent
                };
                await trader.RedeemMoney();
            }
        }

        public async Task SubscribeMoney()
        {
            await ApplicationLogger.Log($"SetTimers to redem and subscribe money....");

            foreach (var user in _appSettings.Credentials)
            {
                await ApplicationLogger.Log($"****Setting up trader for user {user.Key}...");
                var trader = new Trader("", user.Value, user.Key, Domains, _serviceProvider)
                {
                    Engage = _appSettings.EngageInPercent
                };
                await trader.SubscribeMoney();
            }
        }
        //private void InitializeInstances()
        //{
        //    _traders = new Dictionary<string, Trader>();
        //    foreach (var user in _appSettings.Credentials)
        //    {
        //        var chromeOptions = new ChromeOptions();

        //        //chromeOptions.AddArguments("headless");
        //        //chromeOptions.AddArguments("no-sandbox");
        //        //chromeOptions.AddArguments("headless");
        //        var linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        //        if (linux)
        //        {
        //            var path = "selenium";


        //            var fileExiest = File.Exists(path);
        //            var browser = new ChromeDriver(path, chromeOptions);
        //            var trader = new Trader(Domains.Pop(), browser, user.Value, user.Key);
        //            _traders[user.Key] = (trader);
        //        }
        //        else
        //        {
        //             var browser = new ChromeDriver(chromeOptions);
        //            var trader = new Trader(Domains.Pop(), browser, user.Value, user.Key);
        //            _traders[user.Key] = (trader);

        //        }


        //    }
        //}





    }
}
