using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;

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
    public async Task<IReadOnlyList<IDialogStorageItem>> GetItemsAsync()
    {
        var result = await _item.GetItemsAsync().ConfigureAwait(true);
        return result.Select(x => (IDialogStorageItem)(x switch
        {
            IStorageFolder fo => new DialogStorageFolder(fo),
            IStorageFile fi => new DialogStorageFile(fi),
            _ => null!
        })).ToList();
    }
}
