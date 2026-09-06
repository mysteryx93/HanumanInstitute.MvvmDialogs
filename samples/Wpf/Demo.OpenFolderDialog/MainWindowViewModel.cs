using System.Reflection;
using HanumanInstitute.MvvmDialogs.FileSystem;

namespace Demo.Wpf.OpenFolderDialog;

public class MainWindowViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;

    public MainWindowViewModel(IDialogService dialogService)
    {
        this._dialogService = dialogService;

        OpenFolder = new RelayCommand(() => OpenFolderImpl(SetOwner ? this : null));
        OpenFolderAsync = new AsyncRelayCommand(() => OpenFolderImplAsync(SetOwner ? this : null));
    }

    public string? Path
    {
        get => _path;
        private set => SetProperty(ref _path, value);
    }
    private string? _path;

    public bool SetOwner
    {
        get => _setOwner;
        set => SetProperty(ref _setOwner, value);
    }
    private bool _setOwner = true;


    public ICommand OpenFolder { get; }
    public ICommand OpenFolderAsync { get; }

    private void OpenFolderImpl(INotifyPropertyChanged? owner)
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a description",
            SuggestedStartLocation = new DesktopDialogStorageFolder(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
        };

        var result = _dialogService.ShowOpenFolderDialog(owner, settings);
        Path = result?.Path?.LocalPath;
    }

    private async Task OpenFolderImplAsync(INotifyPropertyChanged? owner)
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a description",
            SuggestedStartLocation = new DesktopDialogStorageFolder(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
        };

        var result = await _dialogService.ShowOpenFolderDialogAsync(owner, settings);
        Path = result?.Path?.LocalPath;
    }
}
