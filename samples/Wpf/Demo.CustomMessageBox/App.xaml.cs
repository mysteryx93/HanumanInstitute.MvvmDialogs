using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;
using Microsoft.Extensions.Logging;

namespace Demo.CustomMessageBox;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddFilter(logLevel => true).AddDebug());

        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<IDialogService>(_ => new DialogService(
                    dialogManager: new DialogManager(new CustomFrameworkDialogFactory(), logger: loggerFactory.CreateLogger<DialogManager>()), new
                    ViewLocator()))
                .AddTransient<MainWindowViewModel>()
                .BuildServiceProvider());
    }
}
