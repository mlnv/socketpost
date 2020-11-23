using Socketpost.Utilities.Server;
using System.Threading;
using Xunit;

namespace Socketpost.Services.WebSocket.IntegrationTests
{
    public class WebSocketServiceTests
    {
        [Fact]
        public void Connect_ServerExists_MessageEchoed()
        {
            // Arrange
            EchoWebSocketServer server = new EchoWebSocketServer();
            server.StartServer("0.0.0.0", 4040);

            WebSocketService service = new WebSocketService();

            string messageToSend = "hello";
            string receivedMessage = string.Empty;

            service.MessageReceived += (message) =>
            {
                receivedMessage = message;
            };

            // Act
            service.Connect("ws://localhost:4040");
            service.Send(messageToSend);

            Thread.Sleep(100);

            // Assert
            Assert.Equal(messageToSend, receivedMessage);

            // Clean up
            server.Stop();
        }
    }
}
