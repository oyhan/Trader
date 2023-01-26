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
        public static string LogLevel { get; set; } = LogLevels.Info;
        public static async Task LogInfo(string msg)
        {
            if (LogLevel != LogLevels.Info)
            {
                return;
            }
            if (TelegramService != null)
            {
                await TelegramService.SendPMToAdmins(msg);
            }

            Console.WriteLine(msg);
        }

        public static async Task LogWarning(string msg)
        {
            if (LogLevel is not LogLevels.Warning)
            {
                return;
            }
            if (TelegramService != null)
            {
                await TelegramService.SendPMToAdmins(msg);
            }

            Console.WriteLine(msg);
        }
        public static async Task LogError(string msg)
        {
            if (LogLevel != LogLevels.Error)
            {
                return;
            }
            if (TelegramService != null)
            {
                await TelegramService.SendPMToAdmins(msg);
            }

            Console.WriteLine(msg);
        }
    }

    public class LogLevels
    {
        public const string Info = "Info";
        public const string Warning = "Warning";
        public const string Error = "Error";
        
    }
}
