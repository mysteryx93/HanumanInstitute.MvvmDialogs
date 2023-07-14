using System.Windows.Forms;
using HanumanInstitute.MvvmDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using MessageBoxButton = HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxButton;
using MessageBoxImage = HanumanInstitute.MvvmDialogs.FrameworkDialogs.MessageBoxImage;
using HanumanInstitute.MvvmDialogs.FileSystem;
using System.Linq;

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
    public static DialogResult ShowDialog(this CommonDialog dialog, Window? owner) =>
        dialog.ShowDialog(owner != null ? new Win32Window(owner) : null);

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
    /// <returns>A value that specifies which message box button is clicked by the user. True=OK/Yes, False=No, Null=Cancel</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static bool? ShowMessageBox(
        this IDialogService service,
        INotifyPropertyChanged? ownerViewModel,
        string text,
        string title = "",
        MessageBoxButton button = MessageBoxButton.Ok,
        MessageBoxImage icon = MessageBoxImage.None,
        bool? defaultResult = true)
    {
        var settings = new MessageBoxSettings
        {
            Content = text,
            Title = title,
            Button = button,
            Icon = icon,
            DefaultValue = defaultResult
        };

        return ShowMessageBox(service, ownerViewModel, settings);
    }

    /// <summary>
    /// Displays a message box that has a message, title bar caption, button, and icon; and
    /// that accepts a default message box result and returns a result.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the message box dialog.</param>
    /// <returns>A value that specifies which message box button is clicked by the user. True=OK/Yes, False=No, Null=Cancel</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static bool? ShowMessageBox(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        MessageBoxSettings? settings = null)
    {
        return (bool?)service.DialogManager.AsSync().ShowFrameworkDialog(
            ownerViewModel, settings ?? new MessageBoxSettings());
    }

    /// <summary>
    /// Displays the OpenFileDialog to select a single file.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the open file dialog.</param>
    /// <returns>The file selected by the user, or null if the user cancelled.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static IDialogStorageFile? ShowOpenFileDialog(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        OpenFileDialogSettings? settings = null)
    {
        settings ??= new OpenFileDialogSettings();
        settings.AllowMultiple ??= false;
        return ShowOpenFilesDialog(service, ownerViewModel, settings).FirstOrDefault();
    }

    /// <summary>
    /// Displays the OpenFileDialog to select multiple files.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the open file dialog.</param>
    /// <returns>The list of files selected by the user, or empty if the user cancelled.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static IReadOnlyList<IDialogStorageFile> ShowOpenFilesDialog(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        OpenFileDialogSettings? settings = null)
    {
        settings ??= new OpenFileDialogSettings();
        settings.AllowMultiple ??= true;
        return service.DialogManager.AsSync().ShowFrameworkDialog(
            ownerViewModel, settings, x => string.Join(", ", x)) as IReadOnlyList<IDialogStorageFile> ?? Array.Empty<IDialogStorageFile>();
    }

    /// <summary>
    /// Displays the SaveFileDialog.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the save file dialog.</param>
    /// <returns>The file selected by the user, or null if the user cancelled.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static IDialogStorageFile? ShowSaveFileDialog(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        SaveFileDialogSettings? settings = null)
    {
        return (IDialogStorageFile?)service.DialogManager.AsSync().ShowFrameworkDialog(
            ownerViewModel, settings ?? new SaveFileDialogSettings());
    }

    /// <summary>
    /// Displays the FolderBrowserDialog.
    /// </summary>
    /// <param name="service">The IDialogService on which to attach the extension method.</param>
    /// <param name="ownerViewModel">A view model that represents the owner window of the dialog.</param>
    /// <param name="settings">The settings for the folder browser dialog.</param>
    /// <returns>The folder selected by the user, or null if the user cancelled.</returns>
    /// <exception cref="ViewNotRegisteredException">No view is registered with specified owner view model as data context.</exception>
    public static IDialogStorageFolder? ShowOpenFolderDialog(this IDialogService service, INotifyPropertyChanged? ownerViewModel,
        OpenFolderDialogSettings? settings = null)
    {
        return ((IReadOnlyList<IDialogStorageFolder>?)service.DialogManager.AsSync().ShowFrameworkDialog(
            ownerViewModel, settings ?? new OpenFolderDialogSettings())).FirstOrDefault();
    }
}
