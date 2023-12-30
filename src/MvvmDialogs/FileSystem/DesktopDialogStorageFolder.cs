using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <inheritdoc cref="IDialogStorageFolder"/>
public class DesktopDialogStorageFolder : DesktopDialogStorageItem, IDialogStorageFolder
{
    private readonly DirectoryInfo _info;
    /// <summary>
    /// Returns file system information about the file.
    /// </summary>
    public DirectoryInfo Info => _info;
    
    /// <summary>
    /// Initializes a new instance of DialogStorageFolder to expose a path.
    /// </summary>
    /// <param name="info">Information of the folder to wrap.</param>
    public DesktopDialogStorageFolder(DirectoryInfo info)
    {
        _info = info;
    }

    /// <summary>
    /// Initializes a new instance of DialogStorageFolder to expose a path.
    /// </summary>
    /// <param name="path">The path of the folder to get information for.</param>
    public DesktopDialogStorageFolder(string path)
    {
        _info = new DirectoryInfo(path);
    }

    /// <inheritdoc />
    protected override FileSystemInfo InfoBase => _info;

    /// <inheritdoc />
    protected override DirectoryInfo? Parent => _info.Parent;

    /// <inheritdoc />
    public IAsyncEnumerable<IDialogStorageItem> GetItemsAsync() => GetItemsAsync(string.Empty, SearchOption.TopDirectoryOnly);

    private IAsyncEnumerable<IDialogStorageItem> GetItemsAsync(string searchPattern) => GetItemsAsync(searchPattern, SearchOption.TopDirectoryOnly);

    private async IAsyncEnumerable<IDialogStorageItem> GetItemsAsync(string searchPattern, SearchOption searchOption)
    {
        var items = _info.EnumerateFileSystemInfos(searchPattern, searchOption)
            .Select(x => x is FileInfo f ? (IDialogStorageItem)new DesktopDialogStorageFile(f) : new DesktopDialogStorageFolder((DirectoryInfo)x));

        foreach (var item in items)
        {
            yield return item;
        }
    }

    // /// <inheritdoc />
    // public IEnumerable<FileSystemInfo> EnumeratePathInfos(string searchPattern, SearchOption searchOption) => _info.EnumerateFileSystemInfos(searchPattern, searchOption);

    /// <inheritdoc />
    public Task<IDialogStorageFile?> CreateFileAsync(string name)
    {
        var path = System.IO.Path.Combine(LocalPath, name);
        File.Create(path);
        return Task.FromResult<IDialogStorageFile?>(new DesktopDialogStorageFile(path));
    }

    /// <inheritdoc />
    public Task<IDialogStorageFolder?> CreateFolderAsync(string name)
    {
        var path = System.IO.Path.Combine(LocalPath, name);
        Directory.CreateDirectory(path);
        return Task.FromResult<IDialogStorageFolder?>(new DesktopDialogStorageFolder(path));
    }

    /// <inheritdoc />
    public override Task<IDialogStorageItem?> MoveAsync(IDialogStorageFolder destination)
    {
        if (destination is DesktopDialogStorageFolder storageFolder)
        {
            var newPath = System.IO.Path.Combine(storageFolder.Info.FullName, Info.Name);
            Info.MoveTo(newPath);

            return Task.FromResult<IDialogStorageItem?>(new DesktopDialogStorageFolder(newPath));
        }

        return Task.FromResult<IDialogStorageItem?>(null);
    }
}
