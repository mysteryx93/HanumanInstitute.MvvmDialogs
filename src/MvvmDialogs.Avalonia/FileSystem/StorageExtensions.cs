using Avalonia.Controls.ApplicationLifetimes;
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
    
    internal static async Task<IStorageFolder?> ToAvaloniaAsync(this IDialogStorageFolder item)
    {
        if (item is AvaloniaDialogStorageFolder av)
        {
            return av.Source;
        }
        var storage = GetStorageProvider();
        if (storage != null)
        {
            return await storage.TryGetFolderFromPathAsync(item.Path);
        }
        return null;
    }
    
    private static IStorageProvider? GetStorageProvider()
    {
        var result = Application.Current?.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime d => d.MainWindow,
            ISingleViewApplicationLifetime s => s.MainView as TopLevel,
            _ => null
        };
        return result?.StorageProvider;
    }
}
