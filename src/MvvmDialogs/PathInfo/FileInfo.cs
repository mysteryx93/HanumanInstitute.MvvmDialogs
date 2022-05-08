
namespace HanumanInstitute.MvvmDialogs;

/// <inheritdoc />
public class FileInfo : IFileInfo
{
    private readonly System.IO.FileInfo file;

    /// <summary>
    /// Initializes a new instance of the FileInfo class for specified file.
    /// </summary>
    /// <param name="filePath">The path of the file to get information for.</param>
    public FileInfo(string filePath) =>
        file = new System.IO.FileInfo(filePath);

    /// <inheritdoc />
    public string FileName => file.Name;

    /// <inheritdoc />
    public string? DirectoryName => file.DirectoryName;

    /// <inheritdoc />
    public bool Exists => file.Exists;

    /// <inheritdoc />
    public IDirectoryInfo? DirectoryInfo =>
        !string.IsNullOrEmpty(DirectoryName) ? new DirectoryInfo(DirectoryName!) : null;
}
