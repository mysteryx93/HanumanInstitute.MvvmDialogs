using System.Collections.Generic;
using System.Linq;
using HanumanInstitute.MvvmDialogs.Avalonia.Api;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Handles OpenFileDialog, SaveFileDialog and OpenFolderDialog for Avalonia.
/// </summary>
public class DialogFactory : DialogFactoryBase
{
    private readonly IFrameworkDialogsApi _api;
    private readonly IPathInfoFactory _pathInfo;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public DialogFactory(IDialogFactory? chain = null)
        : this(chain, null, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    /// <param name="api">An interface exposing Avalonia framework dialogs.</param>
    /// <param name="pathInfo">Provides information about files and directories.</param>
    internal DialogFactory(IDialogFactory? chain, IFrameworkDialogsApi? api, IPathInfoFactory? pathInfo)
        : base(chain)
    {
        _api = api ?? new FrameworkDialogsApi();
        _pathInfo = pathInfo ?? new PathInfoFactory();
    }

    /// <inheritdoc />
    public override async Task<object?> ShowDialogAsync<TSettings>(WindowWrapper owner, TSettings settings, AppDialogSettings appSettings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await ShowOpenFolderDialogAsync(owner, s, appSettings),
            OpenFileDialogSettings s => await ShowOpenFileDialogAsync(owner, s, appSettings),
            SaveFileDialogSettings s => await ShowSaveFileDialogAsync(owner, s, appSettings),
            _ => base.ShowDialogAsync(owner, settings, appSettings)
        };

    private async Task<string?> ShowOpenFolderDialogAsync(WindowWrapper owner, OpenFolderDialogSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new OpenFolderApiSettings()
        {
            Title = settings.Title,
            Directory = settings.InitialDirectory
            // d.ShowNewFolderButton = Settings.ShowNewFolderButton;
        };

        return await _api.ShowOpenFolderDialog(owner.Ref, apiSettings).ConfigureAwait(false);
    }

    private async Task<string[]> ShowOpenFileDialogAsync(WindowWrapper owner, OpenFileDialogSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new OpenFileApiSettings()
        {
            AllowMultiple = settings.AllowMultiple ?? false
            // d.ShowReadOnly = Settings.ShowReadOnly;
            // d.ReadOnlyChecked = Settings.ReadOnlyChecked;
        };
        AddSharedSettings(apiSettings, settings);

        return await _api.ShowOpenFileDialog(owner.Ref, apiSettings).ConfigureAwait(false) ?? Array.Empty<string>();
    }

    private async Task<string?> ShowSaveFileDialogAsync(WindowWrapper owner, SaveFileDialogSettings settings, AppDialogSettings appSettings)
    {
        var apiSettings = new SaveFileApiSettings()
        {
            DefaultExtension = settings.DefaultExtension
            // d.CreatePrompt = Settings.CreatePrompt;
            // d.OverwritePrompt = Settings.OverwritePrompt;
        };
        AddSharedSettings(apiSettings, settings);

        var result = await _api.ShowSaveFileDialog(owner.Ref, apiSettings).ConfigureAwait(false);
        return result;
    }

    private void AddSharedSettings(FileApiSettings d, FileDialogSettings s)
    {
        // s.DefaultExtension
        // d.DereferenceLinks = s.DereferenceLinks;
        // d.CheckFileExists = s.CheckFileExists;
        // d.CheckPathExists = s.CheckPathExists;
        d.Directory = s.InitialDirectory;
        d.InitialFileName = s.InitialFile;
        d.Filters = SyncFilters(s.Filters);
        d.Title = s.Title;
    }

    private static List<FileDialogFilter> SyncFilters(List<FileFilter> filters) =>
        filters.Select(
            x => new FileDialogFilter()
            {
                Name = x.NameToString(x.ExtensionsToString()),
                Extensions = x.Extensions
            }).ToList();
}
