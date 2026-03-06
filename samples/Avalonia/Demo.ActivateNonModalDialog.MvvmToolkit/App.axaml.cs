using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Demo.Avalonia.ActivateNonModalDialog;

public class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var services = new ServiceCollection();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddFilter(logLevel => true).AddDebug());

        services.AddSingleton<IDialogService>(sp => new DialogService(
            new DialogManager(
                viewLocator: new ViewLocator(),
                logger: loggerFactory.CreateLogger<DialogManager>()),
            viewModelFactory: sp.GetService));

        services.AddSingleton<MainWindowViewModel>(sp => new MainWindowViewModel(sp.GetService<IDialogService>()!));
        services.AddSingleton<CurrentTimeDialogViewModel>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        GC.KeepAlive(typeof(DialogService));
        DialogService.Show(null, MainWindow);

        base.OnFrameworkInitializationCompleted();
    }

    public static MainWindowViewModel MainWindow => ServiceProvider.GetService<MainWindowViewModel>()!;
    public static CurrentTimeDialogViewModel CurrentTimeDialog => ServiceProvider.GetService<CurrentTimeDialogViewModel>()!;
    public static IDialogService DialogService => ServiceProvider.GetService<IDialogService>()!;
}
