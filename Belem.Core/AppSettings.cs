namespace Belem.Core
{
    public class AppSettings
    {
        public string[] AllowedChats { get; set; } = new string[0];
        public string[] Domains { get; set; } = new string[0];
        public string BotApiKey { get; set; } = string.Empty;

        public string TradingServers { get; set; }
        public Dictionary<string,string> Credentials { get; set; }
        public int EngageInPercent { get; set; } = 25;
    }
}
