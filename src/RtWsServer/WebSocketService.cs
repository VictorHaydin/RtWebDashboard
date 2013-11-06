using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alchemy;
using Alchemy.Classes;
using Newtonsoft.Json;

namespace RtWsServer
{
    class WebSocketService : IDisposable
    {
        private WebSocketServer _wsServer = new WebSocketServer(8181);
        private List<UserContext> _users = new List<UserContext>();
        private ConcurrentQueue<Message> _messages = new ConcurrentQueue<Message>();
        private Thread _workerThread;

        public WebSocketService()
        {
            _workerThread = new Thread(ProcessMessages);
            _wsServer.OnConnected += context =>
            {
                Console.WriteLine("Connected: {0}", context.ClientAddress);
                _users.Add(context); // is this thread safe?
            };
            _wsServer.OnDisconnect += context => Console.WriteLine("Disconnected: {0}", context.ClientAddress);
            _wsServer.OnReceive += e =>
            {
                var message = JsonConvert.DeserializeObject<Message>(e.DataFrame.ToString());
                StatisticsService.StoreRoundtripTime((int)(TimeService.Now - message.SentTimestamp));
            };
            _wsServer.Start();
            _workerThread.Start();
        }

        public void Dispose()
        {
            _workerThread.Abort();
            _wsServer.Dispose();
        }

        public void Publish(Message state)
        {
            _messages.Enqueue(state);
        }

        private void ProcessMessages()
        {
            while (true)
            {
                Message message;
                if (_messages.TryDequeue(out message))
                {
                    foreach (var userContext in _users)
                    {
                        userContext.Send(JsonConvert.SerializeObject(message));
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
