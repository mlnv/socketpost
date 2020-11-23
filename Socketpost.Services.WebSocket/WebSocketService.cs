using System;

namespace Socketpost.Services.WebSocket
{
    public class WebSocketService : IService
    {
        private WebSocketSharp.WebSocket client;

        public event Action<string> MessageReceived;

        public void Connect(string uri)
        {
            client = new WebSocketSharp.WebSocket(uri);

            client.OnMessage += (sender, e) =>
            {
                MessageReceived?.Invoke(e.Data);
            };

            client.OnClose += (sender, e) =>
            {

            };

            client.OnOpen += (sender, e) =>
            {

            };

            client.OnError += (sender, e) =>
            {

            };

            client.Connect();
        }

        public void Disconnect()
        {
            if (client == null || !client.IsAlive)
            {
                Console.WriteLine("The client wasn't connected, disctonnect asked but not needed.");
            }

            Console.WriteLine($"Disconnecting the client for {client.Url}.");
            client?.Close();
        }

        public void Send(string message)
        {
            client?.Send(message);
        }
    }
}
