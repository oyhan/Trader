using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Belem.Core.DTOs
{
    public class SetNewTradeDto
    {
        public TimeSpan BuyTime { get; set; }
        public TimeSpan SellTime { get; set; }
        public string Token { get; set; }
    }
}
