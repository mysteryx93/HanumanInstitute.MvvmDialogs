
namespace HanumanInstitute.MvvmDialogs.PathInfo;

/// <summary>
/// Provides information about a file path.
/// </summary>
public interface IFileInfo : IPathInfo
{
    /// <summary>
    /// Gets the size, in bytes, of the file.
    /// </summary>
    long Length { get; }
}
