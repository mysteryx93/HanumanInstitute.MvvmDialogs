using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using HanumanInstitute.MvvmDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace Demo.CrossPlatform.Services;

public class StorageService : IStorageService
{
    protected virtual IStorageProvider Storage => field ??= GetTopLevel(Application.Current)?.StorageProvider ?? throw new NullReferenceException("No StorageProvider found.");

    public bool CanPickFiles => Storage.CanOpen;

    public bool CanSave => Storage.CanSave;

    public async Task<IDialogStorageFolder?> GetDownloadsFolderAsync()
    {
        var result = await Storage.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads);
        return result?.ToDialog();
    }

    public async Task<IDialogStorageFile?> OpenFileBookmarkAsync(string bookmark)
    {
        var file = await Storage.OpenFileBookmarkAsync(bookmark);
        return file?.ToDialog();
    }

    /// <summary>
    /// Returns the TopLevel from the main window or view. 
    /// </summary>
    /// <param name="app">The application to get the TopLevel for.</param>
    /// <returns>A TopLevel object.</returns>
    private TopLevel? GetTopLevel(Application? app)
    {
        if (app?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        if (app?.ApplicationLifetime is ISingleViewApplicationLifetime viewApp)
        {
            return TopLevel.GetTopLevel(viewApp.MainView);
        }
        return null;
    }
}
