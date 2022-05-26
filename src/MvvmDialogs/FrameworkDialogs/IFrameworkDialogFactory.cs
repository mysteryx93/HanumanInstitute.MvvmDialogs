namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Interface responsible for creating framework dialogs.
/// </summary>
public interface IFrameworkDialogFactory
{
    /// <summary>
    /// Shows a framework dialog whose type depends on the settings type.
    /// </summary>
    /// <param name="settings">The settings to pass to the <see cref="IFrameworkDialog{TResult}"/></param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    /// <typeparam name="TSettings">The settings type used to determine the implementation of <see cref="IFrameworkDialog{TResult}"/> to create.</typeparam>
    /// <typeparam name="TResult">The data type returned by the dialog.</typeparam>
    /// <returns>A framework dialog implementing <see cref="IFrameworkDialog{TResult}"/>.</returns>
    IFrameworkDialog<TResult> Create<TSettings, TResult>(TSettings settings, AppDialogSettingsBase appSettings)
        where TSettings : DialogSettingsBase;
}
