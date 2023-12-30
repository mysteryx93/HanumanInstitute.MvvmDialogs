namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <summary>
/// Allows creating dialog storage classes to get file/folder information. Direct file access will only work in desktop OS.
/// </summary>
public interface IDesktopDialogStorageFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IDialogStorageFile"/> to get information about a file path.
    /// </summary>
    /// <param name="filePath">The path of the file to get information for.</param>
    /// <returns>A new <see cref="IDialogStorageFile"/> instance.</returns>
    IDialogStorageFile GetFile(string filePath);

    /// <summary>
    /// Creates a new instance of <see cref="IDialogStorageFolder"/> to get information about a directory path.
    /// </summary>
    /// <param name="path">The path of the directory to get information for.</param>
    /// <returns>A new <see cref="IDialogStorageFolder"/> instance.</returns>
    IDialogStorageFolder GetDirectory(string path);
}
