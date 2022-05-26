using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.NonModalCustomDialog;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<IDialogService, DialogService>()
                .AddTransient<MainWindowViewModel>()
                .BuildServiceProvider());
    }
}
