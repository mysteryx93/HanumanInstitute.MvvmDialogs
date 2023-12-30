using HanumanInstitute.MvvmDialogs.Avalonia;

namespace Demo.Avalonia.FluentContentDialog;

/// <summary>
/// Maps view models to views in Avalonia.
/// </summary>
// public class ViewLocator : ViewLocatorBase
// {
//     /// <inheritdoc />
//     protected override string GetViewName(object viewModel) => viewModel.GetType().FullName!.Replace("ViewModel", "View");
// }

public class ViewLocator : StrongViewLocator
{
    public ViewLocator()
    {
        Register<AskTextBoxViewModel, AskTextBoxView>();
        Register<CurrentTimeViewModel, CurrentTimeView>();
        Register<MainViewModel, MainView>();
    }
}
