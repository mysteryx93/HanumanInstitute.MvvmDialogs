using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

namespace HanumanInstitute.MvvmDialogs.Avalonia.Api;

/// <inheritdoc />
internal class FrameworkDialogsApi : IFrameworkDialogsApi
{
    public async Task<IReadOnlyList<IDialogStorageFile>> ShowOpenFileDialogAsync(Window? owner, FilePickerOpenOptions options)
    {
        if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
        var result = await owner.StorageProvider.OpenFilePickerAsync(options).ConfigureAwait(true);
        return result.Select(x => new DialogStorageFile(x)).ToList();
    }

    public async Task<IDialogStorageFile?> ShowSaveFileDialogAsync(Window? owner, FilePickerSaveOptions options)
    {
        if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
        var result = await owner.StorageProvider.SaveFilePickerAsync(options).ConfigureAwait(true);
        return result != null ? new DialogStorageFile(result) : null;
    }

    public async Task<IReadOnlyList<IDialogStorageFolder>> ShowOpenFolderDialogAsync(Window? owner, FolderPickerOpenOptions options)
    {
        if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
        var result = await owner.StorageProvider.OpenFolderPickerAsync(options).ConfigureAwait(true);
        return result.Select(x => new DialogStorageFolder(x)).ToList();
    }
}
