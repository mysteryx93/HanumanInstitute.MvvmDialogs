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
    /// <returns>The dialog button that was pressed.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<FAContentDialogResult> ShowContentDialogAsync(this IDialogService service, INotifyPropertyChanged ownerViewModel,
        ContentDialogSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(ownerViewModel);

        return (FAContentDialogResult)(await service.DialogManager.ShowFrameworkDialogAsync(
            ownerViewModel, settings ?? new ContentDialogSettings()).ConfigureAwait(true) ?? FAContentDialogResult.None);
    }

    /// <summary>
    /// Displays the FolderBrowserDialog.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the task dialog.</param>
    /// <returns>The dialog return value.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<FATaskDialogStandardResult> ShowTaskDialogAsync(this IDialogService service, INotifyPropertyChanged ownerViewModel,
        TaskDialogSettings? settings = null)
    {
        ArgumentNullException.ThrowIfNull(ownerViewModel);

        return (FATaskDialogStandardResult)(await service.DialogManager.ShowFrameworkDialogAsync(
            ownerViewModel, settings ?? new TaskDialogSettings()).ConfigureAwait(true) ?? FATaskDialogStandardResult.None);
    }
}
