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

        public string MessageContent
        {
            get => messageContent;
            set => this.RaiseAndSetIfChanged(ref messageContent, value);
        }

        public bool IsConnected
        {
            get => isConnected;
            set => this.RaiseAndSetIfChanged(ref isConnected, value);
        }

        public string Message { get; set; }

        public IService Service { get; private set; }

        private string output;
        private string messageContent;
        private bool isConnected;

        public void OnConnectCommand()
        {
            if (string.IsNullOrEmpty(Address))
            {
                Output += "Address is empty.\n";
                return;
            }

            Service = new WebSocketService();
            SubscribeOnEvents();

            Output += $"Connecting to {Address}.\n";
            Service.Connect(Address);
        }

        public void OnDisconnectCommand()
        {
            Service.Disconnect();
            UnsubscribeFromEvents();
            Output += $"Disconnected from {Address}.\n";
        }

        public void OnSendMessageCommand()
        {
            if (Service == null)
            {
                Output += "Not connected to server.\n";
                return;
            }

            Output += $"Sending to server: {Message}.\n";
            Service.Send(Message);
        }

        public void OnCopyToMessageForSendingCommand()
        {
            Message = MessageContent;
        }

        private void SubscribeOnEvents()
        {
            Service.MessageReceived += MessageReceived;
            Service.OnConnected += Connected;
            Service.OnDisconnected += Disconnected;
        }

        private void UnsubscribeFromEvents()
        {
            Service.MessageReceived -= MessageReceived;
            Service.OnConnected -= Connected;
            Service.OnDisconnected -= Disconnected;
        }

        private void Disconnected()
        {
            IsConnected = false;
        }

        private void Connected()
        {
            IsConnected = true;
        }

        private void MessageReceived(string message)
        {
             Output += $"Message received: {message}.\n";
        }
    }
}
