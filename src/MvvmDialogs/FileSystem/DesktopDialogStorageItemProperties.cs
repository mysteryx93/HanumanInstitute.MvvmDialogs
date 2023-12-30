namespace HanumanInstitute.MvvmDialogs.FileSystem;

/// <summary>
/// Provides access to the content-related properties of an item (like a file or folder).
/// </summary>
public class DesktopDialogStorageItemProperties
{
    /// <summary>
    /// Initializes a new instance of the DialogStorageItemProperties class.
    /// </summary>
    /// <param name="size">The size of the file in bytes.</param>
    /// <param name="dateCreated">The date and time that the current folder was created.</param>
    /// <param name="dateModified">The date and time of the last time the file was modified.</param>
    public DesktopDialogStorageItemProperties(
        ulong? size = null,
        DateTimeOffset? dateCreated = null,
        DateTimeOffset? dateModified = null)
    {
        Size = size;
        DateCreated = dateCreated;
        DateModified = dateModified;
    }

    /// <summary>
    /// Gets the size of the file in bytes.
    /// </summary>
    /// <remarks>
    /// Can be null if property is not available.
    /// </remarks>
    public ulong? Size { get; }

    /// <summary>
    /// Gets the date and time that the current folder was created.
    /// </summary>
    /// <remarks>
    /// Can be null if property is not available.
    /// </remarks>
    public DateTimeOffset? DateCreated { get; }

    /// <summary>
    /// Gets the date and time of the last time the file was modified.
    /// </summary>
    /// <remarks>
    /// Can be null if property is not available.
    /// </remarks>
    public DateTimeOffset? DateModified { get; }
}
