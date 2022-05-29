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
    /// <param name="settings">The settings to pass to the <see cref="IFrameworkDialog{TResult}"/></param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    /// <param name="resultToString">A function to convert the result into a string for logging. If null, ToString will be used.</param>
    /// <typeparam name="TSettings">The settings type used to determine the implementation of <see cref="IFrameworkDialog{TResult}"/> to create.</typeparam>
    /// <typeparam name="TResult">The data type returned by the dialog.</typeparam>
    /// <returns>A framework dialog implementing <see cref="IFrameworkDialog{TResult}"/>.</returns>
    TResult ShowFrameworkDialog<TSettings, TResult>(INotifyPropertyChanged ownerViewModel, TSettings settings, AppDialogSettingsBase appSettings, Func<TResult, string>? resultToString = null)
        where TSettings : DialogSettingsBase;
}
