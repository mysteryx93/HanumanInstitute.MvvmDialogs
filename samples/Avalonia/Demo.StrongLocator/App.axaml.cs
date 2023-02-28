using System;
using Avalonia;
using Avalonia.Markup.Xaml;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.Logging;
using Splat;

namespace Demo.Avalonia.StrongLocator;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var build = Locator.CurrentMutable;
        var loggerFactory = LoggerFactory.Create(builder => builder.AddFilter(logLevel => true).AddDebug());

        ViewLocator = new StrongViewLocator()
            .Register<MainWindowViewModel, MainWindow>()
            .Register<AddTextDialogViewModel, AddTextDialog>();

        build.RegisterLazySingleton(() => (IDialogService)new DialogService(
            new DialogManager(
                viewLocator: ViewLocator,
                logger: loggerFactory.CreateLogger<DialogManager>()),
            viewModelFactory: x => Locator.Current.GetService(x)));

        SplatRegistrations.Register<MainWindowViewModel>();
        SplatRegistrations.Register<AddTextDialogViewModel>();
        SplatRegistrations.SetupIOC();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        GC.KeepAlive(typeof(DialogService));
        DialogService.Show(null, MainWindow);

        base.OnFrameworkInitializationCompleted();
    }

    public static MainWindowViewModel MainWindow => Locator.Current.GetService<MainWindowViewModel>()!;
    public static AddTextDialogViewModel AddTextDialog => Locator.Current.GetService<AddTextDialogViewModel>()!;
    public static IDialogService DialogService => Locator.Current.GetService<IDialogService>()!;
    public static StrongViewLocator ViewLocator { get; private set; } = default!;
}
