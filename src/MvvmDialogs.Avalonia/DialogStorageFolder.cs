using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <inheritdoc cref="IDialogStorageFolder"/>
public class AvaloniaDialogStorageFolder : AvaloniaDialogStorageItem, IDialogStorageFolder
{
    private readonly IStorageFolder _item;

    /// <summary>
    /// Initializes a new instance of DialogStorageFolder as a bridge to specified Avalonia IStorageFolder.
    /// </summary>
    /// <param name="item">An Avalonia IStorageFolder from which to get the values.</param>
    public AvaloniaDialogStorageFolder(IStorageFolder item) : base(item)
    {
        _item = item;
    }

    /// <inheritdoc />
    public IAsyncEnumerable<IDialogStorageItem> GetItemsAsync()
    {
        var list = _item.GetItemsAsync();
        return list.Select(x => x is IStorageFile f ? (IDialogStorageItem)new AvaloniaDialogStorageFile(f) : new AvaloniaDialogStorageFolder((IStorageFolder)x));
    }
}
