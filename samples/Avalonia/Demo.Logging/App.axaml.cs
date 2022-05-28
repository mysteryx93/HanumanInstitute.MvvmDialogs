using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Splat;
using Microsoft.Extensions.Logging;

namespace Demo.Logging;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var build = Locator.CurrentMutable;

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
        });

        build.RegisterLazySingleton(() => (IDialogService)new DialogService(logger: loggerFactory.CreateLogger<DialogService>()));

        SplatRegistrations.Register<MainWindowViewModel>();
        SplatRegistrations.Register<AddTextDialogViewModel>();
        SplatRegistrations.SetupIOC();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        GC.KeepAlive(typeof(DialogService));
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = MainWindow
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static MainWindowViewModel MainWindow => Locator.Current.GetService<MainWindowViewModel>();
    public static AddTextDialogViewModel AddTextDialog => Locator.Current.GetService<AddTextDialogViewModel>();
}
