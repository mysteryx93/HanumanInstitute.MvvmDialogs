using System.Reflection;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace Demo.Wpf.CustomOpenFolderDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    private string? _path;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        BrowseFolderCommand = new AsyncRelayCommand(OpenFolderAsync);
    }

    public string? Path
    {
        get => _path;
        private set => SetProperty(ref _path, value);
    }

    public ICommand BrowseFolderCommand { get; }

    private async Task OpenFolderAsync()
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a description",
            SuggestedStartLocation = new DesktopDialogStorageFolder(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
        };

        var result = await _dialogService.ShowOpenFolderDialogAsync(this, settings);
        Path = result?.LocalPath;
    }
}
