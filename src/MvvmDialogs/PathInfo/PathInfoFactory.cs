namespace HanumanInstitute.MvvmDialogs;

/// <inheritdoc />
public class PathInfoFactory : IPathInfoFactory
{
    /// <inheritdoc />
    public IFileInfo GetFileInfo(string filePath) => new FileInfo(filePath);

    /// <inheritdoc />
    public IDirectoryInfo GetDirectoryInfo(string path) => new DirectoryInfo(path);
}