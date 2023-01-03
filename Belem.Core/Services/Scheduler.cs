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


        public static void SetUpTimer(TimeSpan alertTime, Action action)
        {
            DateTime current = DateTime.Now;

            
            
            TimeSpan timeToGo = alertTime - current.TimeOfDay;



            Console.WriteLine($"Times to go {timeToGo} for action {action.Method}");

            if (timeToGo < TimeSpan.Zero)
            {
                return;//time already passed
            }
            var timer = new Timer(x =>
            {
                Console.WriteLine("FUCK!!@!@!@!@");
                action();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);

            _timers.Add(timer); 
        }
    }
}
