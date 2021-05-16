using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Socketpost.DesktopApp.Utilities;
using Socketpost.DesktopApp.ViewModels;
using Socketpost.DesktopApp.Views;
using Socketpost.Services.WebSocket;

namespace Socketpost.DesktopApp
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(new ApplicationDispatcher(), new WebSocketService()),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
