using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alchemy;
using Alchemy.Classes;

namespace RtWsServer
{
    class Program
    {
        private static Random _rand = new Random();
        
        
        static void Main(string[] args)
        {
            var state = 10;
            using (var service = new WebSocketService())
            {
                while (true)
                {
                    PublishState(state, service);
                    PrintState(state);
                    state = ModifyState(state);
                    Thread.Sleep(500 + _rand.Next(500));
                }
            }
        }

        private static int ModifyState(int state)
        {
            var newState = state + _rand.Next(5) - 2;
            return newState >= 0 ? newState : 0;
        }

        private static void PrintState(int state)
        {
            Console.WriteLine(state);
        }

        private static void PublishState(int state, WebSocketService service)
        {
            service.Publish(state);
        }
    }
}
