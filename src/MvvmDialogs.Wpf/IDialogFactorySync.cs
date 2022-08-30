namespace HanumanInstitute.MvvmDialogs.Wpf;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
public interface IDialogFactorySync
{
    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    /// <returns>Return data specific to the dialog.</returns>
    object? ShowDialog<TSettings>(IView? owner, TSettings settings, AppDialogSettingsBase appSettings);
}
