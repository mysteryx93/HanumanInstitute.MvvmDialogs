using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.Wpf.StrongLocator;

/// <summary>
/// Maps view models to views in Avalonia.
/// </summary>
public class ViewLocator : StrongViewLocator
{
    public ViewLocator()
    {
        Register<MainWindowViewModel, MainWindow>();
        Register<AddTextDialogViewModel, AddTextDialog>();
    }
}
