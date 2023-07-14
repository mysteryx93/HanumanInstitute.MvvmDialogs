using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Adds support for sync methods to IDialogManager.
/// </summary>
public interface IDialogManagerSync
{
    /// <summary>
    /// Shows a new dialog of specified type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="viewModel">The view model of the new dialog.</param>
    /// <returns>The dialog result.</returns>
    void ShowDialog(INotifyPropertyChanged ownerViewModel, IModalDialogViewModel viewModel);

    /// <summary>
    /// Shows a framework dialog whose type depends on the settings type.
    /// </summary>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings to pass to the <see cref="IDialogFactory"/></param>
    /// <param name="resultToString">A function to convert the result into a string for logging. If null, ToString will be used.</param>
    /// <typeparam name="TSettings">The settings type used to determine which dialog to show.</typeparam>
    /// <returns>The dialog result.</returns>
    object? ShowFrameworkDialog<TSettings>(INotifyPropertyChanged? ownerViewModel, TSettings settings, Func<object?, string>? resultToString = null)
        where TSettings : DialogSettingsBase;
}
