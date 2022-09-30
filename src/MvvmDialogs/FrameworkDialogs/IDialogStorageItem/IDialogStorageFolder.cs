using System.Collections.Generic;

namespace HanumanInstitute.MvvmDialogs.FrameworkDialogs;

/// <summary>
/// Manipulates folders and their contents, and provides information about them.
/// </summary>
public interface IDialogStorageFolder : IDialogStorageItem
{
    /// <summary>
    /// Gets the files and subfolders in the current folder.
    /// </summary>
    /// <returns>
    /// When this method completes successfully, it returns a list of the files and folders in the current folder. Each item in the list is represented by an <see cref="IDialogStorageItem"/> implementation object.
    /// </returns>
    Task<IReadOnlyList<IDialogStorageItem>> GetItemsAsync();
}
