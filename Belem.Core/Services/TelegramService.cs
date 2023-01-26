using Microsoft.VisualBasic;
using System.Net.Http.Json;
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
                        var command = update?.Message?.Text;
                        
                        if (command is not null)
                        {
                           await ExecuteCommand(command);
                        }
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

        private async Task ExecuteCommand(string command)
        {
            var sections = command.Split(" ");
            if (sections.Length<2)
            {
                await ApplicationLogger.LogInfo("can not execute command less than 2 part");
                return;
            }
            var toplevel = sections[0];
            var function = sections[1];
            if (function.ToLower()=="server")
            {
                foreach (var tradeServer in _appSettings.TradingServers)
                {
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri(tradeServer);
                    var result = await httpClient.GetAsync($"{toplevel}/{function}");
                    if (result.IsSuccessStatusCode)
                    {
                        await ApplicationLogger.LogInfo($"command executed with result {await result.Content.ReadAsStringAsync()} on server {tradeServer}");
                    }
                    else
                    {
                        var errorMessage = await result.Content.ReadAsStringAsync();
                        await ApplicationLogger.LogInfo($"Server {tradeServer} : Couldn't make a request to set trade {errorMessage}");

                    }
                }
            }

            if (function.ToLower()== "buy" || function.ToLower()=="sell")
            {
                if (sections.Length < 3)
                {
                    await ApplicationLogger.LogError("token needed"); 
                    return;
                }
                var token = sections[2];

                foreach (var tradeServer in _appSettings.TradingServers)
                {
                    using var httpClient = new HttpClient();
                    httpClient.Timeout = TimeSpan.FromMinutes(3);
                    httpClient.BaseAddress = new Uri(tradeServer);
                    var result = await httpClient.GetAsync($"{toplevel}/{function}?token={token}");
                    if (result.IsSuccessStatusCode)
                    {
                        await ApplicationLogger.LogInfo($"command executed with result {await result.Content.ReadAsStringAsync()} on server {tradeServer}");
                    }
                    else
                    {
                        var errorMessage = await result.Content.ReadAsStringAsync();
                        await ApplicationLogger.LogInfo($"Server {tradeServer} : Couldn't make a request to set trade {errorMessage}");

                    }
                }
            }
        }

        public async Task SendPMToAdmins(string messaga)
        {
            foreach (var admin in _appSettings.AllowedChats)
            {
                await _bot.SendTextMessageAsync(admin, messaga);
            }
        }

        public async Task SendPhotoToAdmins(MemoryStream memoryStream,string caption)
        {
            foreach (var admin in _appSettings.AllowedChats)
            {
                await  _bot.SendPhotoAsync(admin,new Telegram.Bot.Types.InputFiles.InputOnlineFile(memoryStream), caption: caption);
            }
        }
    }
}
