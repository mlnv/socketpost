using NSubstitute;
using Socketpost.Services.WebSocket;
using Socketpost.WinApp.Models;
using Socketpost.WinApp.ViewModels;
using System;
using System.Linq;
using Xunit;

namespace Socketpost.WinApp.Tests.ViewModels
{
    public class MainWindowViewModelTests
    {
        private readonly IWebSocketService webSocketService;
        private readonly MainWindowViewModel viewModel;

        public MainWindowViewModelTests()
        {
            webSocketService = Substitute.For<IWebSocketService>();
            viewModel = new MainWindowViewModel(new MockDispatcher(), webSocketService);
        }

        [Fact]
        void Connect_ConnectCommandExecuted_ServiceConnectCalled()
        {
            // Arrange
            viewModel.Address = "ws://localhost:4040";

            // Act
            viewModel.ConnectCommand.Execute();

            // Assert
            webSocketService
                .Received()
                .Connect(viewModel.Address);
        }

        [Fact]
        void Connect_ConnectCommandExecutedWithEmptyAddress_ServiceConnectNotCalled()
        {
            // Arrange
            viewModel.Address = string.Empty;

            // Act
            viewModel.ConnectCommand.Execute();

            // Assert
            webSocketService
                .DidNotReceive()
                .Connect(viewModel.Address);
        }

        [Fact]
        void Connect_ConnectedEventOccurred_IsConnectedPropertyChanged()
        {
            // Arrange
            webSocketService
                .When(service => service.Connect(Arg.Any<string>()))
                .Do(callInfo =>
                {
                    webSocketService.OnConnected += Raise.Event<Action>();
                });

            // Act
            viewModel.ConnectCommand.Execute();

            // Assert
            Assert.True(viewModel.IsConnected);
        }

        [Fact]
        void Disconnect_DisconnectCommandExecuted_ServiceDisconnectCalled()
        {
            // Arrange
            // --

            // Act
            viewModel.DisconnectCommand.Execute();

            // Assert
            webSocketService
                .Received()
                .Disconnect();
        }

        [Fact]
        void Disconnect_DisconnectEventOccurred_IsConnectedPropertyChanged()
        {
            // Arrange
            webSocketService
                .When(service => service.Connect(Arg.Any<string>()))
                .Do(callInfo =>
                {
                    webSocketService.OnConnected += Raise.Event<Action>();
                });

            webSocketService
                .When(service => service.Disconnect())
                .Do(callInfo =>
                {
                    webSocketService.OnDisconnected += Raise.Event<Action>();
                });

            viewModel.ConnectCommand.Execute();

            // Act
            viewModel.DisconnectCommand.Execute();

            // Assert
            Assert.False(viewModel.IsConnected);
        }

        [Fact]
        void ReceivingMessages_MessageReceivedEventOccurred_OutputMessagesAdded()
        {
            // Arrange
            string messageData = "sample";

            // Act
            viewModel.ConnectCommand.Execute();

            webSocketService.MessageReceived += Raise.Event<Action<string>>(messageData);

            // Assert
            Assert.NotEmpty(viewModel.OutputMessages);

            Message message = viewModel.OutputMessages.Last();

            Assert.True(message.FromServer);
            Assert.False(message.Informational);
            Assert.Equal(messageData, message.Data);
        }

        [Fact]
        void CopyingMessageToSend_CopyToMessageForSendingCommandExecuted_MessageToSendChanged()
        {
            // Arrange
            viewModel.MessageContent = "sample content";

            // Act
            viewModel.CopyToMessageForSendingCommand.Execute();

            // Assert
            Assert.Equal(viewModel.MessageContent, viewModel.MessageToSend);
        }

        [Fact]
        void SelectingChangedCommand_SelectionChangedCommandExecuted_MessageContentChanged()
        {
            // Arrange
            Message message = new Message()
            {
                Data = "sample data"
            };

            // Act
            viewModel.SelectionChangedCommand.Execute(new[] { message });

            // Assert
            Assert.Equal(message.Data, viewModel.MessageContent);
        }

        [Fact]
        void SendingMessage_SendMessageCommandExecuted_ServiceSendCalled()
        {
            // Arrange
            viewModel.MessageToSend = "sample data";

            // Act
            viewModel.SendMessageCommand.Execute();

            // Assert
            webSocketService
                .Received()
                .Send(viewModel.MessageToSend);
        }
    }
}
