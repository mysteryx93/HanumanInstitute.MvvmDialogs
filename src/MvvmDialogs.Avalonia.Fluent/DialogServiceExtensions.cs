using System.ComponentModel;
using HanumanInstitute.MvvmDialogs.Avalonia.Fluent;
// ReSharper disable once CheckNamespace

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Provides IDialogService extensions for fluent dialogs.
/// </summary>
public static class DialogServiceExtensions
{
    /// <summary>
    /// Displays a FluentAvalonia content dialog.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the content dialog.</param>
    /// <param name="appSettings">Overrides application-wide settings configured on <see cref="IDialogService"/>.</param>
    /// <returns>The dialog button that was pressed.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<ContentDialogResult> ShowContentDialogAsync(this IDialogService service, INotifyPropertyChanged ownerViewModel,
        ContentDialogSettings? settings = null, AppDialogSettingsBase? appSettings = null)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));

        return (ContentDialogResult)(await service.DialogManager.ShowFrameworkDialogAsync(
            ownerViewModel, settings ?? new ContentDialogSettings(), appSettings ?? service.AppSettings).ConfigureAwait(true) ?? ContentDialogResult.None);
    }

    /// <summary>
    /// Displays the FolderBrowserDialog.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the task dialog.</param>
    /// <param name="appSettings">Overrides application-wide settings configured on <see cref="IDialogService"/>.</param>
    /// <returns>The dialog return value.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<TaskDialogStandardResult> ShowTaskDialogAsync(this IDialogService service, INotifyPropertyChanged ownerViewModel,
        TaskDialogSettings? settings = null, AppDialogSettingsBase? appSettings = null)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));

        return (TaskDialogStandardResult)(await service.DialogManager.ShowFrameworkDialogAsync(
            ownerViewModel, settings ?? new TaskDialogSettings(), appSettings ?? service.AppSettings).ConfigureAwait(true) ?? TaskDialogStandardResult.None);
    }
}
