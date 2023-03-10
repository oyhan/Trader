using Belem.Core;
using Belem.Core.Services;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.DevTools.V106.Network;
using TradingApi.Controllers.Models;

namespace TradingApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TradeController : ControllerBase
    {
        private readonly TradingService _tradingService;
        private readonly AppSettings _appSettings;
        private readonly IHostApplicationLifetime _appliction;

        public TradeController(TradingService tradingService, AppSettings appSettings, IHostApplicationLifetime appliction)
        {
            _tradingService = tradingService;
            _appSettings = appSettings;
            _appliction = appliction;
        }

        [HttpPost]
        public async Task<IActionResult> Trade(TradeApiModel model)
        {
            Console.WriteLine($"{_appSettings}");
            await ApplicationLogger.LogInfo($"Current time  {DateTime.Now.TimeOfDay}");
            await   _tradingService.SetSellAndButOrders(model.BuyTime, model.SellTime, model.Token);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Buy(string token)
        {
            await _tradingService.Buy(token);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> Sell(string token)
        {
            await _tradingService.Sell(token);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Subscribe()
        {
            await _tradingService.SubscribeMoney();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Redeem()
        {
            await _tradingService.RedeemMoney();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Reboot()
        {
            _appliction.StopApplication();

            return Ok("rebooted");
        }
    }
}