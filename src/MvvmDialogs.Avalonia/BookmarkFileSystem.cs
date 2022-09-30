using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <inheritdoc />
public class BookmarkFileSystem : IBookmarkFileSystem
{
    private IStorageProvider? GetStorageProvider()
    {
        var app = Application.Current?.ApplicationLifetime;
        var result = 0 switch
        {
            _ when app is IClassicDesktopStyleApplicationLifetime d => d.MainWindow,
            _ when app is ISingleViewApplicationLifetime s => s.MainView as TopLevel,
            _ => null
        };
        return result?.StorageProvider;
    }

    /// <inheritdoc />
    public async Task<IDialogStorageFile?> OpenFileBookmarkAsync(string bookmark)
    {
        var storage = GetStorageProvider();
        if (storage != null)
        {
            var result = await storage.OpenFileBookmarkAsync(bookmark).ConfigureAwait(false);
            return result != null ? new DialogStorageFile(result) : null;
        }
        return null;
    }
    
    /// <inheritdoc />
    public async Task<IDialogStorageFolder?> OpenFolderBookmarkAsync(string bookmark)
    {
        var storage = GetStorageProvider();
        if (storage != null)
        {
            var result = await storage.OpenFolderBookmarkAsync(bookmark).ConfigureAwait(false);
            return result != null ? new DialogStorageFolder(result) : null;
        }
        return null;
    }
    
    /// <inheritdoc />
    public async Task ReleaseFileBookmarkAsync(string bookmark)
    {
        var storage = GetStorageProvider();
        if (storage != null)
        {
            var result = await storage.OpenFileBookmarkAsync(bookmark).ConfigureAwait(false);
            if (result != null)
            {
                await result.ReleaseBookmarkAsync().ConfigureAwait(false);
            }
        }
    }
    
    /// <inheritdoc />
    public async Task ReleaseFolderBookmarkAsync(string bookmark)
    {
        var storage = GetStorageProvider();
        if (storage != null)
        {
            var result = await storage.OpenFileBookmarkAsync(bookmark).ConfigureAwait(false);
            if (result != null)
            {
                await result.ReleaseBookmarkAsync().ConfigureAwait(false);
            }
        }
    }
}
