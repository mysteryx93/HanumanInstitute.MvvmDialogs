using System.IO;

namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <inheritdoc cref="IDialogStorageFile"/>
public class DesktopDialogStorageFile : DesktopDialogStorageItem, IDialogStorageFile
{
    private readonly FileInfo _info;
    /// <summary>
    /// Returns file system information about the file.
    /// </summary>
    public FileInfo Info => _info;

    /// <summary>
    /// Initializes a new instance of DesktopDialogStorageFile to expose a path.
    /// </summary>
    /// <param name="info">Information of the file to wrap.</param>
    public DesktopDialogStorageFile(FileInfo info)
    {
        _info = info;
    }
    
    /// <summary>
    /// Initializes a new instance of DesktopDialogStorageFile to expose a path.
    /// </summary>
    /// <param name="path">The path of the file to get information for.</param>
    public DesktopDialogStorageFile(string path)
    {
        _info = new FileInfo(path);
        if (_info == null)
        {
            throw new FileNotFoundException($"File '{path}' not found.");
        }
    }

    /// <inheritdoc />
    protected override FileSystemInfo InfoBase => _info;

    /// <inheritdoc />
    protected override DirectoryInfo? Parent => _info.Directory;

    /// <inheritdoc />
    public Task<Stream> OpenReadAsync() => Task.Run<Stream>(() => File.OpenRead(_info.FullName));

    /// <inheritdoc />
    public Task<Stream> OpenWriteAsync() => Task.Run<Stream>(() => File.OpenWrite(_info.FullName));

    /// <inheritdoc />
    public override Task<IDialogStorageItem?> MoveAsync(IDialogStorageFolder destination)
    {
        if (destination is DesktopDialogStorageFolder storageFolder)
        {
            var newPath = System.IO.Path.Combine(storageFolder.Info.FullName, _info.Name);
            _info.MoveTo(newPath);
            return Task.FromResult<IDialogStorageItem?>(new DesktopDialogStorageFile(destination.LocalPath));
        }
        return Task.FromResult<IDialogStorageItem?>(null);
    }
}
