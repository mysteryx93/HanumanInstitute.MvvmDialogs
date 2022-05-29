using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;
using Microsoft.Extensions.Logging;

namespace Demo.CustomOpenFileDialog;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddFilter(logLevel => true).AddDebug());

        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<IDialogService>(_ => new DialogService(
                    new DialogManager(
                        viewLocator: new ViewLocator(),
                        frameworkDialogFactory: new CustomFrameworkDialogFactory(),
                        logger: loggerFactory.CreateLogger<DialogManager>()),
                    viewModelFactory: x => Ioc.Default.GetService(x)))
                .AddTransient<MainWindowViewModel>()
                .BuildServiceProvider());
    }
}
