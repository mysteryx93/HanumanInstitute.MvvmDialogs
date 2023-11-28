using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.Avalonia.Api;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using HanumanInstitute.MvvmDialogs.PathInfo;

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
    public override async Task<object?> ShowDialogAsync<TSettings>(IView? owner, TSettings settings) =>
        settings switch
        {
            OpenFolderDialogSettings s => await ShowOpenFolderDialogAsync(owner, s).ConfigureAwait(true),
            OpenFileDialogSettings s => await ShowOpenFileDialogAsync(owner, s).ConfigureAwait(true),
            SaveFileDialogSettings s => await ShowSaveFileDialogAsync(owner, s).ConfigureAwait(true),
            _ => await base.ShowDialogAsync(owner, settings).ConfigureAwait(true)
        };

    private async Task<IReadOnlyList<IDialogStorageFolder>> ShowOpenFolderDialogAsync(IView? owner, OpenFolderDialogSettings settings)
    {
        var apiSettings = new FolderPickerOpenOptions()
        {
            Title = settings.Title,
            // Directory = settings.InitialDirectory
        };

        return await _api.ShowOpenFolderDialogAsync(owner.GetRef(), apiSettings).ConfigureAwait(true);
    }

    private async Task<IReadOnlyList<IDialogStorageFile>> ShowOpenFileDialogAsync(IView? owner, OpenFileDialogSettings settings)
    {
        var apiSettings = new FilePickerOpenOptions()
        {
            AllowMultiple = settings.AllowMultiple ?? false,
            FileTypeFilter = SyncFilters(settings.Filters)
            // d.ShowReadOnly = Settings.ShowReadOnly;
            // d.ReadOnlyChecked = Settings.ReadOnlyChecked;
        };
        AddSharedSettings(apiSettings, settings);

        return await _api.ShowOpenFileDialogAsync(owner.GetRef(), apiSettings).ConfigureAwait(true) ?? Array.Empty<IDialogStorageFile>();
    }

    private async Task<IDialogStorageFile?> ShowSaveFileDialogAsync(IView? owner, SaveFileDialogSettings settings)
    {
        var apiSettings = new FilePickerSaveOptions()
        {
            DefaultExtension = settings.DefaultExtension,
            FileTypeChoices = SyncFilters(settings.Filters),
            SuggestedFileName = settings.InitialFile
        };
        
        AddSharedSettings(apiSettings, settings);

        var result = await _api.ShowSaveFileDialogAsync(owner.GetRef(), apiSettings).ConfigureAwait(true);

        // Add DefaultExtension.
        // if (result != null && !string.IsNullOrEmpty(settings.DefaultExtension) && !_pathInfo.GetFileInfo(result).Exists && !result.Contains('.'))
        // {
        //     result += "." + settings.DefaultExtension.TrimStart('.');
        // }
        return result;
    }

    private void AddSharedSettings(PickerOptions d, FileDialogSettings s)
    {
        // d.DereferenceLinks = s.DereferenceLinks;
        // d.Directory = s.InitialDirectory;
        // d.InitialFileName = s.InitialFile;
        // d.Filters = SyncFilters(s.Filters);
        // d.SuggestedStartLocation = s.InitialDirectory;
        d.Title = s.Title;
    }

    private static List<FilePickerFileType> SyncFilters(List<FileFilter> filters) =>
        filters.Select(
            x => new FilePickerFileType(x.NameToString(x.ExtensionsToString()))
            {
                Patterns = x.Extensions?.Select(y => "*." + y.TrimStart('.')).ToList(),
                MimeTypes = x.MimeTypes,
                AppleUniformTypeIdentifiers = x.AppleUniformTypeIdentifiers
            }).ToList();
}
