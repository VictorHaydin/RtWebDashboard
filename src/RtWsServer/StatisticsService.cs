using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtWsServer
{
    class StatisticsService
    {
        private static readonly ConcurrentBag<long> Roundtrips = new ConcurrentBag<long>();

        public static void StoreRoundtripTime(long roundtripTime)
        {
            Roundtrips.Add(roundtripTime);
        }

        public static long GetAverageRoundtripTime()
        {
            foreach (var roundtrip in Roundtrips)
            {
                Console.WriteLine(roundtrip);
            }
            return Roundtrips.Sum()/Roundtrips.Count; // not sure if this is thread safe, but for now this is ok
        }

        public static long GetMaxRoundtripTime()
        {
            return Roundtrips.Max();
        }

        public static long GetMinRoundtripTime()
        {
            return Roundtrips.Min();
        }
    }
}
