using System.Collections.Generic;
using SuperSocket.WebSocket.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Socketpost.Utilities.Server
{
    public class EchoWebSocketServer
    {
        private IHost host;

        public void StartServer(string address, int port)
        {
            host = WebSocketHostBuilder.Create()
            .UseWebSocketMessageHandler(
                async (session, message) =>
                {
                    await session.SendAsync(message.Message);
                }
            )
            .ConfigureAppConfiguration((hostCtx, configApp) =>
            {
                configApp.AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "serverOptions:name", "EchoWebSocketServer" },
                    { "serverOptions:listeners:0:ip", address },
                    { "serverOptions:listeners:0:port", port.ToString() }
                });
            })
            .ConfigureLogging((hostCtx, loggingBuilder) =>
            {
                loggingBuilder.AddConsole();
            })
            .Build();

            host.Start();
        }

        public void Stop()
        {
            host?.StopAsync().GetAwaiter().GetResult();
        }
    }
}
