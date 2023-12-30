using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using HanumanInstitute.MvvmDialogs.Avalonia;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace Demo.CrossPlatform.Services;

public class StorageService : IStorageService
{
    protected virtual IStorageProvider Storage => _storage ??= GetTopLevel(Application.Current)?.StorageProvider ?? throw new NullReferenceException("No StorageProvider found.");
    private IStorageProvider? _storage;
    
    public async Task<IDialogStorageFolder?> GetDownloadsFolderAsync()
    {
        var result = await Storage.TryGetWellKnownFolderAsync(WellKnownFolder.Documents);
        return result?.ToDialog();
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
            var visualRoot = viewApp.MainView?.GetVisualRoot();
            return visualRoot as TopLevel;
        }
        return null;
    }
}
