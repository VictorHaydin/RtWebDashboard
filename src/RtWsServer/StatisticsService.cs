using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RtWsServer
{
    public class StatisticsService
    {
        private const int MAX_COUNT = 1000;
        private static readonly ConcurrentQueue<int> Roundtrips = new ConcurrentQueue<int>();
        private static int _recentRoundtrip = 0;

        public static void StoreRoundtripTime(int roundtripTime)
        {
            Roundtrips.Enqueue(roundtripTime);

            if (Roundtrips.Count > MAX_COUNT)
            {
                int x;
                Roundtrips.TryDequeue(out x);
            }

            _recentRoundtrip = roundtripTime;
        }

        public static Stats GetStats()
        {
            var res = new Stats { MaxRoundtrip = int.MinValue, MinRoundtrip = int.MaxValue };
            int sum = 0;
            res.RecentRoundtrip = _recentRoundtrip;
            foreach (var roundtrip in Roundtrips)
            {
                res.Count++;
                sum += roundtrip;
                if (res.MaxRoundtrip < roundtrip)
                {
                    res.MaxRoundtrip = roundtrip;
                }
                if (res.MinRoundtrip > roundtrip)
                {
                    res.MinRoundtrip = roundtrip;
                }
            }
            if (res.Count > 0)
            {
                res.AverageRoundtrip = sum/res.Count;
            }
            else
            {
                res.MaxRoundtrip = 0;
                res.MinRoundtrip = 0;
                res.AverageRoundtrip = 0;
            }
            return res;
        }
        
        public struct Stats
        {
            public int AverageRoundtrip { get; set; }
            public int MaxRoundtrip { get; set; }
            public int MinRoundtrip { get; set; }
            public int Count { get; set; }
            public int RecentRoundtrip { get; set; }
        }
    }
}
