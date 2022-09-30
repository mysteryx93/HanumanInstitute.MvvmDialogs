namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Provides access to the file system via bookmarks, a method commonly used on mobile devices and browser.
/// Bookmarks provide a string key to the file that can be re-used later, useful for maintaining access to files
/// that the application normally wouldn't get access to.
/// </summary>
public interface IBookmarkFileSystem
{
    /// <summary>
    /// Open <see cref="IDialogStorageFile"/> from the bookmark ID.
    /// </summary>
    /// <param name="bookmark">Bookmark ID.</param>
    /// <returns>Bookmarked file or null if OS denied request.</returns>
    Task<IDialogStorageFile?> OpenFileBookmarkAsync(string bookmark);

    /// <summary>
    /// Open <see cref="IDialogStorageFolder"/> from the bookmark ID.
    /// </summary>
    /// <param name="bookmark">Bookmark ID.</param>
    /// <returns>Bookmarked folder or null if OS denied request.</returns>
    Task<IDialogStorageFolder?> OpenFolderBookmarkAsync(string bookmark);
    
    /// <summary>
    /// Releases the bookmark with specified key.
    /// </summary>
    Task ReleaseFileBookmarkAsync(string bookmark);

    /// <summary>
    /// Releases the bookmark with specified key.
    /// </summary>
    Task ReleaseFolderBookmarkAsync(string bookmark);
}
