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
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    await ApplicationLogger.Log($"exception in scheduled task {action.Method.Name}** {ex} ");
                }
            }, null, timeToGo, Timeout.InfiniteTimeSpan);

            _timers.Add(timer);
        }

        public static async Task PeriodicTimer(TimeSpan alertTime, Func<Task> action, TimeSpan period)
        {
            DateTime current = DateTime.Now;

            TimeSpan timeToGo = alertTime - current.TimeOfDay < TimeSpan.Zero? period + (alertTime - current.TimeOfDay) : alertTime - current.TimeOfDay;

            await ApplicationLogger.Log($"Set schedule at  {alertTime} for action {action.Method}");

            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            var timer = new Timer(async x =>
            {
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    await ApplicationLogger.Log($"exception in scheduled task {action.Method.Name}** {ex} ");
                    await ApplicationLogger.Log("Trying to do it anther round");
                    try
                    {
                        await action();
                    }
                    catch (Exception ex2)
                    {
                        await ApplicationLogger.Log($"Second exception for task {action.Method.Name} we leave it alone then ... {ex2}" +
                            $"** {ex} ");

                        
                    }
                }
            }, null, timeToGo, period);

            _timers.Add(timer);
        }
    }
}
