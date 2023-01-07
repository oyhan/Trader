using Telegram.Bot;

namespace Belem.Core.Services
{
    public class TelegramService
    {
        private const string SignalPath = "wwwroot/signal.jpg";
        private const string LastUpdate = "lastupdate.txt";
        private readonly  TelegramBotClient _bot;
        private readonly ImageProcessor _imageProcessor;
        private readonly AppSettings _appSettings;

        

        public TelegramService(TelegramBotClient bot, AppSettings appSettings,
            ImageProcessor imageProcessor)
        {
            _bot = bot;
            _appSettings = appSettings;
            _imageProcessor = imageProcessor;
        }

        public async Task CheckMessages()
        {
            try
            {
                var lastUpdateId = -1;
                try
                {
                    var lastUpdate = File.ReadAllText(LastUpdate);
                    lastUpdateId = int.Parse(lastUpdate);
                }
                catch (Exception ex)
                {


                }

                var updates = await _bot.GetUpdatesAsync(lastUpdateId + 1);

                foreach (var update in updates)
                {

                    if (_appSettings.AllowedChats.Contains(update?.Message?.Chat.Id.ToString()))
                    {
                        var photo = update?.Message?.Photo;
                        if (photo != null)
                        {
                            var biggestPhoto = photo.OrderByDescending(p => p.FileSize).FirstOrDefault();

                            var file = await _bot.GetFileAsync(biggestPhoto.FileId);

                            var fileStream = new FileStream(SignalPath, FileMode.OpenOrCreate);

                            await _bot.DownloadFileAsync(file.FilePath, fileStream);

                            fileStream.Close();

                            (TimeSpan buy, TimeSpan sell, string token) = await _imageProcessor.GetTradeInfo(SignalPath);



                            await SendPMToAdmins($"{buy} , {sell} ,{token}");

                        }
                    }
                }
                if (updates.Any())
                {
                    File.WriteAllText(LastUpdate, updates.Last().Id.ToString());
                }

            }
            catch (Exception ex)
            {
                await SendPMToAdmins(ex.ToString());
            }

        }

        public async Task SendPMToAdmins(string messaga)
        {
            foreach (var admin in _appSettings.AllowedChats)
            {
                await _bot.SendTextMessageAsync(admin, messaga);
            }
        }
    }
}
