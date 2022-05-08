using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.CustomDialogTypeLocator;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Ioc.Default.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<IDialogService>(new DialogService(dialogTypeLocator: new MyCustomDialogTypeLocator()))
                .AddTransient<MainWindowVM>()
                .BuildServiceProvider());
    }
}
