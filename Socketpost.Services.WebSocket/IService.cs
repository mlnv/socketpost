using System;

namespace Socketpost.Services.WebSocket
{
    public interface IService
    {
        event Action OnConnected;
        event Action OnDisconnected;
        event Action<string> MessageReceived;

        void Connect(string uri);
        void Disconnect();
        void Send(string message);
    }
}
