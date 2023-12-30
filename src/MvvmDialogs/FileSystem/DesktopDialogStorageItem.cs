using System.IO;

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
    public Uri Path => _path ??= new Uri(InfoBase.FullName);
    private Uri? _path;

    /// <inheritdoc />
    public string LocalPath => Path.LocalPath;

    /// <inheritdoc />
    public Task<DesktopDialogStorageItemProperties> GetBasicPropertiesAsync()
    {
        if (InfoBase.Exists)
        {
            var length = InfoBase is FileInfo f ? (ulong?)f.Length : null;
            return Task.FromResult(new DesktopDialogStorageItemProperties(length, InfoBase.CreationTimeUtc, InfoBase.LastWriteTimeUtc));
        }
        return Task.FromResult(new DesktopDialogStorageItemProperties());
    }

    /// <inheritdoc />
    public bool CanBookmark => true;

    /// <inheritdoc />
    public Task<string?> SaveBookmarkAsync() => Task.FromResult((string?)InfoBase.FullName);

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
