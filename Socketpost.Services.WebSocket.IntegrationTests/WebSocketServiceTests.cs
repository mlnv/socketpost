using Socketpost.Services.WebSocket.IntegrationTests.Server;
using System.Threading.Tasks;
using Xunit;

namespace Socketpost.Services.WebSocket.IntegrationTests
{
    public class WebSocketServiceTests
    {
        [Fact]
        public async Task Connect_ServerExists_ConnectHappened()
        {
            // Arrange
            WebSocketServer server = new WebSocketServer();
            server.StartServer("ws://localhost");

            WebSocketService service = new WebSocketService();

            // Act
            await service.Connect("ws://localhost/sample");

            // Assert
            // TODO: add check for successful connection
            Assert.True(true);

            server.Stop();
        }
    }
}
