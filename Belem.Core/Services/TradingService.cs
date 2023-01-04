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
        private static Dictionary<string, Trader> _traders;

        private Stack<string> Domains = new Stack<string>();

        public TradingService(AppSettings appSettings)
        {
            this._appSettings = appSettings;

            
            foreach (var item in _appSettings.Domains)
            {
                Domains.Push(item);
            }
            //InitializeInstances();

        }

        public async Task SetSellAndButOrders(TimeSpan buyTime, TimeSpan sellTime, string token)
        {
            await ApplicationLogger.Log($"Current Settings = {_appSettings}");

            foreach (var user in _appSettings.Credentials)
            {
                Console.Write($"****Setting up trader for user {user.Key}...");
                var trader = new Trader(token, user.Value, user.Key , Domains)
                {
                    Engage = _appSettings.EngageInPercent
                };

                await Scheduler.SetUpTimer(buyTime, trader.Buy);
                await Scheduler.SetUpTimer(sellTime, trader.Sell);
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
