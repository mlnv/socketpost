using ReactiveUI;
using Socketpost.Services.WebSocket;
using System;

namespace Socketpost.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Address { get; set; } = "ws://localhost:4040";

        public string Output
        {
            get => output;
            set => this.RaiseAndSetIfChanged(ref output, value);
        }

        public string Message { get; set; }

        public IService Service;

        private string output;

        public void OnConnectCommand()
        {
            if (string.IsNullOrEmpty(Address))
            {
                Output += "\n Address is empty.";
                return;
            }

            Service = new WebSocketService();
            Service.MessageReceived += MessageReceived;

            Output += $"\n Connecting to {Address}.";
            Service.Connect(Address);
        }

        public void OnSendMessageCommand()
        {
            if (Service == null)
            {
                Output += "\n Not connected to server.";
                return;
            }

            Output += $"\n Sending to server: {Message}.";
            Service.Send(Message);
        }

        private void MessageReceived(string message)
        {
             Output += $"\n Message received: {message}.";
        }
    }
}
