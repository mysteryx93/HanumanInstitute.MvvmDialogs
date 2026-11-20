namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <inheritdoc />
public abstract class DesktopDialogStorageItem : IDialogStorageItem
{
    /// <summary>
    /// Information about the file or directory.
    /// </summary>
    protected abstract FileSystemInfo InfoBase { get; }

    /// <inheritdoc />
    public string Name => InfoBase.Name;

    /// <inheritdoc />
    public Uri Path => _path ??= new(InfoBase.FullName);
    private Uri? _path;

    /// <inheritdoc />
    public string? TryGetLocalPath() => InfoBase.FullName;

    /// <inheritdoc />
    public string LocalPath => InfoBase.FullName;

    /// <inheritdoc />
    public Task<DesktopDialogStorageItemProperties> GetBasicPropertiesAsync()
    {
        var info = InfoBase;
        info.Refresh();
        if (info.Exists)
        {
            var length = info is FileInfo f ? (ulong?)f.Length : null;
            return Task.FromResult(new DesktopDialogStorageItemProperties(length, info.CreationTimeUtc, info.LastWriteTimeUtc));
        }
        return Task.FromResult(new DesktopDialogStorageItemProperties());
    }

    /// <inheritdoc />
    public Task<IDialogStorageFolder?> GetParentAsync()
    {
        var parent = Parent;
        return Task.FromResult<IDialogStorageFolder?>(parent != null ? new DesktopDialogStorageFolder(parent) : null);
    }

    /// <summary>
    /// 
    /// </summary>
    protected abstract DirectoryInfo? Parent { get; }

    /// <inheritdoc />
    public Task DeleteAsync() => Task.Run(() => InfoBase.Delete());

    /// <inheritdoc />
    public abstract Task<IDialogStorageItem?> MoveAsync(IDialogStorageFolder destination);

    /// <inheritdoc />
    public void Dispose() { }
}
