using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using HanumanInstitute.MvvmDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Extension methods.
/// </summary>
public static class DialogServiceExtensions
{

    /// <summary>
    /// Displays a modal dialog of a type that is determined by the dialog type locator.
    /// </summary>
    /// <param name="service">The IDialogService.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <returns>A nullable value of type <see cref="bool"/> that signifies how a window was closed by the user.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static bool? ShowDialog(this IDialogService service, INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        service.AsSync().ShowDialog(ownerViewModel, viewModel);

    /// <summary>
    /// Displays a modal dialog of specified type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="service">The IDialogService.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <typeparam name="T">The type of the dialog to show.</typeparam>
    /// <returns>A nullable value of type <see cref="bool"/> that signifies how a window was closed by the user.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static bool? ShowDialog<T>(this IDialogService service, INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel) =>
        service.AsSync().ShowDialog(ownerViewModel, viewModel);

    /// <summary>
    /// Shows a modal dialog in a synchronous way.
    /// </summary>
    /// <param name="window">The window to show.</param>
    public static bool? ShowDialog(this Window window) =>
        window.ShowDialog();

    /// <summary>
    /// Shows a modal dialog in a synchronous way.
    /// </summary>
    /// <param name="dialog">The dialog to show.</param>
    /// <param name="owner">The owner of the modal dialog.</param>
    public static DialogResult ShowDialog(this CommonDialog dialog, Window owner) =>
        dialog.ShowDialog(new Win32Window(owner));
}
