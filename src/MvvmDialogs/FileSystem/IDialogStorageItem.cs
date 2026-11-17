namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <summary>
/// Manipulates storage items (files and folders) and their contents, and provides information about them
/// </summary>
/// <remarks>
/// This interface inherits <see cref="IDisposable"/> . It's recommended to dispose <see cref="IDialogStorageItem"/> when it's not used anymore.
/// </remarks>
public interface IDialogStorageItem : IDisposable
{
    /// <summary>
    /// Gets the name of the item including the file name extension if there is one.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the full file-system path of the item, if the item has a path.
    /// </summary>
    /// <remarks>
    /// Android backend might return file path with "content:" scheme.
    /// Browser and iOS backends might return relative uris.
    /// </remarks>
    Uri Path { get; }

    /// <summary>
    /// Gets a local filesystem path for the item. Prefer <see cref="TryGetLocalPath"/> when the item may have none.
    /// </summary>
    /// <exception cref="InvalidOperationException">This storage item has no local filesystem path.</exception>
    string LocalPath { get; }

    /// <summary>
    /// Gets a local filesystem path for the item when the platform exposes one.
    /// </summary>
    /// <returns>The full path, or null when no filesystem path is available.</returns>
    string? TryGetLocalPath();

    /// <summary>
    /// Gets the basic properties of the current item.
    /// </summary>
    Task<DesktopDialogStorageItemProperties> GetBasicPropertiesAsync();

    /// <summary>
    /// Returns true is item can be bookmarked and reused later.
    /// </summary>
    bool CanBookmark { get; }

    /// <summary>
    /// Saves items to a bookmark.
    /// </summary>
    /// <returns>The bookmark identifier, or null if the platform denied the request.</returns>
    Task<string?> SaveBookmarkAsync();

    /// <summary>
    /// Releases the bookmark permission held by this item.
    /// </summary>
    Task ReleaseBookmarkAsync();

    /// <summary>
    /// Gets the parent folder of the current storage item.
    /// </summary>
    Task<IDialogStorageFolder?> GetParentAsync();

    /// <summary>
    /// Deletes the current storage item and it's contents
    /// </summary>
    Task DeleteAsync();

    /// <summary>
    /// Moves the item by delegating to the storage object that backs it.
    /// </summary>
    /// <param name="destination">The folder that should contain the item.</param>
    /// <returns>The item at its new location, or null when the move cannot be delegated.</returns>
    Task<IDialogStorageItem?> MoveAsync(IDialogStorageFolder destination);
}

