using Belem.Core;
using Belem.Core.Services;
using Microsoft.AspNetCore.Mvc;
using TradingApi.Controllers.Models;

namespace TradingApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TradeController : ControllerBase
    {
        private readonly TradingService _tradingService;
        private readonly AppSettings _appSettings;

        public TradeController(TradingService tradingService, AppSettings appSettings)
        {
            _tradingService = tradingService;
            _appSettings = appSettings;
        }

        [HttpPost]
        public async Task<IActionResult> Trade(TradeApiModel model)
        {
            Console.WriteLine($"{_appSettings}");
            await ApplicationLogger.Log($"Current time  {DateTime.Now.TimeOfDay}");
            await   _tradingService.SetSellAndButOrders(model.BuyTime, model.SellTime, model.Token);
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Console.WriteLine(Request.Headers.UserAgent.ToString());
            return Ok();
        }
    }
}