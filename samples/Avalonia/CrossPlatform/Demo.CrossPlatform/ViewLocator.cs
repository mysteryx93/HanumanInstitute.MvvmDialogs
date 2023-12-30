using Demo.CrossPlatform.ViewModels;
using Demo.CrossPlatform.Views;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace Demo.CrossPlatform;

/// <summary>
/// Maps view models to views in Avalonia.
/// </summary>
public class ViewLocator : StrongViewLocator
{
    public ViewLocator()
    {
        ForceSinglePageNavigation = false;
        Register<MainViewModel, MainView, MainWindow>();
        Register<CurrentTimeViewModel, CurrentTimeView, CurrentTimeWindow>();
        Register<ConfirmCloseViewModel, ConfirmCloseView, ConfirmCloseWindow>();
    }
}
