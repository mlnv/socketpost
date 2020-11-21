using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Websocket.Client;

namespace Socketpost.Services.WebSocket
{
    public class WebSocketService : IService
    {
        private IWebsocketClient client;

        public async Task Connect(string uri)
        {
            var factory = new Func<ClientWebSocket>(() =>
            {
                var client = new ClientWebSocket
                {
                    Options =
                    {
                        KeepAliveInterval = TimeSpan.FromSeconds(5),
                        // Proxy = ...
                        // ClientCertificates = ...
                    }
                };

                //client.Options.SetRequestHeader("Origin", "xxx");
                return client;
            });

            var url = new Uri(uri);

            using (client = new WebsocketClient(url, factory))
            {
                client.Name = "SampleClient";
                client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                client.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);

                client.ReconnectionHappened.Subscribe(type =>
                {
                    Console.WriteLine($"Reconnection happened, type: {type}, url: {client.Url}");
                });

                client.DisconnectionHappened.Subscribe(info =>
                    Console.WriteLine($"Disconnection happened, type: {info.Type}"));

                client.MessageReceived.Subscribe(msg =>
                {
                    Console.WriteLine($"Message received: {msg}");
                });

                Console.WriteLine("Starting...");
                await client.Start();
                Console.WriteLine("Started.");
            }
        }

        public async Task<bool> Disconnect()
        {
            if (client == null || !client.IsStarted || !client.IsRunning)
            {
                Console.WriteLine("The client wasn't connected, disctonnect asked but not needed.");
                return true;
            }

            Console.WriteLine($"Disconnecting the client {client.Name}.");
            return await client?.Stop(WebSocketCloseStatus.NormalClosure, WebSocketCloseStatus.NormalClosure.ToString());
        }

    }
}
