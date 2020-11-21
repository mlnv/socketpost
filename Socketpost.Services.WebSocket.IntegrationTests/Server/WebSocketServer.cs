using WebSocketSharp;
using WebSocketSharp.Server;

namespace Socketpost.Services.WebSocket.IntegrationTests.Server
{
    internal class WebSocketServer
    {
        private WebSocketSharp.Server.WebSocketServer webSocketServer;
        public class SampleService : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                var msg = e.Data == "hello"
                          ? "universe"
                          : "hi";

                Send(msg);
            }
        }

        public void StartServer(string uri)
        {
            webSocketServer = new WebSocketSharp.Server.WebSocketServer(uri);
            webSocketServer.AddWebSocketService<SampleService>("/sample");
        }

        public void Stop()
        {
            webSocketServer?.Stop();
        }
    }
}
