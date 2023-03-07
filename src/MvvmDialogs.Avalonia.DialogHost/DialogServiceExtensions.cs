using System.ComponentModel;
using HanumanInstitute.MvvmDialogs.Avalonia.DialogHost;

// ReSharper disable once CheckNamespace

namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Provides IDialogService extensions for fluent dialogs.
/// </summary>
public static class DialogServiceExtensions
{
    /// <summary>
    /// Displays a DialogHost.Avalonia content dialog.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the content dialog.</param>
    /// <param name="appSettings">Overrides application-wide settings configured on <see cref="IDialogService"/>.</param>
    /// <returns>The dialog button that was pressed.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<object?> ShowDialogHostAsync(this IDialogService service, INotifyPropertyChanged ownerViewModel,
        DialogHostSettings? settings = null, AppDialogSettingsBase? appSettings = null)
    {
        if (ownerViewModel == null) throw new ArgumentNullException(nameof(ownerViewModel));
        if (settings == null) throw new ArgumentNullException(nameof(settings));
        if (settings.ContentViewModel == null) throw new ArgumentNullException(nameof(settings.ContentViewModel));

        return await service.DialogManager.ShowFrameworkDialogAsync(
            ownerViewModel, settings, appSettings ?? service.AppSettings).ConfigureAwait(true);
    }
}
