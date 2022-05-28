using HanumanInstitute.MvvmDialogs.Wpf;

namespace Demo.CustomDialogTypeLocator;

/// <summary>
/// Maps view models to views in Avalonia.
/// </summary>
public class ViewLocator : ViewLocatorBase
{
    /// <inheritdoc />
    protected override string GetViewName(object viewModel) => viewModel.GetType().FullName!.Replace("VM", "");
}
