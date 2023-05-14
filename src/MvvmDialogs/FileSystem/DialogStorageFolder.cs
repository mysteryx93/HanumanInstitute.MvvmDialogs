using System.Collections.Generic;
using System.IO;
using System.Linq;
using HanumanInstitute.MvvmDialogs.PathInfo;

namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <inheritdoc cref="IDialogStorageFolder"/>
public class DialogStorageFolder : DialogStorageItem, IDialogStorageFolder
{
    private readonly IDirectoryInfo _info;

    /// <summary>
    /// Initializes a new instance of DialogStorageFolder to expose a path.
    /// </summary>
    /// <param name="info">Information about the path to expose.</param>
    public DialogStorageFolder(IDirectoryInfo info)
        : base(info)
    {
        _info = (IDirectoryInfo)InfoBase;
    }

    /// <inheritdoc />
    public IAsyncEnumerable<IDialogStorageItem> GetItemsAsync() => GetItemsAsync(string.Empty, SearchOption.TopDirectoryOnly);

    private IAsyncEnumerable<IDialogStorageItem> GetItemsAsync(string searchPattern) => GetItemsAsync(searchPattern, SearchOption.TopDirectoryOnly);

    private async IAsyncEnumerable<IDialogStorageItem> GetItemsAsync(string searchPattern, SearchOption searchOption)
    {
        var items = _info.EnumeratePathInfos(searchPattern, searchOption)
            .Select(x => x is IFileInfo f ? (IDialogStorageItem)new DialogStorageFile(f) : new DialogStorageFolder((IDirectoryInfo)x));

        foreach (var item in items)
        {
            yield return item;
        }
    }
}
