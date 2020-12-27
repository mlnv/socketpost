using System;

namespace Socketpost.Services.WebSocket
{
    /// <summary>
    /// WebSocket service.
    /// </summary>
    public interface IWebSocketService
    {
        /// <summary>
        /// The event that occured on connection established completed.
        /// </summary>
        event Action OnConnected;

        /// <summary>
        /// The event that occured on disconnection completed.
        /// </summary>
        event Action OnDisconnected;

        /// <summary>
        /// The event that occured when a message received.
        /// </summary>
        event Action<string> MessageReceived;

        /// <summary>
        /// The event that occured or error.
        /// </summary>
        event Action<string> OnError;

        /// <summary>
        /// Connects to the given WebSocket server.
        /// </summary>
        /// <param name="uri">Uri to connect to</param>
        void Connect(string uri);

        /// <summary>
        /// Disconnects from the connected server.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sends the message
        /// </summary>
        /// <param name="message">The message to send</param>
        void Send(string message);
    }
}
