namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <inheritdoc />
public class DesktopDialogStorageFactory : IDesktopDialogStorageFactory
{
    /// <inheritdoc />
    public IDialogStorageFile GetFile(string filePath) => new DesktopDialogStorageFile(filePath);

    /// <inheritdoc />
    public IDialogStorageFolder GetDirectory(string path) => new DesktopDialogStorageFolder(path);
}
