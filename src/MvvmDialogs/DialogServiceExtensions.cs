using System.Collections.Generic;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

// ReSharper disable once CheckNamespace
namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Provides IDialogService extensions for standard dialog methods.
/// </summary>
public static class DialogServiceExtensions
{
    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon; and
    /// that accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="text">A <see cref="string"/> that specifies the text to display.</param>
    /// <param name="title">A <see cref="string"/> that specifies the title bar caption to display. Default value is an empty string.</param>
    /// <param name="button">A <see cref="MessageBoxButton"/> value that specifies which button or buttons to display.
    /// Default value is <see cref="MessageBoxButton.Ok"/>.</param>
    /// <param name="icon">A <see cref="MessageBoxImage"/> value that specifies the icon to display.
    /// Default value is <see cref="MessageBoxImage.None"/>.</param>
    /// <param name="defaultResult">Specifies the value of the button selected by default. Default value is true.</param>
    /// <param name="appSettings">Overrides application-wide settings configured on <see cref="IDialogService"/>.</param>
    /// <returns>A value that specifies which message box button is clicked by the user. True=OK/Yes, False=No, Null=Cancel</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static Task<bool?> ShowMessageBoxAsync(
        this IDialogService service,
        INotifyPropertyChanged? ownerViewModel,
        string text,
        string title = "",
        MessageBoxButton button = MessageBoxButton.Ok,
        MessageBoxImage icon = MessageBoxImage.None,
        bool? defaultResult = true,
        AppDialogSettingsBase? appSettings = null)
    {
        var settings = new MessageBoxSettings
        {
            Text = text,
            Title = title,
            Button = button,
            Icon = icon,
            DefaultValue = defaultResult
        };

        return ShowMessageBoxAsync(service, ownerViewModel, settings, appSettings ?? service.AppSettings);
    }

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon; and
    /// that accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the message box dialog.</param>
    /// <param name="appSettings">Overrides application-wide settings configured on <see cref="IDialogService"/>.</param>
    /// <returns>A value that specifies which message box button is clicked by the user. True=OK/Yes, False=No, Null=Cancel</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<bool?> ShowMessageBoxAsync(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        MessageBoxSettings? settings = null, AppDialogSettingsBase? appSettings = null)
    {
        return (bool?)await service.DialogManager.ShowFrameworkDialogAsync(
            ownerViewModel, settings ?? new MessageBoxSettings(), appSettings ?? service.AppSettings).ConfigureAwait(true);
    }

    /// <summary>
    /// Displays the OpenFileDialog to select a single file.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the open file dialog.</param>
    /// <param name="appSettings">Overrides application-wide settings configured on <see cref="IDialogService"/>.</param>
    /// <returns>The file selected by the user, or null if the user cancelled.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<IDialogStorageFile?> ShowOpenFileDialogAsync(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        OpenFileDialogSettings? settings = null, AppDialogSettingsBase? appSettings = null)
    {
        settings ??= new OpenFileDialogSettings();
        settings.AllowMultiple ??= false;
        var result = await ShowOpenFilesDialogAsync(service, ownerViewModel, settings, appSettings).ConfigureAwait(true);
        return result.Count > 0 ? result[0] : null;
    }

    /// <summary>
    /// Displays the OpenFileDialog to select multiple files.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the open file dialog.</param>
    /// <param name="appSettings">Overrides application-wide settings configured on <see cref="IDialogService"/>.</param>
    /// <returns>The list of files selected by the user, or null if the user cancelled.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<IReadOnlyList<IDialogStorageFile>> ShowOpenFilesDialogAsync(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        OpenFileDialogSettings? settings = null, AppDialogSettingsBase? appSettings = null)
    {
        settings ??= new OpenFileDialogSettings();
        settings.AllowMultiple ??= true;
        return (IReadOnlyList<IDialogStorageFile>)(await service.DialogManager.ShowFrameworkDialogAsync(
            ownerViewModel, settings, appSettings ?? service.AppSettings, x => string.Join(", ", x)).ConfigureAwait(true))!;
    }

    /// <summary>
    /// Displays the SaveFileDialog.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the save file dialog.</param>
    /// <param name="appSettings">Overrides application-wide settings configured on <see cref="IDialogService"/>.</param>
    /// <returns>The path to the file selected by the user, or null if the user cancelled.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<IDialogStorageFile?> ShowSaveFileDialogAsync(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        SaveFileDialogSettings? settings = null, AppDialogSettingsBase? appSettings = null)
    {
        return (IDialogStorageFile?)await service.DialogManager.ShowFrameworkDialogAsync(
            ownerViewModel, settings ?? new SaveFileDialogSettings(), appSettings ?? service.AppSettings).ConfigureAwait(true);
    }

    /// <summary>
    /// Displays the FolderBrowserDialog to select multiple folders.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    /// <param name="appSettings">Overrides application-wide settings configured on <see cref="IDialogService"/>.</param>
    /// <returns>The path of the folder selected by the user, or null if the user cancelled.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<IReadOnlyList<IDialogStorageFolder>> ShowOpenFoldersDialogAsync(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        OpenFolderDialogSettings? settings = null, AppDialogSettingsBase? appSettings = null)
    {
        return (IReadOnlyList<IDialogStorageFolder>)(await service.DialogManager.ShowFrameworkDialogAsync(
            ownerViewModel, settings ?? new OpenFolderDialogSettings(), appSettings ?? service.AppSettings).ConfigureAwait(true))!;
    }
    
    /// <summary>
    /// Displays the FolderBrowserDialog to select a single folder.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    /// <param name="appSettings">Overrides application-wide settings configured on <see cref="IDialogService"/>.</param>
    /// <returns>The path of the folder selected by the user, or null if the user cancelled.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static async Task<IDialogStorageFolder?> ShowOpenFolderDialogAsync(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        OpenFolderDialogSettings? settings = null, AppDialogSettingsBase? appSettings = null)
    {
        settings ??= new OpenFolderDialogSettings();
        settings.AllowMultiple ??= false;
        var result = await ShowOpenFoldersDialogAsync(service, ownerViewModel, settings, appSettings).ConfigureAwait(true);
        return result.Count > 0 ? result[0] : null;
    }
}
