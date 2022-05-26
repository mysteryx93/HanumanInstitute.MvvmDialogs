using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.ApplicationLifetimes;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.DialogTypeLocators;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// DialogManager for Avalonia.
/// </summary>
public class DialogManager : DialogManagerBase<Window>
{
    /// <inheritdoc />
    public DialogManager(IFrameworkDialogFactory? frameworkDialogFactory = null)
        :
        base(frameworkDialogFactory ?? new FrameworkDialogFactory())
    {
    }

    /// <inheritdoc />
    protected override IWindow CreateWrapper(Window window) => window.AsWrapper();

    private static IEnumerable<Window> Windows =>
        (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Windows ?? Array.Empty<Window>();

    /// <inheritdoc />
    public override IWindow? FindWindowByViewModel(INotifyPropertyChanged viewModel) =>
        Windows.FirstOrDefault(x => ReferenceEquals(viewModel, x.DataContext)).AsWrapper();

    // /// <summary>
    // /// Returns the Window owning specified ViewModel.
    // /// </summary>
    // /// <param name="ownerViewModel">The ViewModel to get the Window for.</param>
    // /// <returns>A Window for the ViewModel..</returns>
    // protected Window? FindOwnerWindow(INotifyPropertyChanged ownerViewModel) =>
    //     (ViewRegistration.FindView(ownerViewModel) as WindowWrapper)?.Ref;
}
