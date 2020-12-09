using Socketpost.Utilities.Server;
using System;

namespace Socketpost.ConsoleWebSocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            EchoWebSocketServer server = new EchoWebSocketServer();
            server.StartServer("0.0.0.0", 4040);

            Console.ReadLine();
        }
    }
}
