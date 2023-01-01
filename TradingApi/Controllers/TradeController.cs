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

        public TradeController(TradingService tradingService)
        {
            _tradingService = tradingService;
        }

        [HttpPost]
        public IActionResult Trade(TradeApiModel model)
        {
             _tradingService.SetSellAndButOrders(model.BuyTime, model.SellTime, model.Token);
            return Ok();
        }
    }
}