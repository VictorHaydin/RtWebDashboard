using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alchemy;
using Alchemy.Classes;

namespace RtWsServer
{
    class WebSocketService : IDisposable
    {
        private WebSocketServer _wsServer = new WebSocketServer(8181);
        private List<UserContext> _users = new List<UserContext>();
        private ConcurrentQueue<int> _messages = new ConcurrentQueue<int>();
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
            _wsServer.OnReceive += e => Console.WriteLine("Received data from client: {0}", e.DataFrame);
            _wsServer.Start();
            _workerThread.Start();
        }

        public void Dispose()
        {
            _workerThread.Abort();
            _wsServer.Dispose();
        }

        public void Publish(int state)
        {
            _messages.Enqueue(state);
        }

        private void ProcessMessages()
        {
            int message;
            while (true)
            {
                if (_messages.TryDequeue(out message))
                {
                    foreach (var userContext in _users)
                    {
                        userContext.Send(message.ToString());
                    }
                }
            }
        }
    }
}
