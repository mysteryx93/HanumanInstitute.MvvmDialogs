namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Provides information about a directory path.
/// </summary>
public interface IDirectoryInfo
{
    /// <summary>
    /// Returns whether the directory exists.
    /// </summary>
    bool Exists { get; }
}