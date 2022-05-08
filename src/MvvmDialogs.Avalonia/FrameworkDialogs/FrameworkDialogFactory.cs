using System;
using HanumanInstitute.MvvmDialogs.Avalonia.FrameworkDialogs.Api;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Avalonia.FrameworkDialogs;

/// <summary>
/// Default framework dialog factory that will create instances of standard Windows dialogs.
/// </summary>
public class FrameworkDialogFactory : IFrameworkDialogFactory
{
    private readonly IFrameworkDialogsApi _api;
    private readonly IPathInfoFactory _pathInfo;

    public FrameworkDialogFactory() : this(null, null)
    {}

    /// <summary>
    /// Initializes the FrameworkDialogFactory.
    /// </summary>
    /// <param name="api">Optional. An interface exposing Avalonia framework dialog API calls. Can be replaced with a mock for unit testing.</param>
    /// <param name="pathInfo">Optional. An interface providing information about file and directory paths. Can be replaced with a mock for unit testing.</param>
    internal FrameworkDialogFactory(IFrameworkDialogsApi? api = null, IPathInfoFactory? pathInfo = null)
    {
        this._api = api ?? new FrameworkDialogsApi();
        this._pathInfo = pathInfo ?? new PathInfoFactory();
    }

    /// <inheritdoc />
    public virtual IFrameworkDialog<TResult> Create<TSettings, TResult>(TSettings settings, AppDialogSettingsBase appSettings)
        where TSettings : DialogSettingsBase
    {
        var s2 = (AppDialogSettings)appSettings;
        return settings switch
        {
            MessageBoxSettings s => (IFrameworkDialog<TResult>)new MessageBox(_api, _pathInfo, s, s2),
            OpenFileDialogSettings s => (IFrameworkDialog<TResult>)new OpenFileDialog(_api, _pathInfo, s, s2),
            SaveFileDialogSettings s => (IFrameworkDialog<TResult>)new SaveFileDialog(_api, _pathInfo, s, s2),
            OpenFolderDialogSettings s => (IFrameworkDialog<TResult>)new OpenFolderDialog(_api, _pathInfo, s, s2),
            _ => throw new NotSupportedException()
        };
    }
}
