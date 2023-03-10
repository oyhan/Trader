namespace Belem.Core
{
    public class AppSettings
    {
        public string[] AllowedChats { get; set; } = new string[0];
        public string[] Domains { get; set; } = new string[0];
        public string BotApiKey { get; set; } = string.Empty;

        public string[] TradingServers { get; set; }
        public Dictionary<string,string> Credentials { get; set; }
        public int EngageInPercent { get; set; } = 25;

        public string LogLevel { get; set; } = LogLevels.Info;

        public string OcrURL { get; set; }
        public string SignalImageUrl { get; set; }

        public TelegramSettings TelegramSettings { get; set; }

        public override string ToString()
        {
            var msg = @$"Domains= {string.Join(",", Domains)}
EngagePercent = {EngageInPercent}%
Allowed Chats = {string.Join(",", AllowedChats)}
";
            return msg; 
        }
    }
}

public class TelegramSettings
{
    public int ApiId { get; set; }
    public string ApiHash { get; set; }
}
