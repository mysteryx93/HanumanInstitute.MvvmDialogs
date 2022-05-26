namespace HanumanInstitute.MvvmDialogs;

/// <inheritdoc />
public class DirectoryInfo : IDirectoryInfo
{
    private readonly System.IO.DirectoryInfo directory;

    /// <summary>
    /// Initializes a new instance of the DirectoryInfo class for specified path.
    /// </summary>
    /// <param name="path">The path of the directory to get information for.</param>
    public DirectoryInfo(string path) =>
        directory = new System.IO.DirectoryInfo(path);

    /// <inheritdoc />
    public bool Exists => directory.Exists;
}