using System.IO;

namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <summary>
/// Represents a file. Provides information about the file and its contents, and ways to manipulate them.
/// </summary>
public interface IDialogStorageFile : IDialogStorageItem
{
    /// <summary>
    /// Opens a stream for read access.
    /// </summary>
    Task<Stream> OpenReadAsync();

    /// <summary>
    /// Opens stream for writing to the file.
    /// </summary>
    Task<Stream> OpenWriteAsync();
}
