using System;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf.Api;
using HanumanInstitute.MvvmDialogs.Wpf;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs.Wpf;

/// <summary>
/// Default framework dialog factory that will create instances of standard Windows dialogs.
/// </summary>
public class FrameworkDialogFactory : IFrameworkDialogFactory
{
    private readonly IFrameworkDialogsApi api;
    private readonly IPathInfoFactory pathInfo;

    /// <summary>
    /// Initializes the FrameworkDialogFactory.
    /// </summary>
    public FrameworkDialogFactory() : this(null)
    {}

    /// <summary>
    /// Initializes the FrameworkDialogFactory.
    /// </summary>
    /// <param name="api">Optional. An interface exposing Win32 framework dialog API calls. Can be replaced with a mock for testing.</param>
    /// <param name="pathInfo">Optional. An interface providing information about file and directory paths. Can be replaced with a mock for unit testing.</param>
    internal FrameworkDialogFactory(IFrameworkDialogsApi? api = null, IPathInfoFactory? pathInfo = null)
    {
        this.api = api ?? new FrameworkDialogsApi();
        this.pathInfo = pathInfo ?? new PathInfoFactory();
    }

    /// <inheritdoc />
    public virtual IFrameworkDialog<TResult> Create<TSettings, TResult>(TSettings settings, AppDialogSettingsBase appSettings)
        where TSettings : DialogSettingsBase
    {
        var s2 = (AppDialogSettings)appSettings;
        return settings switch
        {
            MessageBoxSettings s => (IFrameworkDialog<TResult>)new MessageBox(api, pathInfo, s, s2),
            OpenFileDialogSettings s => (IFrameworkDialog<TResult>)new OpenFileDialog(api, pathInfo, s, s2),
            SaveFileDialogSettings s => (IFrameworkDialog<TResult>)new SaveFileDialog(api, pathInfo, s, s2),
            OpenFolderDialogSettings s => (IFrameworkDialog<TResult>)new OpenFolderDialog(api, pathInfo, s, s2),
            _ => throw new NotSupportedException()
        };
    }
}
