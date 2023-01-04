using Belem.Core;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using TL;
using WTelegram;

namespace Bot.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TelegramController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly WTelegram.Client _tgClient;
        static readonly Dictionary<long, TL.User> Users = new();
        static readonly Dictionary<long, ChatBase> Chats = new();
        public TelegramController(AppSettings appSettings, WTelegram.Client tgClient)
        {
            _appSettings = appSettings;
            _tgClient = tgClient;
            //_tgClient.OnUpdate += Client_OnUpdate;
            
        }
        [HttpGet]
        public async Task<ActionResult<string>> Login(string loginInfo)
        {
            //var My = await _tgClient.LoginUserIfNeeded();
            //if (My != null)
            //{
            //    Users[My.id] = My;
            //    return Ok("you're logged in");
            //}
            return await DoLogin(loginInfo);
            async Task<string> DoLogin(string loginInfo) // (add this method to your code)
            {
                while (_tgClient.User == null)
                    switch (await _tgClient.Login(loginInfo)) // returns which config is needed to continue login
                    {
                        case "verification_code": return ("Code: ");
                        case "name": loginInfo = "John Doe"; break;    // if sign-up is required (first/last_name)
                        case "password": return ("ss: ");  // if user has enabled 2FA
                        default: loginInfo = null; break;
                    }
                return ($"We are logged-in as {_tgClient.User} (id {_tgClient.User.id})");
            }
        }

        //private async Task Client_OnUpdate(IObject arg)
        //{
        //    if (arg is not UpdatesBase updates) return;
        //    foreach (var update in updates.UpdateList)
        //    {
        //        switch (update)
        //        {
        //            case UpdateNewMessage unm: await DisplayMessage(unm.message); break;
        //        }
        //    }


        //}

        //private static async Task DisplayMessage(MessageBase messageBase, bool edit = false)
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
