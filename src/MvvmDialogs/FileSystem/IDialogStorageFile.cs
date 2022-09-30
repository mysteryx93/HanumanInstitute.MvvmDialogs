using System.IO;

namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <summary>
/// Represents a file. Provides information about the file and its contents, and ways to manipulate them.
/// </summary>
public interface IDialogStorageFile : IDialogStorageItem
{
    /// <summary>
    /// Returns true, if file is readable.
    /// </summary>
    bool CanOpenRead { get; }

    /// <summary>
    /// Opens a stream for read access.
    /// </summary>
    Task<Stream> OpenReadAsync();

    /// <summary>
    /// Returns true, if file is writeable. 
    /// </summary>
    bool CanOpenWrite { get; }

    /// <summary>
    /// Opens stream for writing to the file.
    /// </summary>
    Task<Stream> OpenWriteAsync();
}
