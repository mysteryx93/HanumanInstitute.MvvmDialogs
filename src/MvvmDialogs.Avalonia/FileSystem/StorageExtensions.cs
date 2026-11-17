using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <summary>
/// Provides extension methods to convert between Avalonia and MvvmDialog storage file/folder types.
/// </summary>
public static class StorageExtensions
{
    /// <summary>
    /// Converts an Avalonia <see cref="IStorageFile"/> into a MvvmDialogs <see cref="IDialogStorageFile"/>. 
    /// </summary>
    /// <param name="item">The Avalonia storage object to convert.</param>
    /// <returns>A MvvmDialogs storage object.</returns>
    public static IDialogStorageFile ToDialog(this IStorageFile item) => new AvaloniaDialogStorageFile(item);

    /// <summary>
    /// Converts an Avalonia <see cref="IStorageFolder"/> into a MvvmDialogs <see cref="IDialogStorageFolder"/>.
    /// </summary>
    /// <param name="item">The Avalonia storage object to convert.</param>
    /// <returns>A MvvmDialogs storage object.</returns>
    public static IDialogStorageFolder ToDialog(this IStorageFolder item) => new AvaloniaDialogStorageFolder(item);

    // internal static IStorageFile Convert(this IDialogStorageFile item) => new DialogStorageFileToAvalonia(item);

    /// <summary>
    /// Converts a folder for a dialog owned by the supplied provider.
    /// </summary>
    /// <param name="item">The folder chosen as a start location.</param>
    /// <param name="storage">The provider for the dialog owner.</param>
    /// <returns>The Avalonia folder, or null when the provider cannot open it.</returns>
    internal static async Task<IStorageFolder?> ToAvaloniaAsync(this IDialogStorageFolder item, IStorageProvider storage)
    {
        if (item is AvaloniaDialogStorageFolder av)
        {
            return av.Source;
        }

        var localPath = item.TryGetLocalPath();
        if (localPath != null)
        {
            return await storage.TryGetFolderFromPathAsync(localPath).ConfigureAwait(true);
        }

        return await storage.TryGetFolderFromPathAsync(item.Path).ConfigureAwait(true);
    }

    /// <summary>
    /// Gets the storage provider for a dialog owner.
    /// </summary>
    /// <param name="owner">The control that owns the dialog.</param>
    /// <returns>The owner's storage provider, or null when the control has no top level.</returns>
    internal static IStorageProvider? GetStorageProvider(ContentControl? owner) =>
        owner == null ? null : TopLevel.GetTopLevel(owner)?.StorageProvider;
}
