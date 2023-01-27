
namespace HanumanInstitute.MvvmDialogs.PathInfo;

/// <summary>
/// Provides information about a file path.
/// </summary>
public interface IFileInfo
{
    /// <summary>
    /// The name of the file.
    /// </summary>
    string FileName { get; }
    /// <summary>
    /// The directory path containing the file.
    /// </summary>
    string? DirectoryName { get; }
    /// <summary>
    /// Whether the file exists.
    /// </summary>
    bool Exists { get; }
    /// <summary>
    /// Returns information about the folder containing the file.
    /// </summary>
    IDirectoryInfo? DirectoryInfo { get; }
}