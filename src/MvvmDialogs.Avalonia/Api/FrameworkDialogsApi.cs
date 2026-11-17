using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Api;

/// <inheritdoc />
internal class FrameworkDialogsApi : IFrameworkDialogsApi
{
    public async Task<OpenFilePickerResult> ShowOpenFileDialogAsync(ContentControl? owner, FilePickerOpenOptions options)
    {
        if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
        return await GetStorage(owner).OpenFilePickerWithResultAsync(options).ConfigureAwait(true);
    }

    public async Task<SaveFilePickerResult> ShowSaveFileDialogAsync(ContentControl? owner, FilePickerSaveOptions options)
    {
        if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
        return await GetStorage(owner).SaveFilePickerWithResultAsync(options).ConfigureAwait(true);
    }

    public async Task<IReadOnlyList<IDialogStorageFolder>> ShowOpenFolderDialogAsync(ContentControl? owner, FolderPickerOpenOptions options)
    {
        if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
        var result = await GetStorage(owner).OpenFolderPickerAsync(options).ConfigureAwait(true);
        return result.Select(x => new AvaloniaDialogStorageFolder(x)).ToList();
    }

    // A detached owner has no top level, so the picker cannot borrow another window's provider.
    private static IStorageProvider GetStorage(ContentControl owner) => 
        StorageExtensions.GetStorageProvider(owner) ??
        throw new ArgumentException("The dialog owner is not attached to a top-level view, so it has no storage provider.", nameof(owner));
}
