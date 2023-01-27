using Avalonia;
using Avalonia.Markup.Xaml;
using Demo.CrossPlatform.ViewModels;
using Demo.CrossPlatform.Views;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.Logging;
using Splat;

namespace Demo.CrossPlatform;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        var build = Locator.CurrentMutable;
        var loggerFactory = LoggerFactory.Create(builder => builder.AddFilter(logLevel => true).AddDebug());

        build.RegisterLazySingleton(() => (IDialogService)new DialogService(
            new DialogManager(
                viewLocator: new ViewLocator() { SinglePageNavigation = true },
                logger: loggerFactory.CreateLogger<DialogManager>()),
            viewModelFactory: x => Locator.Current.GetService(x)));

        SplatRegistrations.Register<MainWindow>();
        SplatRegistrations.Register<MainView>();
        SplatRegistrations.Register<MainViewModel>();
        SplatRegistrations.Register<CurrentTimeWindow>();
        SplatRegistrations.Register<CurrentTimeView>();
        SplatRegistrations.Register<CurrentTimeViewModel>();
        SplatRegistrations.SetupIOC();
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        DialogService.Show(null, MainViewModel);

        base.OnFrameworkInitializationCompleted();
    }
    
    public static MainViewModel MainViewModel => Locator.Current.GetService<MainViewModel>()!;
    public static CurrentTimeViewModel CurrentTimeViewModel => Locator.Current.GetService<CurrentTimeViewModel>()!;
    private static IDialogService DialogService => Locator.Current.GetService<IDialogService>()!;
}
