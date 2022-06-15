namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
public interface IDialogFactory
{
    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    /// <typeparam name="TSettings">The type of settings to use for this dialog.</typeparam>
    /// <returns>Return data specific to the dialog.</returns>
    Task<object?> ShowDialogAsync<TSettings>(IWindow owner, TSettings settings, AppDialogSettingsBase appSettings);
}
