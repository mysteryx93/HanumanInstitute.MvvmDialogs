using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.Avalonia.Api;
using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Handles OpenFileDialog, SaveFileDialog and OpenFolderDialog for Avalonia.
/// </summary>
public class DialogFactory : DialogFactoryBase
{
    private readonly IFrameworkDialogsApi _api;
    // private readonly IPathInfoFactory _pathInfo;

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    public DialogFactory(IDialogFactory? chain = null)
        : this(chain, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of a FrameworkDialog.
    /// </summary>
    /// <param name="chain">If the dialog is not handled by this class, calls this other handler next.</param>
    /// <param name="api">An interface exposing Avalonia framework dialogs.</param>
    internal DialogFactory(IDialogFactory? chain, IFrameworkDialogsApi? api)
        : base(chain)
    {
        _api = api ?? new FrameworkDialogsApi();
        // _pathInfo = pathInfo ?? new PathInfoFactory();
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
            AllowMultiple = settings.AllowMultiple ?? false
        };
        await AddSharedSettingsAsync(owner, apiSettings, settings);

        return await _api.ShowOpenFolderDialogAsync(owner.GetRef(), apiSettings).ConfigureAwait(true);
    }

    private async Task<IReadOnlyList<IDialogStorageFile>> ShowOpenFileDialogAsync(IView? owner, OpenFileDialogSettings settings)
    {
        var filters = SyncFilters(settings.Filters);
        var apiSettings = new FilePickerOpenOptions()
        {
            AllowMultiple = settings.AllowMultiple ?? false,
            FileTypeFilter = filters,
            SuggestedFileType = FindMappedFilter(settings.Filters, filters, settings.SuggestedFileType)
            // d.ShowReadOnly = Settings.ShowReadOnly;
            // d.ReadOnlyChecked = Settings.ReadOnlyChecked;
        };
        await AddSharedSettingsAsync(owner, apiSettings, settings);

        var result = await _api.ShowOpenFileDialogAsync(owner.GetRef(), apiSettings).ConfigureAwait(true);
        return WithSelectedFilter(settings, filters, result);
    }

    private async Task<IDialogStorageFile?> ShowSaveFileDialogAsync(IView? owner, SaveFileDialogSettings settings)
    {
        var filters = SyncFilters(settings.Filters);
        var apiSettings = new FilePickerSaveOptions()
        {
            DefaultExtension = string.IsNullOrEmpty(settings.DefaultExtension) ? null : settings.DefaultExtension,
            FileTypeChoices = filters,
            SuggestedFileName = settings.SuggestedFileName,
            SuggestedFileType = FindMappedFilter(settings.Filters, filters, settings.SuggestedFileType)
        };
        if (settings.ShowOverwritePrompt.HasValue)
        {
            apiSettings.ShowOverwritePrompt = settings.ShowOverwritePrompt.Value;
        }

        await AddSharedSettingsAsync(owner, apiSettings, settings);

        var result = await _api.ShowSaveFileDialogAsync(owner.GetRef(), apiSettings).ConfigureAwait(true);
        return WithSelectedFilter(settings, filters, result);
    }

    private async Task AddSharedSettingsAsync(IView? owner, PickerOptions d, PickerDialogSettings s)
    {
        // d.DereferenceLinks = s.DereferenceLinks;
        // d.Directory = s.InitialDirectory;
        // d.InitialFileName = s.InitialFile;
        // d.Filters = SyncFilters(s.Filters);
        var storage = StorageExtensions.GetStorageProvider(owner.GetRef());
        // A detached owner contributes no start folder. The picker call reports the missing provider.
        if (s.SuggestedStartLocation != null && storage != null)
        {
            d.SuggestedStartLocation = await s.SuggestedStartLocation.ToAvaloniaAsync(storage);
        }

        d.Title = s.Title;
    }

    // Records the chosen filter when the dialog returns at least one file.
    private static IReadOnlyList<IDialogStorageFile> WithSelectedFilter(OpenFileDialogSettings settings, IList<FilePickerFileType> mapped, OpenFilePickerResult result)
    {
        var files = (result.Files ?? [])
            .Select(IDialogStorageFile (file) => new AvaloniaDialogStorageFile(file))
            .ToList();
        settings.SelectedFileType = files.Count == 0 ? null : FilterFor(settings.Filters, mapped, result.SelectedFileType);
        return files;
    }

    // Records the chosen filter when the dialog returns a file.
    private static IDialogStorageFile? WithSelectedFilter(SaveFileDialogSettings settings, IList<FilePickerFileType> mapped, SaveFilePickerResult result)
    {
        if (result.File == null)
        {
            settings.SelectedFileType = null;
            return null;
        }

        settings.SelectedFileType = FilterFor(settings.Filters, mapped, result.SelectedFileType);
        return new AvaloniaDialogStorageFile(result.File);
    }

    // The picker returns the file type instance this factory created.
    private static FileFilter? FilterFor(IList<FileFilter> filters, IList<FilePickerFileType> mapped, FilePickerFileType? selected)
    {
        if (selected == null)
        {
            return null;
        }

        for (var i = 0; i < mapped.Count && i < filters.Count; i++)
        {
            if (ReferenceEquals(mapped[i], selected))
            {
                return filters[i];
            }
        }

        return null;
    }

    private static FilePickerFileType? FindMappedFilter(IList<FileFilter> filters, IList<FilePickerFileType> mapped, FileFilter? selected)
    {
        if (selected == null)
        {
            return null;
        }

        var index = selected.IndexIn(filters);
        return index < 0 ? null : mapped[index];
    }

    private static List<FilePickerFileType> SyncFilters(IList<FileFilter> filters) =>
        filters.Select(
            x => new FilePickerFileType(x.NameToString(x.ExtensionsToString()))
            {
                Patterns = x.Extensions?.Select(y => "*." + y.TrimStart('.')).ToList(),
                MimeTypes = x.MimeTypes,
                AppleUniformTypeIdentifiers = x.AppleUniformTypeIdentifiers
            }).ToList();
}
