namespace TradingApi.Controllers.Models
{
    public class TradeApiModel
    {
        public TimeSpan BuyTime { get; set; }
        public TimeSpan SellTime { get; set; }
        public string Token { get; set; }
    }
}
