namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <inheritdoc />
public abstract class AvaloniaDialogStorageItem : IDialogStorageItem
{
    private readonly IStorageItem _item;

    /// <summary>
    /// Initializes a new instance of DialogStorageItem as a bridge to specified Avalonia IStorageItem.
    /// </summary>
    /// <param name="item">An Avalonia IStorageItem from which to get the values.</param>
    protected AvaloniaDialogStorageItem(IStorageItem item)
    {
        _item = item;
    }

    /// <inheritdoc />
    public string Name => _item.Name;

    /// <inheritdoc />
    public Uri Path => _item.Path;

    /// <inheritdoc />
    public string? TryGetLocalPath() => _item.TryGetLocalPath();

    /// <inheritdoc />
    public string LocalPath => _item.TryGetLocalPath() ??
                               throw new InvalidOperationException("This storage item has no local filesystem path. Use TryGetLocalPath or open a stream.");

    /// <inheritdoc />
    public async Task<DesktopDialogStorageItemProperties> GetBasicPropertiesAsync()
    {
        var result = await _item.GetBasicPropertiesAsync().ConfigureAwait(true);
        return new(result.Size, result.DateCreated, result.DateModified);
    }

    /// <summary>
    /// Gets whether the underlying Avalonia item can be bookmarked for later access.
    /// </summary>
    public bool CanBookmark => _item.CanBookmark;

    /// <summary>
    /// Saves a bookmark for the underlying Avalonia item.
    /// </summary>
    /// <returns>The bookmark identifier, or null if the platform did not create one.</returns>
    public Task<string?> SaveBookmarkAsync() => _item.SaveBookmarkAsync();

    /// <summary>
    /// Releases the permission held by the underlying Avalonia bookmark, when present.
    /// </summary>
    public Task ReleaseBookmarkAsync() =>
        _item is IStorageBookmarkItem bookmark ? bookmark.ReleaseBookmarkAsync() : Task.CompletedTask;

    /// <inheritdoc />
    public async Task<IDialogStorageFolder?> GetParentAsync()
    {
        var result = await _item.GetParentAsync().ConfigureAwait(true);
        return result != null ? new AvaloniaDialogStorageFolder(result) : null;
    }

    /// <inheritdoc />
    public Task DeleteAsync() => _item.DeleteAsync();

    /// <inheritdoc />
    public async Task<IDialogStorageItem?> MoveAsync(IDialogStorageFolder destination)
    {
        // Delegate only when the destination is already an Avalonia folder. The wrapper type is not a provider identity.
        if (destination is AvaloniaDialogStorageFolder avaloniaDestination)
        {
            return await _item.MoveAsync(avaloniaDestination.Source) switch
            {
                IStorageFile file => file.ToDialog(),
                IStorageFolder folder => folder.ToDialog(),
                _ => null
            };
        }
        return null;
    }

    /// <inheritdoc />
    public void Dispose() => _item.Dispose();
}
