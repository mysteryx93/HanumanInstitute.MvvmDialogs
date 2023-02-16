using HanumanInstitute.MvvmDialogs.FileSystem;
using HanumanInstitute.MvvmDialogs.PathInfo;

namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <inheritdoc />
public abstract class DialogStorageItem : IDialogStorageItem
{
    /// <summary>
    /// Information about the file or directory.
    /// </summary>
    protected readonly IPathInfo InfoBase;

    /// <summary>
    /// Initializes a new instance of DialogStorageItem to expose a path.
    /// </summary>
    /// <param name="info">Information about the path to expose.</param>
    protected DialogStorageItem(IPathInfo info)
    {
        InfoBase = info;
    }

    /// <inheritdoc />
    public string Name => InfoBase.Name;

    /// <inheritdoc />
    public Uri Path => _path ??= new Uri(InfoBase.FullName);
    private Uri? _path;

    /// <inheritdoc />
    public string LocalPath => Path.LocalPath;

    /// <inheritdoc />
    public Task<DialogStorageItemProperties> GetBasicPropertiesAsync()
    {
        var length = InfoBase is IFileInfo f ? (ulong?)f.Length : null;
        return Task.FromResult(new DialogStorageItemProperties(length, new DateTimeOffset(InfoBase.CreationTime), new DateTimeOffset(InfoBase.LastWriteTime)));
    }

    /// <inheritdoc />
    public bool CanBookmark => true;

    /// <inheritdoc />
    public Task<string?> SaveBookmarkAsync() => Task.FromResult((string?)InfoBase.FullName);

    /// <inheritdoc />
    public Task<IDialogStorageFolder?> GetParentAsync()
    {
        var parent = InfoBase.Parent;
        return Task.FromResult<IDialogStorageFolder?>(parent != null ? new DialogStorageFolder(parent) : null);
    }

    /// <inheritdoc />
    public void Dispose() { }
}
