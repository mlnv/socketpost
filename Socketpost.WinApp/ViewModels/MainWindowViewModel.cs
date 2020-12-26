using Socketpost.WinApp.Models;
using Socketpost.Services.WebSocket;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Prism.Commands;
using System;
using System.Windows;
using Socketpost.Utilities;

namespace Socketpost.WinApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// The address to connect to.
        /// </summary>
        public string Address { get; set; } = "ws://localhost:4040";

        /// <summary>
        /// The messages from a connection between the client and a server.
        /// </summary>
        public ObservableCollection<Message> OutputMessages { get; set; } = new ObservableCollection<Message>();

        /// <summary>
        /// The selected message content.
        /// </summary>
        public string MessageContent
        {
            get => messageContent;
            set => SetProperty(ref messageContent, value);
        }

        /// <summary>
        /// Indicates whether or not the client is connected.
        /// </summary>
        public bool IsConnected
        {
            get => isConnected;
            set => SetProperty(ref isConnected, value);
        }

        /// <summary>
        /// The message to send to the server.
        /// </summary>
        public string MessageToSend
        {
            get => messageToSend;
            set => SetProperty(ref messageToSend, value);
        }

        private readonly IWebSocketService service;
        private readonly IDispatcher dispatcher;
        private string messageToSend;
        private string messageContent;
        private bool isConnected;

        /// <summary>
        /// Connect a client to a server command.
        /// </summary>
        public DelegateCommand ConnectCommand { get; private set; }

        /// <summary>
        /// Disconnect existing connection command.
        /// </summary>
        public DelegateCommand DisconnectCommand { get; private set; }

        /// <summary>
        /// Send message in existing connection command.
        /// </summary>
        public DelegateCommand SendMessageCommand { get; private set; }

        /// <summary>
        /// Copy selected message to send message holder.
        /// </summary>
        public DelegateCommand CopyToMessageForSendingCommand { get; private set; }

        /// <summary>
        /// Updated on every change of selected message.
        /// </summary>
        public DelegateCommand<object> SelectionChangedCommand { get; private set; }

        public MainWindowViewModel(IDispatcher dispatcher, IWebSocketService service)
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
            this.dispatcher = dispatcher;
        }

        private void Connect()
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

        private void Disconnect()
        {
            service.Disconnect();
            UnsubscribeFromEvents();

            OutputMessages.Add(new Message()
            {
                Informational = true,
                Data = $"Disconnected from {Address}."
            });
        }

        private void SendMessage()
        {
            OutputMessages.Add(new Message()
            {
                Data = MessageToSend
            });
            service.Send(MessageToSend);
        }

        private void CopyToMessageForSending()
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
            dispatcher.Dispatch(new Action(() => 
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
