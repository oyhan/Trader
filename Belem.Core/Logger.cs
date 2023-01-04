using Belem.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belem.Core
{
    public static class ApplicationLogger
    {
        public static TelegramService? TelegramService { get; set; } = null;
        public static async Task Log(string msg)
        {
            if (TelegramService != null)
            {
                await TelegramService.SendPMToAdmins(msg);
            }

            Console.WriteLine(msg);
        }
    }
}
