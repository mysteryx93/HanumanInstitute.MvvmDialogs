using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HanumanInstitute.MvvmDialogs.PathInfo;

/// <inheritdoc />
public class DirectoryInfo : IDirectoryInfo
{
    private readonly System.IO.DirectoryInfo _info;

    /// <summary>
    /// Initializes a new instance of the DirectoryInfo class for specified path.
    /// </summary>
    /// <param name="path">The path of the directory to get information for.</param>
    public DirectoryInfo(string path) =>
        _info = new System.IO.DirectoryInfo(path);

    /// <summary>
    /// Initializes a new instance of the DirectoryInfo class for specified path.
    /// </summary>
    /// <param name="pathInfo">The System.IO.DirectoryInfo to wrap onto.</param>
    public DirectoryInfo(System.IO.DirectoryInfo pathInfo) =>
        _info = pathInfo;

    /// <inheritdoc />
    public string Name => _info.Name;

    /// <inheritdoc />
    public string FullName => _info.FullName;

    /// <inheritdoc />
    public IDirectoryInfo? Parent => _info.Parent != null ? new DirectoryInfo(_info.Parent) : null;

    /// <inheritdoc />
    public bool Exists => _info.Exists;

    /// <inheritdoc />
    public DateTime CreationTime => _info.CreationTime;

    /// <inheritdoc />
    public DateTime LastWriteTime => _info.LastWriteTime;

    /// <inheritdoc />
    public IEnumerable<IPathInfo> EnumeratePathInfos() => EnumeratePathInfos(string.Empty, SearchOption.TopDirectoryOnly);

    /// <inheritdoc />
    public IEnumerable<IPathInfo> EnumeratePathInfos(string searchPattern) => EnumeratePathInfos(searchPattern, SearchOption.TopDirectoryOnly);

    /// <inheritdoc />
    public IEnumerable<IPathInfo> EnumeratePathInfos(string searchPattern, SearchOption searchOption) => _info.EnumerateFileSystemInfos(searchPattern, searchOption)
        .Select<FileSystemInfo, IPathInfo>(x => x is System.IO.FileInfo f ? new FileInfo(f) : new DirectoryInfo((System.IO.DirectoryInfo)x));
}
