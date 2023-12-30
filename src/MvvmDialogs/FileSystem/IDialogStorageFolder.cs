using System.Collections.Generic;

namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <summary>
/// Manipulates folders and their contents, and provides information about them.
/// </summary>
public interface IDialogStorageFolder : IDialogStorageItem
{
    /// <summary>
    /// Gets the files and subfolders in the current folder.
    /// </summary>
    /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
    /// <returns>When this method completes successfully, it returns a list of the files and folders in the current folder. Each item in the list is represented by an <see cref="IDialogStorageItem"/> implementation object.</returns>
    IAsyncEnumerable<IDialogStorageItem> GetItemsAsync();
    
    // /// <summary>
    // /// Gets the files and subfolders in the current folder.
    // /// </summary>
    // /// <param name="searchPattern">The search string to match against the names of directories.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
    // /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive).</exception>
    // /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
    // /// <returns>When this method completes successfully, it returns a list of the files and folders in the current folder. Each item in the list is represented by an <see cref="IDialogStorageItem"/> implementation object.</returns>
    // Task<IEnumerable<IDialogStorageItem>> GetItemsAsync(string searchPattern);
    // /// <summary>
    // /// Gets the files and subfolders in the current folder.
    // /// </summary>
    // /// <param name="searchPattern">The search string to match against the names of directories.  This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
    // /// <param name="searchOption">One of the enumeration values that specifies whether the search operation should include only the current directory or all subdirectories. The default value is <see cref="F:System.IO.SearchOption.TopDirectoryOnly" />.</param>
    // /// <exception cref="T:System.IO.DirectoryNotFoundException">The path encapsulated in the <see cref="T:System.IO.DirectoryInfo" /> object is invalid (for example, it is on an unmapped drive).</exception>
    // /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
    // /// <returns>When this method completes successfully, it returns a list of the files and folders in the current folder. Each item in the list is represented by an <see cref="IDialogStorageItem"/> implementation object.</returns>
    // Task<IEnumerable<IDialogStorageItem>> GetItemsAsync(string searchPattern, SearchOption searchOption);

    /// <summary>
    /// Creates a file with specified name as a child of the current storage folder
    /// </summary>
    /// <param name="name">The display name</param>
    /// <returns>A new <see cref="IDialogStorageFile"/> pointing to the moved file. If not null, the current storage item becomes invalid</returns>
    Task<IDialogStorageFile?> CreateFileAsync(string name);

    /// <summary>
    /// Creates a folder with specified name as a child of the current storage folder
    /// </summary>
    /// <param name="name">The display name</param>
    /// <returns>A new <see cref="IDialogStorageFolder"/> pointing to the moved file. If not null, the current storage item becomes invalid</returns>
    Task<IDialogStorageFolder?> CreateFolderAsync(string name);
}
