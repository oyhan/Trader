using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System.IO;
using System.Runtime.InteropServices;

namespace Belem.Core.Services
{
    public class TradingService
    {
        private readonly AppSettings _appSettings;
        private Dictionary<string, Trader> _traders;

        private Stack<string> Domains = new Stack<string>();

        public TradingService(AppSettings appSettings)
        {
            this._appSettings = appSettings;

            if (appSettings.Credentials.Count > appSettings.Domains.Count())
            {
                throw new InvalidOperationException("no of domains is less than no of users");
            }
            foreach (var item in _appSettings.Domains)
            {
                Domains.Push(item);
            }
            InitializeInstances();

        }

        public async Task SetSellAndButOrders(TimeSpan buyTime, TimeSpan sellTime, string token)
        {

            foreach (var trader in _traders)
            {
                trader.Value.Token = token;

                trader.Value.Engage = _appSettings.EngageInPercent;

                SetUpTimer(buyTime, trader.Value.Buy);

                SetUpTimer(sellTime, trader.Value.Sell);
            }
        }

        private void InitializeInstances()
        {
            _traders = new Dictionary<string, Trader>();
            foreach (var user in _appSettings.Credentials)
            {
                var chromeOptions = new ChromeOptions();

                chromeOptions.AddArguments("headless");
                chromeOptions.AddArguments("no-sandbox");
                //chromeOptions.AddArguments("headless");
                var linux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

                if (linux)
                {
                    var path = "selenium";
                   

                    var fileExiest = File.Exists(path);
                    using var browser = new ChromeDriver(path, chromeOptions);
                    var trader = new Trader(Domains.Pop(), browser, user.Value, user.Key);
                    _traders[user.Key] = (trader);
                    trader.Buy(); 
                }
                else
                {
                    using var browser = new ChromeDriver(chromeOptions);
                    var trader = new Trader(Domains.Pop(), browser, user.Value, user.Key);
                    _traders[user.Key] = (trader);

                }


            }
        }




        private void SetUpTimer(TimeSpan alertTime, Action action)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            var timer = new Timer(x =>
            {
                action();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }
    }
}
