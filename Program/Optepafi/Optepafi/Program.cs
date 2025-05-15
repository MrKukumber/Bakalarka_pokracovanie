using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Optepafi.ViewModels.Main;
using Optepafi.Views.Main.Windows;

namespace Optepafi;

/// <summary>
/// The entry point of the application.
/// </summary>
sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        const string mutexName = "Global\\com.github.MrKukumber.Optepafi";
        var mutex = new Mutex(true, mutexName, out bool createdNew);

        if (!createdNew)
        {
            BuildAvaloniaApp().SetupWithoutStarting();
            var msgWindow = new SimpleMessageWindow()
            {
                DataContext = new SimpleMessageWindowViewModel("Another instance of Optepafi is already running.")
            };
            CancellationTokenSource cts = new CancellationTokenSource();
            var ct = cts.Token;
            msgWindow.Closed += (_, __) => cts.Cancel();
            msgWindow.Show();
            Dispatcher.UIThread.MainLoop(ct);
            return;
        }
        
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args, ShutdownMode.OnMainWindowClose);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
    
}