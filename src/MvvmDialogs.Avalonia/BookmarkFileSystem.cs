using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace HanumanInstitute.MvvmDialogs.Avalonia;

/// <inheritdoc />
public class BookmarkFileSystem : IBookmarkFileSystem
{
    private IStorageProvider? GetStorageProvider()
    {
        var result = Application.Current?.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime d => d.MainWindow,
            ISingleViewApplicationLifetime s => s.MainView as TopLevel,
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
            return result != null ? new AvaloniaDialogStorageFile(result) : null;
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
            return result != null ? new AvaloniaDialogStorageFolder(result) : null;
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
