using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.FrameworkDialogs;
using IOPath = System.IO.Path;

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
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
        };

        var result = _dialogService.ShowOpenFolderDialog(owner, settings);
        Path = result;
    }

    private async Task OpenFolderImplAsync(INotifyPropertyChanged? owner)
    {
        var settings = new OpenFolderDialogSettings
        {
            Title = "This is a description",
            InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!
        };

        var result = await _dialogService.ShowOpenFolderDialogAsync(owner, settings);
        Path = result?.Path?.ToString() ?? "";
    }
}
