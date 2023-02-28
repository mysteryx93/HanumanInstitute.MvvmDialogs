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

        ViewLocator = new StrongViewLocator() { ForceSinglePageNavigation = false }
            .Register<MainViewModel>(() => new MainView(), () => new MainWindow())
            .Register<CurrentTimeViewModel>(() => new CurrentTimeView(), () => new CurrentTimeWindow())
            .Register<ConfirmCloseViewModel>(() => new ConfirmCloseView(), () => new ConfirmCloseWindow());
        
        build.RegisterLazySingleton(() => (IDialogService)new DialogService(
            new DialogManager(
                viewLocator: ViewLocator,
                logger: loggerFactory.CreateLogger<DialogManager>(),
                dialogFactory: new DialogFactory().AddFluent(messageBoxType: FluentMessageBoxType.ContentDialog)),
            viewModelFactory: x => Locator.Current.GetService(x)));

        SplatRegistrations.Register<MainViewModel>();
        SplatRegistrations.Register<CurrentTimeViewModel>();
        SplatRegistrations.Register<ConfirmCloseViewModel>();
        SplatRegistrations.SetupIOC();
    }
    
    public override void OnFrameworkInitializationCompleted()
    {
        DialogService.Show(null, MainViewModel);

        base.OnFrameworkInitializationCompleted();
    }
    
    public static MainViewModel MainViewModel => Locator.Current.GetService<MainViewModel>()!;
    public static CurrentTimeViewModel CurrentTimeViewModel => Locator.Current.GetService<CurrentTimeViewModel>()!;
    public static ConfirmCloseViewModel ConfirmCloseViewModel => Locator.Current.GetService<ConfirmCloseViewModel>()!;
    private static IDialogService DialogService => Locator.Current.GetService<IDialogService>()!;
    public static StrongViewLocator ViewLocator { get; private set; } = default!;
}
