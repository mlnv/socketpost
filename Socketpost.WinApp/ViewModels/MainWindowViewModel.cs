using Socketpost.WinApp.Models;
using Socketpost.Services.WebSocket;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Commands;
using System;
using System.Windows;

namespace Socketpost.WinApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public string Address { get; set; } = "ws://localhost:4040";

        public ObservableCollection<Message> OutputMessages { get; set; } = new ObservableCollection<Message>();

        public string MessageContent
        {
            get => messageContent;
            set => SetProperty(ref messageContent, value);
        }

        public bool IsConnected
        {
            get => isConnected;
            set => SetProperty(ref isConnected, value);
        }

        public string MessageToSend
        {
            get => messageToSend;
            set => SetProperty(ref messageToSend, value);
        }

        private readonly IService service;

        private string messageToSend;
        private string messageContent;
        private bool isConnected;

        public DelegateCommand ConnectCommand { get; private set; }
        public DelegateCommand DisconnectCommand { get; private set; }
        public DelegateCommand SendMessageCommand { get; private set; }
        public DelegateCommand CopyToMessageForSendingCommand { get; private set; }
        public DelegateCommand<object> SelectionChangedCommand { get; private set; }

        public MainWindowViewModel(IService service)
        {
            ConnectCommand = new DelegateCommand(Connect, () => !IsConnected).ObservesProperty(() => IsConnected);
            DisconnectCommand = new DelegateCommand(Disconnect, () => IsConnected).ObservesProperty(() => IsConnected);

            CopyToMessageForSendingCommand = new DelegateCommand(CopyToMessageForSending, () => !string.IsNullOrEmpty(MessageContent))
                .ObservesProperty(() => MessageContent);

            SendMessageCommand = new DelegateCommand(SendMessage, () => IsConnected && !string.IsNullOrEmpty(MessageToSend))
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => MessageToSend);

            SelectionChangedCommand = new DelegateCommand<object>(SelectionChanged);

            this.service = service;
        }

        public void Connect()
        {
            if (string.IsNullOrEmpty(Address))
            {
                OutputMessages.Add(new Message() 
                {
                    Informational = true,
                    Data = "Address is empty."
                });
                return;
            }

            SubscribeOnEvents();

            OutputMessages.Add(new Message()
            {
                Informational = true,
                Data = $"Connecting to {Address}."
            });
            service.Connect(Address);
        }

        public void Disconnect()
        {
            service.Disconnect();
            UnsubscribeFromEvents();

            OutputMessages.Add(new Message()
            {
                Informational = true,
                Data = $"Disconnected from {Address}."
            });
        }

        public void SendMessage()
        {
            OutputMessages.Add(new Message()
            {
                Data = MessageToSend
            });
            service.Send(MessageToSend);
        }

        public void CopyToMessageForSending()
        {
            MessageToSend = MessageContent;
        }

        private void SubscribeOnEvents()
        {
            service.MessageReceived += MessageReceived;
            service.OnConnected += Connected;
            service.OnDisconnected += Disconnected;
        }

        private void UnsubscribeFromEvents()
        {
            service.MessageReceived -= MessageReceived;
            service.OnConnected -= Connected;
            service.OnDisconnected -= Disconnected;
        }

        private void Disconnected()
        {
            IsConnected = false;
            OutputMessages.Add(new Message()
            {
                Informational = true,
                Data = $"Disconnected from {Address}."
            });
        }

        private void Connected()
        {
            IsConnected = true;
            OutputMessages.Add(new Message()
            {
                Informational = true,
                Data = $"Connected to {Address}."
            });
        }

        private void MessageReceived(string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => 
            {
                OutputMessages.Add(new Message()
                {
                    FromServer = true,
                    Data = MessageToSend
                });
            }));
        }

        private void SelectionChanged(object obj)
        {
            // TODO: Remove hardcoded conversion
            Message selectedMessage = (Message)((object[])obj)[0];
            MessageContent = selectedMessage.Data;
        }
    }
}
