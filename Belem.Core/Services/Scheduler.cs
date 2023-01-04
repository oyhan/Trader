using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belem.Core.Services
{
    public static class Scheduler
    {
        public static List<Timer> _timers { get; set; } = new List<Timer>();


        public static async Task SetUpTimer(TimeSpan alertTime, Func<Task> action)
        {
            DateTime current = DateTime.Now;

            
            
            TimeSpan timeToGo = alertTime - current.TimeOfDay;



            await ApplicationLogger.Log($"Times to go {timeToGo} for action {action.Method}");

            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            var timer = new Timer(async x =>
            {
                await action();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);

            _timers.Add(timer); 
        }
    }
}
