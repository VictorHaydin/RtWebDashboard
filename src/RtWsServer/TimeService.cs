using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtWsServer
{
    public class TimeService
    {
        private static readonly DateTime EpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public static long Now
        {
            get
            {
                return (long) (DateTime.Now - EpochStart).TotalMilliseconds;
            }
        }
    }
}
