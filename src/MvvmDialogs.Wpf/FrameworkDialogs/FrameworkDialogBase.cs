using System;
using System.Threading.Tasks;
using HanumanInstitute.MvvmDialogs.Wpf;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf.Api;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

/// <summary>
/// Interface representing a framework dialog.
/// </summary>
/// <typeparam name="TSettings">The type of settings to use for this dialog.</typeparam>
/// <typeparam name="TResult">The data type returned by the dialog.</typeparam>
internal abstract class FrameworkDialogBase<TSettings, TResult> : IFrameworkDialog<TResult>, IFrameworkDialogSync<TResult>
{
    /// <summary>
    /// Gets the Win32 dialogs API interface.
    /// </summary>
    internal IFrameworkDialogsApi Api { get; }
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
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    protected FrameworkDialogBase(TSettings settings, AppDialogSettings appSettings)
        : this(settings, appSettings, new PathInfoFactory(), new FrameworkDialogsApi())
    {
    }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="settings">The settings for the framework dialog.</param>
    /// <param name="appSettings">Application-wide settings configured on the DialogService.</param>
    /// <param name="pathInfo">Provides information about files and directories.</param>
    /// <param name="api">An interface exposing Win32 framework dialogs.</param>
    internal FrameworkDialogBase(TSettings settings, AppDialogSettings appSettings, IPathInfoFactory pathInfo, IFrameworkDialogsApi api)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        PathInfo = pathInfo ?? throw new ArgumentNullException(nameof(pathInfo));
        Api = api ?? throw new ArgumentNullException(nameof(api));
    }

    /// <inheritdoc />
    public Task<TResult> ShowDialogAsync(IWindow owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (owner is not WindowWrapper window) throw new ArgumentException($"{nameof(owner)} must be of type {nameof(WindowWrapper)}");
        return ShowDialogAsync(window);
    }

    /// <inheritdoc />
    public TResult ShowDialog(IWindow owner)
    {
        if (owner == null) throw new ArgumentNullException(nameof(owner));
        if (owner is not WindowWrapper window) throw new ArgumentException($"{nameof(owner)} must be of type {nameof(WindowWrapper)}");
        return ShowDialog(window);
    }

    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <returns>true if user clicks the OK button; otherwise false.</returns>
    public abstract Task<TResult> ShowDialogAsync(WindowWrapper owner);

    /// <summary>
    /// Opens a framework dialog with specified owner.
    /// </summary>
    /// <param name="owner">Handle to the window that owns the dialog.</param>
    /// <returns>true if user clicks the OK button; otherwise false.</returns>
    public abstract TResult ShowDialog(WindowWrapper owner);
}
