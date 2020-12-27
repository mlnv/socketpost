using Prism.DryIoc;
using Prism.Ioc;
using Socketpost.Services.WebSocket;
using Socketpost.Utilities;
using Socketpost.WinApp.Utilities;
using Socketpost.WinApp.Views;
using System.Windows;

namespace Socketpost.WinApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            var window = Container.Resolve<MainWindow>();
            return window;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDispatcher, ApplicationDispatcher>();
            containerRegistry.RegisterSingleton<IWebSocketService, WebSocketService>();
        }
    }
}
