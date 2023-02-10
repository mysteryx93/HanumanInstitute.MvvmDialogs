using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <inheritdoc cref="IDialogStorageFolder"/>
public class DialogStorageFolder : DialogStorageItem, IDialogStorageFolder
{
    private readonly IStorageFolder _item;
    
    /// <summary>
    /// Initializes a new instance of DialogStorageFolder as a bridge to specified Avalonia IStorageFolder.
    /// </summary>
    /// <param name="item">An Avalonia IStorageFolder from which to get the values.</param>
    public DialogStorageFolder(IStorageFolder item) : base(item)
    {
        _item = item;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IDialogStorageItem>> GetItemsAsync()
    {
        var list = await _item.GetItemsAsync();
        return list.Select(x => x is IStorageFile f ? (IDialogStorageItem)new DialogStorageFile(f) : new DialogStorageFolder((IStorageFolder)x));
    }
}
