using System;

namespace Socketpost.Services.WebSocket
{
    public interface IService
    {
        void Connect(string uri);
        void Disconnect();
        event Action<string> MessageReceived;
        void Send(string message);
    }
}
