using Belem.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Linq;
using TL;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using WTelegram;
using System.Net.Http.Json;
using Belem.Core.DTOs;

namespace Belem.Core
{
    public static class Startup
    {
        public static IServiceCollection ConfigureDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            var appSettings = new AppSettings();
            var appConfig = configuration.GetSection("AppSettings");
            appConfig.Bind(appSettings);

            if (appSettings.TelegramSettings?.ApiId != null && appSettings.TelegramSettings?.ApiHash != null)
            {
                services.AddSingleton(new WTelegram.Client(appSettings.TelegramSettings.ApiId, appSettings.TelegramSettings.ApiHash));
            }

            services.AddSingleton(appSettings);
            services.AddSingleton(new TelegramBotClient(appSettings.BotApiKey));
            services.AddSingleton<TelegramService>();
            services.AddSingleton<ImageProcessor>();
            services.AddSingleton<TradingService>();
            return services;
        }


    }

    public static class ApplicationHelper
    {
        static readonly Dictionary<long, TL.User> Users = new();
        static readonly Dictionary<long, ChatBase> Chats = new();
        static readonly Dictionary<long, ChatBase> WhiteList = new() { { 5492349096, new TL.Chat() { title = "nanci" } }, { 54170325, new TL.Chat() { title = "MSN" } } };

        public static async Task UseTelegramClient(this IApplicationBuilder webApplication)
        {
            var tgClient = webApplication.ApplicationServices.GetService<WTelegram.Client>();
            var imageProcessor = webApplication.ApplicationServices.GetService<ImageProcessor>();
            var botService = webApplication.ApplicationServices.GetService<TelegramService>();
            var appSettings = webApplication.ApplicationServices.GetService<AppSettings>();
            ApplicationLogger.TelegramService = botService;
            ApplicationLogger.LogLevel = appSettings.LogLevel;


            try
            {
                await tgClient.LoginUserIfNeeded();
            }
            catch (Exception ex)
            {


            }
            try
            {
                var dialogs = await tgClient.Messages_GetAllDialogs(); // dialogs = groups/channels/users
                dialogs.CollectUsersChats(Users, Chats);
            }
            catch (TL.RpcException ex)
            {
                await ApplicationLogger.LogInfo(ex.ToString());
                Console.WriteLine("You should login first");
                return;
            }

            var onUpdate = async (IObject arg) =>
            {
                if (arg is not UpdatesBase updates) return;
                foreach (var update in updates.UpdateList)
                {
                    //1623976558 signal group
                    //869380573 test group

                    switch (update)
                    {
                        case UpdateNewMessage { message: TL.Message message }:
                            if (message.Peer.ID == 869380573 || message.Peer.ID == 1623976558)
                            {
                                var canProcessMessage = message.Peer.ID == 1623976558 ? WhiteList.Any(c => c.Key == message.from_id.ID) ? true :
                                false : true;
                                if (canProcessMessage)
                                {
                                    if (message.media is MessageMediaPhoto { photo: Photo photo })
                                    {

                                        var filename = $"signal.jpg";
                                        await ApplicationLogger.LogInfo("Downloading " + filename);
                                        using var fileStream = System.IO.File.Create(filename);
                                        var type = await tgClient.DownloadFileAsync(photo, fileStream);
                                        fileStream.Close(); // necessary for the renaming
                                        await ApplicationLogger.LogInfo("Download finished");

                                        try
                                        {
                                            (TimeSpan buy, TimeSpan sell, string token) = await imageProcessor.GetTradeInfo(filename);

                                            var tradeModel = new SetNewTradeDto()
                                            {
                                                BuyTime = buy.Add(TimeSpan.FromMinutes(0)),
                                                SellTime = sell.Add(TimeSpan.FromMinutes(0)),
                                                Token = token
                                            };

                                            await botService.SendPMToAdmins($"{buy} , {sell} ,{token}");
                                            foreach (var tradeServer in appSettings.TradingServers)
                                            {
                                                using var httpClient = new HttpClient();
                                                httpClient.BaseAddress = new Uri(tradeServer);
                                                var result = await httpClient.PostAsJsonAsync("trade/trade", tradeModel);
                                                if (result.IsSuccessStatusCode)
                                                {
                                                    await ApplicationLogger.LogInfo($"Trade set on server {tradeServer}");
                                                }
                                                else
                                                {
                                                    var errorMessage = await result.Content.ReadAsStringAsync();
                                                    await ApplicationLogger.LogInfo($"Server {tradeServer} : Couldn't make a request to set trade {errorMessage}");

                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            await botService.SendPMToAdmins(ex.ToString());
                                        }

                                        //if (type is not Storage_FileType.unknown and not Storage_FileType.partial)
                                        //    System.IO.File.Move(filename, $"{photo.id}.{type}", true); // rename extension
                                    }
                                }

                                //await DisplayMessage(unm.message);
                            }
                            return;
                    }
                }
            };
            tgClient.OnUpdate += onUpdate;


        }

        public static void UseTelegramLogger(this IApplicationBuilder webApplication)
        {
            var botService = webApplication.ApplicationServices.GetService<TelegramService>();
            ApplicationLogger.TelegramService = botService;


        }

        public static async Task SetTimers(this IApplicationBuilder webApplication)
        {
            var tradingService = webApplication.ApplicationServices.GetService<TradingService>();
            await tradingService.SetupTimers();


        }

        //private static Task DisplayMessage(MessageBase messageBase, bool edit = false)
        //{
        //    if (edit) Console.Write("(Edit): ");
        //    switch (messageBase)
        //    {
        //        case TL.Message m: await ApplicationLogger.Log($"{Peer(m.from_id) ?? m.post_author} in {Peer(m.peer_id)}> {m.message}"); break;
        //        case MessageService ms: await ApplicationLogger.Log($"{Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]"); break;
        //    }
        //    return Task.CompletedTask;
        //}

        private static string Peer(Peer peer) => peer is null ? null : peer is PeerUser user ? User(user.user_id)
            : peer is PeerChat or PeerChannel ? Chat(peer.ID) : $"Peer {peer.ID}";

        private static string User(long id) => Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
        private static string Chat(long id) => Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
    }
}
