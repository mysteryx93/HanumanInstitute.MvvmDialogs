namespace HanumanInstitute.MvvmDialogs;

/// <summary>
/// Allows creating IFileInfo classes to get file information.
/// </summary>
public interface IPathInfoFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IFileInfo"/> to get information about a file path.
    /// </summary>
    /// <param name="filePath">The path of the file to get information for.</param>
    /// <returns>A new <see cref="IFileInfo"/> instance.</returns>
    IFileInfo GetFileInfo(string filePath);

    /// <summary>
    /// Creates a new instance of <see cref="IDirectoryInfo"/> to get information about a directory path.
    /// </summary>
    /// <param name="path">The path of the directory to get information for.</param>
    /// <returns>A new <see cref="IDirectoryInfo"/> instance.</returns>
    IDirectoryInfo GetDirectoryInfo(string path);
}