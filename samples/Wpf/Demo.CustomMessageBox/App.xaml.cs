using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.CustomMessageBox;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<IDialogService>(_ => new DialogService(dialogManager: new DialogManager(new CustomFrameworkDialogFactory())))
                .AddTransient<MainWindowViewModel>()
                .BuildServiceProvider());
    }
}
