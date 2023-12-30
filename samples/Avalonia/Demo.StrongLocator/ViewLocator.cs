using HanumanInstitute.MvvmDialogs.Avalonia;

namespace Demo.Avalonia.StrongLocator;

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
