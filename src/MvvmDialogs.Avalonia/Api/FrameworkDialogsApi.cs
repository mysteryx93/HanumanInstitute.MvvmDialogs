using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Api;

/// <inheritdoc />
internal class FrameworkDialogsApi : IFrameworkDialogsApi
{
    public async Task<IReadOnlyList<IDialogStorageFile>> ShowOpenFileDialogAsync(ContentControl? owner, FilePickerOpenOptions options)
    {
        if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
        var result = await GetStorage(owner).OpenFilePickerAsync(options).ConfigureAwait(true);
        return result.Select(x => new AvaloniaDialogStorageFile(x)).ToList();
    }

    public async Task<IDialogStorageFile?> ShowSaveFileDialogAsync(ContentControl? owner, FilePickerSaveOptions options)
    {
        if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
        var result = await GetStorage(owner).SaveFilePickerAsync(options).ConfigureAwait(true);
        return result != null ? new AvaloniaDialogStorageFile(result) : null;
    }

    public async Task<IReadOnlyList<IDialogStorageFolder>> ShowOpenFolderDialogAsync(ContentControl? owner, FolderPickerOpenOptions options)
    {
        if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
        var result = await GetStorage(owner).OpenFolderPickerAsync(options).ConfigureAwait(true);
        return result.Select(x => new AvaloniaDialogStorageFolder(x)).ToList();
    }

    private static IStorageProvider GetStorage(ContentControl owner) => TopLevel.GetTopLevel(owner)?.StorageProvider ??
                                                                        throw new ArgumentException("Cannot find StorageProvider for specified dialog owner.", nameof(owner));
}
