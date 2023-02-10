namespace HanumanInstitute.MvvmDialogs.PathInfo;

/// <summary>
/// Provides information about a file or directory.
/// </summary>
public interface IPathInfo
{
    /// <summary>
    /// The name of the file or directory.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// The full path.
    /// </summary>
    string FullName { get; }
    /// <summary>
    /// The parent directory.
    /// </summary>
    IDirectoryInfo? Parent { get; }
    /// <summary>
    /// Whether the path exists.
    /// </summary>
    bool Exists { get; }
    /// <summary>
    /// Gets the creation time of the file or directory.
    /// </summary>
    DateTime CreationTime { get; }
    /// <summary>
    /// Gets the time the file or directory was last written to.
    /// </summary>
    DateTime LastWriteTime { get; }
}
