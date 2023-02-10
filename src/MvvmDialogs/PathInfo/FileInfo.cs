namespace HanumanInstitute.MvvmDialogs.PathInfo;

/// <inheritdoc />
public class FileInfo : IFileInfo
{
    private readonly System.IO.FileInfo _info;

    /// <summary>
    /// Initializes a new instance of the FileInfo class for specified file.
    /// </summary>
    /// <param name="filePath">The path of the file to get information for.</param>
    public FileInfo(string filePath) =>
        _info = new System.IO.FileInfo(filePath);

    /// <summary>
    /// Initializes a new instance of the FileInfo class for specified file.
    /// </summary>
    /// <param name="fileInfo">The System.IO.FileInfo to wrap onto.</param>
    public FileInfo(System.IO.FileInfo fileInfo) =>
        _info = fileInfo;

    /// <inheritdoc />
    public IDirectoryInfo? Parent => _info.Directory != null ? new DirectoryInfo(_info.Directory) : null;

    /// <inheritdoc />
    public string Name => _info.Name;

    /// <inheritdoc />
    public string FullName => _info.FullName;

    /// <inheritdoc />
    public bool Exists => _info.Exists;

    /// <inheritdoc />
    public DateTime CreationTime => _info.CreationTime;

    /// <inheritdoc />
    public DateTime LastWriteTime => _info.LastWriteTime;

    /// <inheritdoc />
    public long Length => _info.Length;
}
