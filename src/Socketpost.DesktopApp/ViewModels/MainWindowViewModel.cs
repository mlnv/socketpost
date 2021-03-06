using Avalonia.Controls.Selection;
using ReactiveUI;
using Socketpost.DesktopApp.Models;
using Socketpost.Services.WebSocket;
using Socketpost.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace Socketpost.DesktopApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
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
            set => this.RaiseAndSetIfChanged(ref messageContent, value);
        }

        /// <summary>
        /// Indicates whether or not the client is connected.
        /// </summary>
        public bool IsConnected
        {
            get => isConnected;
            set => this.RaiseAndSetIfChanged(ref isConnected, value);
        }

        /// <summary>
        /// The message to send to the server.
        /// </summary>
        public string MessageToSend
        {
            get => messageToSend;
            set => this.RaiseAndSetIfChanged(ref messageToSend, value);
        }

        public SelectionModel<Message> OutputMessagesSelection { get; }

        private readonly IWebSocketService service;
        private readonly IDispatcher dispatcher;
        private string messageToSend;
        private string messageContent;
        private bool isConnected;

        /// <summary>
        /// Connect a client to a server command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> ConnectCommand { get; }

        /// <summary>
        /// Disconnect existing connection command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> DisconnectCommand { get; private set; }

        /// <summary>
        /// Send message in existing connection command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> SendMessageCommand { get; private set; }

        public MainWindowViewModel(IDispatcher dispatcher, IWebSocketService service)
        {
            ConnectCommand = ReactiveCommand
                .Create(Connect,
                canExecute: this.WhenAnyValue(x => x.IsConnected, (isConnected) => !isConnected));

            DisconnectCommand = ReactiveCommand
                .Create(Disconnect,
                canExecute: this.WhenAnyValue(x => x.IsConnected, (isConnected) => isConnected == true));

            SendMessageCommand = ReactiveCommand
                .Create(SendMessage,
                canExecute: this.WhenAnyValue(x => x.IsConnected, y => y.MessageToSend,
                    (connected, messageToSend) => connected && !string.IsNullOrEmpty(messageToSend)));

            this.service = service;
            this.dispatcher = dispatcher;

            OutputMessagesSelection = new SelectionModel<Message>();
            OutputMessagesSelection.SelectionChanged += OutputMessagesSelectionChanged;
        }

        private void OutputMessagesSelectionChanged(object? sender, SelectionModelSelectionChangedEventArgs<Message> e)
        {
            var selectedMessage = e.SelectedItems.FirstOrDefault();

            if (selectedMessage == null)
            {
                return;
            }

            MessageContent = selectedMessage.Data;
        }

        private void Connect()
        {
            if (string.IsNullOrEmpty(Address))
            {
                OutputMessages.Add(new Message()
                {
                    Type = MessageType.Informational,
                    Data = "Address is empty."
                });
                return;
            }

            SubscribeOnEvents();

            OutputMessages.Add(new Message()
            {
                Type = MessageType.Informational,
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
                Type = MessageType.Informational,
                Data = $"Disconnected from {Address}."
            });
        }

        private void SendMessage()
        {
            OutputMessages.Add(new Message()
            {
                Type = MessageType.Sent,
                Data = MessageToSend
            });
            service.Send(MessageToSend);
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
                Type = MessageType.Informational,
                Data = $"Disconnected from {Address}."
            });
        }

        private void Connected()
        {
            IsConnected = true;
            OutputMessages.Add(new Message()
            {
                Type = MessageType.Informational,
                Data = $"Connected to {Address}."
            });
        }

        private void MessageReceived(string message)
        {
            dispatcher.Dispatch(new Action(() =>
            {
                OutputMessages.Add(new Message()
                {
                    Type = MessageType.Received,
                    Data = message
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
