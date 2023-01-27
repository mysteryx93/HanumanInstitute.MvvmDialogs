namespace Demo.ActivateNonModalDialog;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddFilter(logLevel => true).AddDebug());

        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<IDialogService>(new DialogService(
                    new DialogManager(
                        viewLocator: new ViewLocator(),
                        logger: loggerFactory.CreateLogger<DialogManager>()),
                    viewModelFactory: x => Ioc.Default.GetService(x)))
                .AddTransient<MainWindowViewModel>()
                .BuildServiceProvider());

        var dialogService = Ioc.Default.GetRequiredService<IDialogService>();
        var vm = dialogService.CreateViewModel<MainWindowViewModel>();
        dialogService.Show(null, vm);
        Application.Current.MainWindow = Application.Current.Windows[0];
    }
}
