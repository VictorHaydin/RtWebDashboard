using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtWsServer
{
    public class Message
    {
        public int Data { get; set; }
        public long SentTimestamp { get; set; }
        public long ReturnedTimestamp { get; set; }
        public StatisticsService.Stats Stats { get; set; }
    }
}
