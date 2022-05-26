using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia.Api;
using HanumanInstitute.MvvmDialogs.Avalonia;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Avalonia;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
/// <typeparam name="TSettings">The type of settings to use for this dialog.</typeparam>
/// <typeparam name="TResult">The data type returned by the dialog.</typeparam>
internal abstract class FrameworkDialogBase<TSettings, TResult> : IFrameworkDialog<TResult>
{
    /// <summary>
    /// Gets the Avalonia dialogs API interface.
    /// </summary>
    protected IFrameworkDialogsApi Api { get; }
    /// <summary>
    /// Provides information about files and directories
    /// </summary>
    protected IPathInfoFactory PathInfo { get; }
    /// <summary>
    /// Gets the settings for the framework dialog.
    /// </summary>
    protected TSettings Settings { get; }
    /// <summary>
    /// Gets application-wide settings.
    /// </summary>
    protected AppDialogSettings AppSettings { get; }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="api">An interface exposing Avalonia framework dialogs.</param>
    /// <param name="pathInfo">Provides information about files and directories.</param>
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    protected FrameworkDialogBase(IFrameworkDialogsApi? api, IPathInfoFactory? pathInfo, TSettings settings, AppDialogSettings appSettings)
    {
        Api = api ?? new FrameworkDialogsApi();
        PathInfo = pathInfo ?? new PathInfoFactory();
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
    }

    /// <inheritdoc />
    public Task<TResult> ShowDialogAsync(IWindow owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (owner is not WindowWrapper window) throw new ArgumentException($"{nameof(owner)} must be of type {nameof(WindowWrapper)}");
        return ShowDialogAsync(window);
    }

    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <returns>true if user clicks the OK button; otherwise false.</returns>
    public abstract Task<TResult> ShowDialogAsync(WindowWrapper owner);
}
