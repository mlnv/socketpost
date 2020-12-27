using System;

namespace Socketpost.Services.WebSocket
{
    /// <inheritdoc/>
    public class WebSocketService : IWebSocketService
    {
        /// <inheritdoc/>
        public event Action<string> MessageReceived;

        /// <inheritdoc/>
        public event Action OnConnected;

        /// <inheritdoc/>
        public event Action OnDisconnected;

        /// <inheritdoc/>
        public event Action<string> OnError;

        private WebSocketSharp.WebSocket client;

        /// <inheritdoc/>
        public void Connect(string uri)
        {
            client = new WebSocketSharp.WebSocket(uri);

            client.OnMessage += (sender, e) =>
            {
                MessageReceived?.Invoke(e.Data);
            };

            client.OnClose += (sender, e) =>
            {
                OnDisconnected?.Invoke();
            };

            client.OnOpen += (sender, e) =>
            {
                OnConnected?.Invoke();
            };

            client.OnError += (sender, e) =>
            {
                OnError?.Invoke(e.Message);
            };

            client.Connect();
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            if (client == null || !client.IsAlive)
            {
                Console.WriteLine("The client wasn't connected, disctonnect asked but not needed.");
            }

            Console.WriteLine($"Disconnecting the client for {client.Url}.");
            client?.Close();
        }

        /// <inheritdoc/>
        public void Send(string message)
        {
            client?.Send(message);
        }
    }
}
