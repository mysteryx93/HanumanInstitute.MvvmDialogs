using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <inheritdoc />
public class AvaloniaDialogStorageItem : IDialogStorageItem
{
    private readonly IStorageItem _item;

    /// <summary>
    /// Initializes a new instance of DialogStorageItem as a bridge to specified Avalonia IStorageItem.
    /// </summary>
    /// <param name="item">An Avalonia IStorageItem from which to get the values.</param>
    public AvaloniaDialogStorageItem(IStorageItem item)
    {
        _item = item;
    }

    /// <inheritdoc />
    public string Name => _item.Name;

    /// <inheritdoc />
    public Uri Path => _item.Path;

    /// <inheritdoc />
    public string LocalPath => _item.Path.LocalPath;

    /// <inheritdoc />
    public async Task<DialogStorageItemProperties> GetBasicPropertiesAsync()
    {
        var result = await _item.GetBasicPropertiesAsync().ConfigureAwait(true);
        return new DialogStorageItemProperties(result.Size, result.DateCreated, result.DateModified);
    }

    /// <inheritdoc />
    public bool CanBookmark => _item.CanBookmark;

    /// <inheritdoc />
    public Task<string?> SaveBookmarkAsync() => _item.SaveBookmarkAsync();

    /// <inheritdoc />
    public async Task<IDialogStorageFolder?> GetParentAsync()
    {
        var result = await _item.GetParentAsync().ConfigureAwait(true);
        return result != null ? new AvaloniaDialogStorageFolder(result) : null;
    }

    /// <inheritdoc />
    public void Dispose() => _item.Dispose();
}
